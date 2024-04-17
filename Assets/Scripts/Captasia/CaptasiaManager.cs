using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CaptasiaManager : MonoBehaviourPunCallbacks
{
    #region Public Static Variables

    public const string GAME_STATE_TEXT_PATH = "Prefabs/UI/InGame/GameStateText";

    #endregion

    #region Public Variables

    public GameObject yourCharacter;

    [Header("Captasia Generator")]
    public CaptasiaGenerator generator;

    [Header("Dictionaries")]
    public ExitGames.Client.Photon.Hashtable roomStateProperties;

    [Header("Loading Components")]
    public GameObject loadingPanel;
    public Text loadingText;
    public AudioSource gameMusic;


    #endregion

    private bool spawned = false;
    private bool isRitualCountSet = false;

    private float loadTime = 0f;

    #region Unity Update/Start/Awake

    void Awake()
    {
        if (generator != null)
        {
            StartCoroutine(initializeProps());
        } else
        {
            // Setup Network Stuff
            roomStateProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.PLAYER_HAS_LOADED_KEY] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(checkAllPlayersConnected());
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gameOver();
        }

        if (spawned)
        {
            loadingPanel.gameObject.SetActive(false);

            if (!gameMusic.isPlaying)
            {
                gameMusic.Play();
            }
        } else
        {
            loadTime += Time.deltaTime;
            loadingPanel.gameObject.SetActive(true);
            if (loadTime <= 1)
            {
                loadingText.text = "Loading.";
            } else if (loadTime > 1 && loadTime <= 2)
            {
                loadingText.text = "Loading..";
            } else
            {
                loadingText.text = "Loading...";
            }

            if (loadTime > 3)
            {
                loadTime = 0;
            }
        }
    }

    #endregion

    #region Private Methods
    private void gameOver()
    {
        setExplorerWin();
        setCaptivatorWin();

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.EXPLORER_WIN_KEY, out object explorerWon))
        {
            if ((bool)explorerWon)
                PhotonNetwork.LoadLevel("GameOverScene");
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.CAPTIVATOR_WIN_KEY, out object captivatorWon))
        {
            if ((bool)captivatorWon)
                PhotonNetwork.LoadLevel("GameOverScene");
        }
    }
    private void setExplorerWin()
    {
        GameObject[] ritualList = GameObject.FindGameObjectsWithTag("Ritual");

        int counter = 0;

        foreach (GameObject ritual in ritualList)
        {
            if (ritual.GetComponent<Ritual>().completed)
            {
                counter++;
            }
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.MAX_RITUAL_COUNTER_KEY, out object num))
        {
            if (counter >= (int) num)
            {
                PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.EXPLORER_WIN_KEY] = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
            }
        }
    }

    private void setCaptivatorWin()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        int capturedCount = 0;

        // Look through all players in game and count how many players are captured
        foreach (GameObject player in playerList)
        {
            PhotonView targetView = player.GetComponent<PhotonView>();
            if (targetView != null)
            {
                if (targetView.Controller.CustomProperties.TryGetValue(
                    CustomProperties.EXPLORER_COMPLETELY_CAPSULED_KEY, out object capsuled))
                {
                    if ((bool)capsuled)
                        capturedCount++;
                }
            }
        }

        if (capturedCount >= PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.CAPTIVATOR_WIN_KEY] = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }
    }

    private void spawnBasedOnCharacterSelection()
    {
        GameObject gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
        Vector3 spawnPosition = new Vector3(0, 0, 0);

        foreach (GameObject spawnpoint in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    if (spawnpoint.GetComponent<SpawnPoint>().spawnPointNumber == i + 1)
                    {
                        spawnPosition = spawnpoint.transform.position;
                    }
                }
            }
        }

        GameObject captivatorUI;
        GameObject explorerUI;

        switch (PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.SELECTED_CHARACTER_KEY])
        {
            case CustomProperties.Character.CAPTIVATOR:
                yourCharacter = PhotonNetwork.Instantiate("Prefabs/Captivator/Captivator",
                    spawnPosition,
                    Quaternion.identity);

                captivatorUI = Instantiate(CaptasiaResources.Instance.UI.CAPTIVATOR_UI, gameCanvas.transform);
                captivatorUI.GetComponent<CaptivatorUI>().captivatorRef = yourCharacter.GetComponent<Captivator>();

                break;
            case CustomProperties.Character.MATT:
                yourCharacter = PhotonNetwork.Instantiate("Prefabs/Explorer/Matt",
                    spawnPosition,
                    Quaternion.identity);

                explorerUI = Instantiate(CaptasiaResources.Instance.UI.EXPLORER_UI, gameCanvas.transform);
                explorerUI.GetComponent<ExplorerUI>().explorerRef = yourCharacter.GetComponent<Explorer>();
                explorerUI.GetComponent<Inventory>().explorerRef = yourCharacter.GetComponent<Explorer>();

                break;
            case CustomProperties.Character.COOLGUY:
                yourCharacter = PhotonNetwork.Instantiate("Prefabs/Explorer/CoolGuy",
                    spawnPosition,
                    Quaternion.identity);

                explorerUI = Instantiate(CaptasiaResources.Instance.UI.EXPLORER_UI, gameCanvas.transform);
                explorerUI.GetComponent<ExplorerUI>().explorerRef = yourCharacter.GetComponent<Explorer>();
                explorerUI.GetComponent<Inventory>().explorerRef = yourCharacter.GetComponent<Explorer>();

                break;
            case CustomProperties.Character.PAM:
                yourCharacter = PhotonNetwork.Instantiate("Prefabs/Explorer/Pam",
                    spawnPosition,
                    Quaternion.identity);

                explorerUI = Instantiate(CaptasiaResources.Instance.UI.EXPLORER_UI, gameCanvas.transform);
                explorerUI.GetComponent<ExplorerUI>().explorerRef = yourCharacter.GetComponent<Explorer>();
                explorerUI.GetComponent<Inventory>().explorerRef = yourCharacter.GetComponent<Explorer>();

                break;
            case CustomProperties.Character.JEN:
                yourCharacter = PhotonNetwork.Instantiate("Prefabs/Explorer/Jen",
                    spawnPosition,
                    Quaternion.identity);

                explorerUI = Instantiate(CaptasiaResources.Instance.UI.EXPLORER_UI, gameCanvas.transform);
                explorerUI.GetComponent<ExplorerUI>().explorerRef = yourCharacter.GetComponent<Explorer>();
                explorerUI.GetComponent<Inventory>().explorerRef = yourCharacter.GetComponent<Explorer>();

                break;
        }
    }

    #endregion

    #region PUN Callbacks

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        roomStateProperties = propertiesThatChanged;

        if (roomStateProperties.TryGetValue(CustomProperties.GAME_STARTED_KEY, out object started))
        {
            if ((bool)started && !spawned)
            {
                spawnBasedOnCharacterSelection();
                spawned = true;
            }
        }

        if (propertiesThatChanged.TryGetValue(CustomProperties.MAX_RITUAL_COUNTER_KEY, out object num))
        {
            if ((int) num == (PhotonNetwork.CurrentRoom.PlayerCount - 1) *
                (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.RITUALS_REQUIRED_PER_PLAYER] + 1)
            {
                isRitualCountSet = true;
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    #endregion

    #region Coroutines

    IEnumerator checkAllPlayersConnected()
    {
        int connectionCheckCount = 0;
        bool allPlayersLoaded = false;

        while (!allPlayersLoaded && connectionCheckCount < 30)
        {
            connectionCheckCount++;
            yield return new WaitForSeconds(1);

            bool playerNotLoaded = false;

            // Loop through each player in the room
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                object hasLoaded;

                // Check if the player is loaded into the scene
                if (player.CustomProperties.TryGetValue(CustomProperties.PLAYER_HAS_LOADED_KEY, out hasLoaded))
                {
                    if (!(bool)hasLoaded)
                    {
                        playerNotLoaded = true;
                        break;
                    }
                }
                else
                {
                    playerNotLoaded = true;
                    break;
                }
            }

            if (!playerNotLoaded)
                allPlayersLoaded = true;
        }

        if (allPlayersLoaded)
        {
            if (generator != null)
            {
                while (!generator.generatedTiles)
                    yield return new WaitForSeconds(1f);

                StartCoroutine(setRitualCount());

                while (!isRitualCountSet)
                    yield return new WaitForSeconds(1f);

                generator.SpawnObjectives();

                while (!generator.spawnedTrees)
                    yield return new WaitForSeconds(1f);
            } else
            {
                StartCoroutine(setRitualCount());
            }

            roomStateProperties[CustomProperties.GAME_STARTED_KEY] = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomStateProperties);
            Debug.Log("All Players Connected");
        }
        else
        {
            Debug.LogError("NOT ALL PLAYERS CONNECTED ON TIME...");
        }

        yield return null;
    }

    IEnumerator initializeProps()
    {
        generator.GenerateWorld();

        while (!generator.generatedTiles)
            yield return new WaitForSeconds(1f);

        // Setup Network Stuff
        roomStateProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.PLAYER_HAS_LOADED_KEY] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
    }

    IEnumerator setRitualCount()
    {
        roomStateProperties[CustomProperties.MAX_RITUAL_COUNTER_KEY] =
                (PhotonNetwork.CurrentRoom.PlayerCount - 1) *
                (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.RITUALS_REQUIRED_PER_PLAYER] + 1;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomStateProperties);

        yield return new WaitForEndOfFrame();
    }

    #endregion
}