using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Podium : MonoBehaviour, IPunObservable
{
    public static int PODIUM_NUMBER_COUNTER = 0;

    [Header("Capsule Placement Spot")]
    public GameObject capsulePlacementSpot;

    [Header("Podium Number")]
    public int podiumNumber;

    [Header("Is Occupied")]
    public bool occupied;

    void Start()
    {
        podiumNumber = PODIUM_NUMBER_COUNTER++;
        occupied = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(podiumNumber);
            stream.SendNext(occupied);
        } else
        {
            podiumNumber = (int) stream.ReceiveNext();
            occupied = (bool)stream.ReceiveNext();
        }
    }
}
