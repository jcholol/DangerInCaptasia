using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HandleMapSelection : MonoBehaviourPunCallbacks
{
    public NetworkManager networkManager;
    public Image mapImage;
    public List<Sprite> mapImageList;

    public int mapIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.MAP_SELECTION_KEY, out object num))
        {
            mapIndex = (int)num;
        }
    }

    // Update is called once per frame
    void Update()
    {
        networkManager.selectedMap = networkManager.mapList[mapIndex];
        mapImage.sprite = mapImageList[mapIndex];
    }

    public void incrementMapIndex()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.MAP_SELECTION_KEY, out object num))
        {
            if ((int)num + 1 >= networkManager.mapList.Count)
            {
                PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.MAP_SELECTION_KEY] = 0;
            }
            else
            {
                PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.MAP_SELECTION_KEY] = (int)num + 1;
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }
    }

    public void decrementMapIndex()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.MAP_SELECTION_KEY, out object num))
        {
            if ((int)num - 1 < 0)
            {
                PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.MAP_SELECTION_KEY] = networkManager.mapList.Count - 1;
            }
            else
            {
                PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.MAP_SELECTION_KEY] = (int)num - 1;
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if (propertiesThatChanged.TryGetValue(CustomProperties.MAP_SELECTION_KEY, out object num))
        {
            mapIndex = (int)num;
        }
    }
}
