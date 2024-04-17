using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraFollowPlayer : MonoBehaviourPunCallbacks
{
    public GameObject playerToFollow;

    // Update is called once per frame
    void Update()
    {
        if (playerToFollow == null)
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in playerList)
            {
                PhotonView photonView = player.GetComponent<PhotonView>();

                if (photonView.IsMine)
                {
                    playerToFollow = player;
                }
            }
        } else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, 
                new Vector3(playerToFollow.transform.position.x,
                    playerToFollow.transform.position.y, this.transform.position.z), 
                Time.deltaTime);
        }
    }
}
