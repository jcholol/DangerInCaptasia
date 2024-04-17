using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ItemEffect : MonoBehaviour, IPunObservable
{
    public enum ItemEffectType
    {
        BANANA_PEEL,
        FLASH_LIGHT
    }

    public ItemEffectType effect;

    void Start()
    {
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(effect);
        } else
        {
            effect = (ItemEffectType) stream.ReceiveNext();
        }
    }

    public void raiseDestroyBananaEvent()
    {
        PhotonView.Get(this).RPC("DestroyBananaPeel", RpcTarget.All);
    }

    [PunRPC]
    public void DestroyBananaPeel()
    {
        if (this.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
        }
    }
}
