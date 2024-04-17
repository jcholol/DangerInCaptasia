using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Key,
        Banana,
        Map,
        FlashLight,
        CandyBar,
        Lantern
    }

    public ItemType itemType;

    void Update()
    {

    }

    [PunRPC]
    void RPC_DestroyItem()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
        }
    }
}
