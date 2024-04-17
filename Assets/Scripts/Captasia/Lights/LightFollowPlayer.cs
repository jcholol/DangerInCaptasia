using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollowPlayer : MonoBehaviour
{
    public GameObject playerToFollow;

    // Update is called once per frame
    void Update()
    {
        // Find the correct player to follow
        if (playerToFollow == null)
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

            foreach(GameObject player in playerList)
            {
                PhotonView targetView = player.GetComponent<PhotonView>();

                if (targetView != null)
                {
                    if (targetView.IsMine)
                    {
                        playerToFollow = targetView.gameObject;
                    }
                }
            }

            return;
        }

        this.transform.position = playerToFollow.transform.position;
    }
}
