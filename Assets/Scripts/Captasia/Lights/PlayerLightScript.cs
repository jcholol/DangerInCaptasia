using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerLightScript : MonoBehaviour
{
    public PhotonView photonView;
    public new Light2D light;

    void Start()
    {
        if (!photonView.IsMine)
        {
            light.enabled = false;
            return;
        }

        light.enabled = true;
    }
}
