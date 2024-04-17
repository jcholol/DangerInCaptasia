using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExplorerInteractBox : MonoBehaviour
{
    [Header("Explorer Reference")]
    public Explorer explorerRef;

    #region Collision Functions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonView.Get(explorerRef).IsMine)
        {
            return;
        }

        if (!explorerRef.capsuled && !explorerRef.onPodium && !explorerRef.completelyCapsuled)
        {
            if (collision.tag == "Item")
            {
                explorerRef.foundItem = collision.gameObject;
            }

            if (collision.tag == "Ritual")
                explorerRef.foundRitual = collision.gameObject;

            if (collision.tag == "Chest")
            {
                if (!collision.GetComponent<Chest>().isOpen)
                {
                    explorerRef.foundChest = collision.gameObject;
                }
                else
                {
                    explorerRef.foundChest = null;
                }
            }

            if (collision.tag == "Attack")
            {
                if (!explorerRef.isInvulnerable)
                {
                    Destroy(explorerRef.performingRitual);

                    if (explorerRef.hp > 0)
                    {
                        explorerRef.hp--;
                        StartCoroutine(explorerRef.afterHitMovementBoost());
                    }
                    else if (explorerRef.hp == 0)
                    {
                        explorerRef.capsuled = true;
                    }
                }
            }

            if (collision.tag == "Player" && collision.GetComponent<Explorer>())
            {
                explorerRef.foundExplorer = collision.gameObject;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!PhotonView.Get(explorerRef).IsMine)
        {
            return;
        }

        if (!explorerRef.capsuled && !explorerRef.onPodium && !explorerRef.completelyCapsuled)
        {
            if (collision.tag == "Item")
            {
                explorerRef.foundItem = collision.gameObject;
            }

            if (collision.tag == "Ritual")
                explorerRef.foundRitual = collision.gameObject;

            if (collision.tag == "Chest")
            {
                if (!collision.GetComponent<Chest>().isOpen)
                {
                    explorerRef.foundChest = collision.gameObject;
                }
                else
                {
                    explorerRef.foundChest = null;
                }
            }

            if (collision.tag == "Attack")
            {
                if (!explorerRef.isInvulnerable)
                {
                    Destroy(explorerRef.performingRitual);

                    if (explorerRef.hp > 0)
                    {
                        explorerRef.hp--;
                        StartCoroutine(explorerRef.afterHitMovementBoost());
                    }
                    else if (explorerRef.hp == 0)
                    {
                        explorerRef.capsuled = true;
                    }
                }
            }

            if (collision.tag == "Player" && collision.GetComponent<Explorer>())
            {
                explorerRef.foundExplorer = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!PhotonView.Get(explorerRef).IsMine)
        {
            return;
        }

        if (collision.tag == "Item")
            explorerRef.foundItem = null;

        if (collision.tag == "Ritual")
            Destroy(explorerRef.performingRitual);
        explorerRef.foundRitual = null;

        if (collision.tag == "Chest")
            explorerRef.foundChest = null;

        if (collision.tag == "Player" && collision.GetComponent<Explorer>())
        {
            if (explorerRef.foundExplorer == collision.gameObject)
            {
                explorerRef.foundExplorer = null;
            }
        }
    }

    #endregion
}