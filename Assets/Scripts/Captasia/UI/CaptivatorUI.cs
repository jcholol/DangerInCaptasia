using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CaptivatorUI : MonoBehaviourPunCallbacks
{
    [Header("Captivator Reference")]
    public Captivator captivatorRef;

    [Header("Text Components")]
    public Text capturedText;
    public Text ritualCounterText;

    [Header("Button Components")]
    public Button attackButton;
    public Button grabCapsuleButton;
    public Button dropCapsuleButton;
    public Button placeOnPodiumButton;

    [Header("Map UI Components")]
    public GameObject MapUIPanel;
    public RawImage Map;

    void Update()
    {
        // Handle Map UI
        if (Input.GetButtonDown("Tab") && !MapUIPanel.activeSelf)
        {
            MapUIPanel.SetActive(true);
        } else if (Input.GetButtonDown("Tab") && MapUIPanel.activeSelf)
        {
            MapUIPanel.SetActive(false);
        }

        handleCoolDown();

        // Button Component Updates
        handleGrabCapsuleButton();
        handleDropCapsuleButton();
        handlePlaceCapsuleButton();

        // Text Component Updates
        handleCapturedText();
        ritualCounterHandler();

        handleControllerInput();
    }

    private void handleControllerInput()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Attack();
        }
    }

    #region Text Handler Methods

    private void handleCapturedText()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        int capturedCount = 0;

        // Look through all players in game and count how many players are captured
        foreach(GameObject player in playerList)
        {
            PhotonView targetView = player.GetComponent<PhotonView>();
            if (targetView != null)
            {
                if (targetView.Controller.CustomProperties.TryGetValue(
                    CustomProperties.EXPLORER_COMPLETELY_CAPSULED_KEY, out object capsuled))
                {
                    if ((bool)capsuled)
                        capturedCount++;
                }
            }
        }

        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        capturedText.text = "Captured: " + capturedCount + " / " + (PhotonNetwork.CurrentRoom.PlayerCount - 1);
    }

    private void ritualCounterHandler()
    {
        GameObject[] ritualList = GameObject.FindGameObjectsWithTag("Ritual");

        int counter = 0;

        foreach (GameObject ritual in ritualList)
        {
            if (ritual.GetComponent<Ritual>().completed)
            {
                counter++;
            }
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.MAX_RITUAL_COUNTER_KEY, out object num))
        {
            ritualCounterText.text = "Rituals Completed: " + counter + " / " + (int)num;
        }
    }

    #endregion

    #region Button Handler Methods

    private void handleCoolDown()
    {
        attackButton.GetComponent<Image>().fillAmount = captivatorRef.attackCooldown / captivatorRef.ATTACK_MAX_COOLDOWN;
    }

    private void handleGrabCapsuleButton()
    {
        if (captivatorRef.foundCapsuledExplorer != null &&
            captivatorRef.captive == null)
        {
            PhotonView targetView = captivatorRef.foundCapsuledExplorer.GetComponent<PhotonView>();
            if (targetView.Controller.CustomProperties.TryGetValue(CustomProperties.EXPLORER_PODIUM_KEY, out object num))
            {
                if ((int) num >= 0)
                {
                    grabCapsuleButton.interactable = false;
                    return;
                }
            }
            grabCapsuleButton.interactable = true;
        } else
        {
            grabCapsuleButton.interactable = false;
        }
    }

    private void handleDropCapsuleButton()
    {
        if (captivatorRef.captive != null &&
            captivatorRef.foundPodium == null)
        {
            dropCapsuleButton.gameObject.SetActive(true);
        } else
        {
            dropCapsuleButton.gameObject.SetActive(false);
        }
    }

    private void handlePlaceCapsuleButton()
    {
        if (captivatorRef.captive != null &&
            captivatorRef.foundPodium != null &&
            !captivatorRef.foundPodium.GetComponent<Podium>().occupied) {
            placeOnPodiumButton.gameObject.SetActive(true);
        } else
        {
            placeOnPodiumButton.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Button Methods

    public void Attack()
    {
        if (captivatorRef.attackCooldown >= captivatorRef.ATTACK_MAX_COOLDOWN)
        {
            captivatorRef.photonView.RPC("RPC_Attack", RpcTarget.All);
            captivatorRef.animator.SetTrigger("Attack");
            captivatorRef.attackCooldown = 0;

            StartCoroutine(captivatorRef.attackSpeedReduction());
        }
    }

    public void PickUpExplorer()
    {
        captivatorRef.captive = captivatorRef.foundCapsuledExplorer;
    }

    public void DropCapsule()
    {
        captivatorRef.captive = null;
        captivatorRef.previous_captive_state = null;
    }

    public void PlaceCapsuleOnPodium()
    {
        int podiumNumber = captivatorRef.foundPodium.GetComponent<Podium>().podiumNumber;
        PhotonView targetView = captivatorRef.captive.GetComponent<PhotonView>();

        if (targetView.Controller.CustomProperties.ContainsKey(CustomProperties.EXPLORER_PODIUM_KEY))
        {
            targetView.Controller.CustomProperties[CustomProperties.EXPLORER_PODIUM_KEY] = podiumNumber;
            targetView.Controller.SetCustomProperties(captivatorRef.captive.GetComponent<PhotonView>().Controller.CustomProperties);
        }

        captivatorRef.captive = null;
        captivatorRef.previous_captive_state = null;
    }

    #endregion
}
