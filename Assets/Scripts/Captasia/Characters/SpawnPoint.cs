using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPoint : MonoBehaviour, IPunObservable
{
    public int spawnPointNumber;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(spawnPointNumber);
        }
        else
        {
            spawnPointNumber = (int)stream.ReceiveNext();
        }
    }
}
