using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEditor;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager networkManagerInstance;

    private string selectedCharacter = "";

    public string selectedMap = "";

    #region Public Static Variables

    public const string ROOM_TAB_CONTENT_PATH = "Prefabs/UI/SpawnableContent/RoomTabContent";
    public const string PLAYER_TAB_CONTENT_PATH = "Prefabs/UI/SpawnableContent/PlayerTabContent";

    public const string CUSTOM_PROPERTY_MAX_CAPTIVATORS = "MaxCaptivators";
    public const string CUSTOM_PROPERTY_MAX_EXPLORERS = "MaxExplorers";

    public const int MAX_CAPTIVATORS = 1;
    public const int MAX_EXPLORERS = 4;

    #endregion

    #region Public Variables

    [Header("List of Map Names")]
    public List<string> mapList;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject connectingToMasterPanel;
    public GameObject serverLobbyPanel;
    public GameObject roomLobbyPanel;

    [Header("Login Fields")]
    public InputField loginName;
    public Button loginButton;

    [Header("ScrollView Contents")]
    public GameObject serverLobbyContent;
    public GameObject roomLobbyContent;

    [Header("Room Player Dictionary")]
    public Dictionary<string, GameObject> roomTabDict = new Dictionary<string, GameObject>();
    public Dictionary<int, GameObject> roomPlayerTabDict;
    public Dictionary<int, string> characterSelectionDict;

    [Header("Room Creation Fields")]
    public InputField roomNameField;
    public Dropdown maxPlayersDropdown;

    [Header("Room Lobby Components")]
    public Text roomNameText;
    public Text roomCapacityStatusText;
    public Button startGameButton;
    public Button incrementMapIndex;
    public Button decrementMapIndex;

    [Header("Character Selection")]
    public Text selectedCharacterText;
    public Button captivatorButton;
    public Button mattButton;
    public Button coolGuyButton;
    public Button pamButton;
    public Button jenButton;

    #endregion

    #region Unity Update/Start Methods

    void Awake()
    {
        if (networkManagerInstance == null)
        {
            networkManagerInstance = this;
        }
        else
        {
            Debug.Log("Network Manager is a singleton. Deleting this object...");
            Destroy(this);
        }

        // Shows the login panel
        if (PhotonNetwork.IsConnected)
        {
            loginPanel.SetActive(false);
        } else
        {
            loginPanel.SetActive(true);
        }

        selectedMap = mapList[0];
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
            incrementMapIndex.gameObject.SetActive(true);
            decrementMapIndex.gameObject.SetActive(true);
        } else
        {
            startGameButton.gameObject.SetActive(false);
            incrementMapIndex.gameObject.SetActive(false);
            decrementMapIndex.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a new room tab to store in a scroll view content object.
    /// </summary>
    /// <param name="roomName">The name of the room.</param>
    /// <param name="parent">The scroll view content to add room tab too.</param>
    private GameObject makeNewRoomTab(RoomInfo room, Transform parent)
    {
        GameObject newRoomTab = Instantiate(Resources.Load<GameObject>(ROOM_TAB_CONTENT_PATH), parent);
        
        Text roomNameText = newRoomTab.transform.Find("RoomNameText").GetComponent<Text>();
        roomNameText.text = room.Name;

        Text capacityText = newRoomTab.transform.Find("CapacityText").GetComponent<Text>();
        capacityText.text = room.PlayerCount + " / " + room.MaxPlayers;

        Button button = newRoomTab.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            PhotonNetwork.JoinRoom(room.Name);
        });

        return newRoomTab;
    }

    /// <summary>
    /// Create a new player tab to store in a scroll view content object.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="parent">The scroll view content to add room tab too.</param>
    private GameObject makeNewPlayerTab(string playerName, Transform parent)
    {
        GameObject newPlayerTab = Instantiate(Resources.Load<GameObject>(PLAYER_TAB_CONTENT_PATH), parent);
        
        Text playerNameText = newPlayerTab.transform.Find("PlayerNameText").GetComponent<Text>();
        playerNameText.text = playerName;

        Text lockedInCharacterText = newPlayerTab.transform.Find("LockedInCharacter").GetComponent<Text>();
        lockedInCharacterText.text = "Has Not Selected Yet";

        return newPlayerTab;
    }

    /// <summary>
    /// This method will update the text status for the number of players
    /// in the room.
    /// </summary>
    private void updateRoomOccupencyText()
    {
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        int currentPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        roomCapacityStatusText.text = "Room Occupency: " + currentPlayerCount + " / " + maxPlayers;
    }

    /// <summary>
    /// This will update the information on the player tabs
    /// </summary>
    private void updatePlayerTabs()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Text lockedInCharacterText = roomPlayerTabDict[player.ActorNumber].transform
                    .Find("LockedInCharacter").GetComponent<Text>();

            if (player.CustomProperties.ContainsKey(CustomProperties.SELECTED_CHARACTER_KEY))
            {
                lockedInCharacterText.text = (string) player.CustomProperties[CustomProperties.SELECTED_CHARACTER_KEY];
            }
        }
    }

    /// <summary>
    /// This method will create new room options
    /// </summary>
    /// <returns></returns>
    private RoomOptions makeRoomOptions()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        roomOptions.MaxPlayers = byte.Parse(maxPlayersDropdown.options[maxPlayersDropdown.value].text);
        roomOptions.CleanupCacheOnLeave = true;
        roomOptions.DeleteNullProperties = true;

        // delete
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

        // Max number of specific characters
        roomOptions.CustomRoomProperties.Add(CUSTOM_PROPERTY_MAX_CAPTIVATORS, MAX_CAPTIVATORS);
        roomOptions.CustomRoomProperties.Add(CUSTOM_PROPERTY_MAX_EXPLORERS, MAX_EXPLORERS);

        // Captasia game properties
        roomOptions.CustomRoomProperties.Add(CustomProperties.CAPTIVATOR_WIN_KEY, false);
        roomOptions.CustomRoomProperties.Add(CustomProperties.EXPLORER_WIN_KEY, false);
        roomOptions.CustomRoomProperties.Add(CustomProperties.GAME_STARTED_KEY, false);
        roomOptions.CustomRoomProperties.Add(CustomProperties.MAX_RITUAL_COUNTER_KEY, 5);
        roomOptions.CustomRoomProperties.Add(CustomProperties.RITUAL_STATE_COUNTER_KEY, 0);
        roomOptions.CustomRoomProperties.Add(CustomProperties.CAPTURE_COUNTER_KEY, 0);
        roomOptions.CustomRoomProperties.Add(CustomProperties.RITUALS_REQUIRED_PER_PLAYER, 4);
        roomOptions.CustomRoomProperties.Add(CustomProperties.MAP_SELECTION_KEY, 0);

        // World properties
        roomOptions.CustomRoomProperties.Add(CustomProperties.WORLD_SEED, Random.Range(24, 29));
        roomOptions.CustomRoomProperties.Add(CustomProperties.WORLD_SIZE, 25);

        return roomOptions;
    }

    #endregion

    #region PUN Callbacks

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.PLAYER_HAS_LOADED_KEY] = false;
        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.PERFORMING_RITUAL_NUM_KEY] = -1;
        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.EXPLORER_PODIUM_KEY] = -1;
        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.EXPLORER_COMPLETELY_CAPSULED_KEY] = false;
        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.RESCUED_KEY] = false;
        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.CAPTIVE_KEY] = -1;
        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.CHARACTER_VIEW_KEY] = -1;

        // Connect to the server lobby
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        foreach (KeyValuePair<string, GameObject> kvp in roomTabDict)
        {
            Destroy(kvp.Value);
        }

        roomTabDict = new Dictionary<string, GameObject>();

        connectingToMasterPanel.SetActive(false);
        serverLobbyPanel.SetActive(true);
        //Debug.Log("JOINED LOBBY");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        serverLobbyPanel.SetActive(false);
        roomLobbyPanel.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = true;

        // Update local dictionaries
        roomPlayerTabDict = new Dictionary<int, GameObject>();

        // Set RoomName Text
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject newPlayerTab = makeNewPlayerTab(player.NickName, roomLobbyContent.transform);
            roomPlayerTabDict.Add(player.ActorNumber, newPlayerTab);
        }

        updatePlayerTabs();
        updateRoomOccupencyText();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (roomTabDict.TryGetValue(room.Name, out GameObject val))
                {
                    Destroy(val);
                    roomTabDict.Remove(room.Name);
                }
            } else
            {
                if (!roomTabDict.ContainsKey(room.Name))
                {
                    roomTabDict.Add(room.Name, makeNewRoomTab(room, serverLobbyContent.transform));
                } else
                {
                    roomTabDict.TryGetValue(room.Name, out GameObject roomTab);
                    roomTab.transform.Find("CapacityText").GetComponent<Text>().text =
                        room.PlayerCount + " / " + room.MaxPlayers;
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        updatePlayerTabs();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        GameObject newPlayerTab = makeNewPlayerTab(newPlayer.NickName, roomLobbyContent.transform);
        roomPlayerTabDict.Add(newPlayer.ActorNumber, newPlayerTab);

        updateRoomOccupencyText();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Destroy(roomPlayerTabDict[otherPlayer.ActorNumber]);
        roomPlayerTabDict.Remove(otherPlayer.ActorNumber);

        updateRoomOccupencyText();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        selectedCharacter = "";

        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        roomLobbyPanel.SetActive(false);
        serverLobbyPanel.SetActive(true);
    }

    #region FAILED PUN Callbacks

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogError("ERROR CODE: " + returnCode);
        Debug.LogError(message);
    }

    #endregion

    #endregion

    #region Unity UI Methods

    public void Login()
    {
        PhotonNetwork.NickName = loginName.text;

        // Shows the connecting screen panel
        loginPanel.SetActive(false);
        connectingToMasterPanel.SetActive(true);

        // Connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        if (roomNameField.text.Length <= 0)
        {
            Debug.Log("Room Name is Empty! Please type something in.");
            return;
        }

        RoomOptions roomOptions = makeRoomOptions();

        PhotonNetwork.CreateRoom(roomNameField.text, roomOptions);
    }

    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int captivatorCount = 0;
        int explorerCount = 0;

        // Check if all players have locked in a character
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(CustomProperties.SELECTED_CHARACTER_KEY, out object selection))
            {
                if ((string) selection == CustomProperties.Character.CAPTIVATOR)
                {
                    captivatorCount++;
                }

                if ((string)selection == CustomProperties.Character.MATT ||
                    (string)selection == CustomProperties.Character.COOLGUY ||
                    (string)selection == CustomProperties.Character.PAM ||
                    (string)selection == CustomProperties.Character.JEN)
                {
                    explorerCount++;
                }
            }
        }

        // Checks if there are too many captivators
        if (captivatorCount > (int)PhotonNetwork.CurrentRoom.CustomProperties[CUSTOM_PROPERTY_MAX_CAPTIVATORS] ||
            captivatorCount <= 0)
        {
            Debug.Log("Too many or too little captivators locked in");
            return;
        }

        // Checks if there are too many explorers
        if (explorerCount > (int)PhotonNetwork.CurrentRoom.CustomProperties[CUSTOM_PROPERTY_MAX_EXPLORERS] ||
            explorerCount <= 0)
        {
            Debug.Log("Too many or too little explorers locked in");
            return;
        }

        if (captivatorCount + explorerCount < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("Not all players are locked in...");
            return;
        }

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        switch(selectedMap)
        {
            case "ProceduralMap":
                PhotonNetwork.LoadLevel("Captasia");
                break;
            case "AlphaMap":
                PhotonNetwork.LoadLevel("Captasia2");
                break;
            default:
                PhotonNetwork.LoadLevel("Captasia");
                break;
        }
    }

    public void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(roomPlayerTabDict[player.ActorNumber]);
        }

        PhotonNetwork.LeaveRoom();
    }

    public void SelectCaptivator()
    {
        selectedCharacter = CustomProperties.Character.CAPTIVATOR;

        selectedCharacterText.text = captivatorButton.GetComponent<CharacterButtons>().characterDescription.text;
    }

    public void SelectExplorer(string name)
    {
        selectedCharacter = name;

        switch (name)
        {
            case CustomProperties.Character.MATT:
                selectedCharacterText.text = mattButton.GetComponent<CharacterButtons>().characterDescription.text;
                break;
            case CustomProperties.Character.COOLGUY:
                selectedCharacterText.text = coolGuyButton.GetComponent<CharacterButtons>().characterDescription.text;
                break;
            case CustomProperties.Character.PAM:
                selectedCharacterText.text = pamButton.GetComponent<CharacterButtons>().characterDescription.text;
                break;
            case CustomProperties.Character.JEN:
                selectedCharacterText.text = jenButton.GetComponent<CharacterButtons>().characterDescription.text;
                break;
        }
    }

    public void LockInCharacter()
    {
        int maxCharacters;
        int currentCharacterCount = 0;

        // Check which max character value to use
        switch (selectedCharacter)
        {
            case CustomProperties.Character.CAPTIVATOR:
                maxCharacters = (int)PhotonNetwork.CurrentRoom.CustomProperties[CUSTOM_PROPERTY_MAX_CAPTIVATORS];
                break;
            case CustomProperties.Character.MATT:
                maxCharacters = (int)PhotonNetwork.CurrentRoom.CustomProperties[CUSTOM_PROPERTY_MAX_EXPLORERS];
                break;
            case CustomProperties.Character.COOLGUY:
                maxCharacters = (int)PhotonNetwork.CurrentRoom.CustomProperties[CUSTOM_PROPERTY_MAX_EXPLORERS];
                break;
            case CustomProperties.Character.PAM:
                maxCharacters = (int)PhotonNetwork.CurrentRoom.CustomProperties[CUSTOM_PROPERTY_MAX_EXPLORERS];
                break;
            case CustomProperties.Character.JEN:
                maxCharacters = (int)PhotonNetwork.CurrentRoom.CustomProperties[CUSTOM_PROPERTY_MAX_EXPLORERS];
                break;
            default:
                Debug.Log("Invalid Character Selection...");
                return;
        }

        // Checks if any other players in the room selected Captivator
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey(CustomProperties.SELECTED_CHARACTER_KEY))
                continue;

            if ((string) player.CustomProperties[CustomProperties.SELECTED_CHARACTER_KEY] == selectedCharacter)
            {
                currentCharacterCount++;
            }
        }

        if (currentCharacterCount >= maxCharacters)
        {
            Debug.Log("Too many players selected this character...");
            return;
        }

        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.SELECTED_CHARACTER_KEY] = selectedCharacter;
        PhotonNetwork.SetPlayerCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
    }

    #endregion
}
