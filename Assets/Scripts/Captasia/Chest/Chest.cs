using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Photon.Pun;

public class Chest : MonoBehaviour, IPunObservable
{
    public static List<string> legendaryItems = new List<string>();
    public static List<string> uniqueItems = new List<string>();
    public static List<string> epicItems = new List<string>();
    public static List<string> commonItems = new List<string>();

    public Animator animator;
    public bool isOpen = false;
    public bool droppedItem;

    void Start()
    {
        // Common
        commonItems.Add(CaptasiaResources.ItemPrefabPath.BANANA_ITEM);
        commonItems.Add(CaptasiaResources.ItemPrefabPath.LANTERN_ITEM);
        commonItems.Add(CaptasiaResources.ItemPrefabPath.CANDY_BAR);

        // Epic
        epicItems.Add(CaptasiaResources.ItemPrefabPath.FLASH_LIGHT_ITEM);

        // Unique
        uniqueItems.Add(CaptasiaResources.ItemPrefabPath.MAP_ITEM);

        // Legendary
        legendaryItems.Add(CaptasiaResources.ItemPrefabPath.KEY_ITEM);

        PhotonNetwork.AddCallbackTarget(this);
    }

    void Update()
    {
        if (isOpen && !droppedItem && this.GetComponent<PhotonView>().IsMine)
        {
            droppedItem = true;
            SpawnTieredItem();
        }
    }

    public void SpawnTieredItem()
    {
        int randNum = Random.Range(0, 100);

        if (randNum < 50)
        {
            spawnItem(commonItems);
        }
        else if (randNum >= 50 && randNum < 75)
        {
            spawnItem(epicItems);
        }
        else if (randNum >= 75 && randNum < 90)
        {
            spawnItem(uniqueItems);
        }
        else
        {
            spawnItem(legendaryItems);
        }

    }

    private void spawnItem(List<string> itemList)
    {
        Vector3 itemSpawnLocation = transform.localPosition;
        itemSpawnLocation.y -= 0.5f;

        int randNum = Random.Range(0, itemList.Count);

        PhotonNetwork.InstantiateRoomObject(itemList[randNum], itemSpawnLocation, Quaternion.identity);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isOpen);
            stream.SendNext(droppedItem);
        }
        else
        {
            isOpen = (bool)stream.ReceiveNext();
            droppedItem = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void RPC_OpenChest()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            animator.SetBool("ButtonClick", true);
            isOpen = true;
        }
    }
}