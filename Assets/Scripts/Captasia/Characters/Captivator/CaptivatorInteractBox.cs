using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CaptivatorInteractBox : MonoBehaviour
{
    [Header("Captivator Reference")]
    public Captivator captivatorRef;

    #region Collision Functions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!captivatorRef.GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        // Handle Interaction with an explorer
        if (collision.tag == "Player" && collision.GetComponent<Explorer>() != null)
        {
            // Handle interaction with capsuled explorer
            if (collision.GetComponent<Explorer>().animator.GetCurrentAnimatorStateInfo(0).IsName("Capsuled"))
            {
                captivatorRef.foundCapsuledExplorer = collision.gameObject;
            }
        }

        if (collision.tag == "Podium")
        {
            captivatorRef.foundPodium = collision.gameObject;
        }

        if (collision.tag == "ItemEffect")
        {
            if (collision.GetComponent<ItemEffect>().effect == ItemEffect.ItemEffectType.BANANA_PEEL) 
            {
                captivatorRef.stunnedDuration = 3;
                collision.GetComponent<ItemEffect>().raiseDestroyBananaEvent();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!captivatorRef.GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        if (collision.tag == "Player" && collision.GetComponent<Explorer>() != null)
        {
            if (collision.GetComponent<Explorer>().animator.GetCurrentAnimatorStateInfo(0).IsName("Capsuled"))
            {
                captivatorRef.foundCapsuledExplorer = collision.gameObject;
            }
        }

        if (collision.tag == "Podium")
        {
            captivatorRef.foundPodium = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!captivatorRef.GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        if (collision.gameObject == captivatorRef.foundCapsuledExplorer)
        {
            captivatorRef.foundCapsuledExplorer = null;
        }

        if (collision.gameObject == captivatorRef.foundPodium)
        {
            captivatorRef.foundPodium = null;
        }
    }

    #endregion
}
