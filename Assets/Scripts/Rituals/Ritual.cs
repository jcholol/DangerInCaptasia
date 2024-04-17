
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ritual : MonoBehaviourPun, IPunObservable
{
    public enum RitualType
    {
        ClickFast,
        KeyPad,
        Simon,
        Shark,
        PictureSlider,
        TypeSentence,
        CardMatch,
        Sense,
        CardSwipe
    }

    public const int RITUAL_TYPE_COUNT = 9;

    public static int RITUAL_NUMBER_COUNTER = 0;

    public RitualType ritualType;
    public bool completed = false;
    public int ritualNumber;

    [Header("This Objects Sprite Renderer")]
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        ritualNumber = RITUAL_NUMBER_COUNTER++;
        PhotonNetwork.AddCallbackTarget(this);
    }

    void Update()
    {
        if (completed)
        {
            spriteRenderer.color = new Color(0, 255, 0);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ritualType);
            stream.SendNext(completed);
            stream.SendNext(ritualNumber);
        } else
        {
            ritualType = (RitualType)stream.ReceiveNext();
            completed = (bool)stream.ReceiveNext();
            ritualNumber = (int)stream.ReceiveNext();
        }
    }

    public void raiseKeyEvent()
    {
        PhotonView.Get(this).RPC("RPC_UseKey", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_UseKey()
    {
        if (PhotonView.Get(this).IsMine)
        {
            completed = true;
        }
    }
}
