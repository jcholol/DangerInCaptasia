
﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorerUI : MonoBehaviour
{
    [Header("Health Bar List")]
    public List<GameObject> heartList;

    [Header("Button Components")]
    public Button interactWithRitualButton;
    public Button pickUpButton;
    public Button interactWithChestButton;
    public Button struggleButton;
    public Button surviveButton;
    public Button rescueButton;
    public Button riskEscapeButton;

    [Header("Gauge Components")]
    public Image EscapeGaugeBG;
    public Image EscapeGaugeFill;
    public Image SurviveGauge;
    public Image RescueGauge;
    public Image RescueGaugeFill;

    [Header("Text Components")]
    public Text ritualCounterText;

    [Header("Inventory Reference")]
    public Inventory inventory;

    [Header("Explorer Reference")]
    public Explorer explorerRef;

    private GameObject parentCanvas;
    private GameObject performingRitual;

    void Start()
    {
        parentCanvas = this.transform.parent.gameObject;
    }

    void Update()
    {
        explorerRef.performingRitual = performingRitual;

        handleControllerInput();

        // Handle Button Updates
        handleInteractWithRitualButton();
        handlePickUpButton();
        handleStruggleButton();
        handleSurviveButton();
        handleChestButton();
        handleRescueButton();
        handleRiskEscapeButton();

        // Handle Gauge Updates
        handleEscapeGauge();
        handleSurviveGauge();
        handleRescueGauge();
        handleRescueGauge();

        // Text Handlers
        ritualCounterHandler();

        handleHeartContainers();
    }

    private void handleControllerInput()
    {
        if (interactWithRitualButton.interactable)
        {
            if (explorerRef.foundRitual != null && Input.GetButtonDown("Interact"))
            {
                InteractWithRitual();
                return;
            }
        }

        if (interactWithChestButton.interactable)
        {
            if (explorerRef.foundChest != null && Input.GetButtonDown("Interact"))
            {
                OpenChest();
                return;
            }
        }

        if (pickUpButton.interactable)
        {
            if (explorerRef.foundItem != null && Input.GetButtonDown("Interact"))
            {
                PickUpItem();
                return;
            }
        }

        if (rescueButton.interactable)
        {
            if (explorerRef.foundExplorer != null && Input.GetButtonDown("Interact"))
            {
                Rescue();
                return;
            }
        }
    }

    private void handleHeartContainers()
    {
        if (explorerRef.MAX_HP > heartList.Count)
        {
            return;
        }

        for (int i = explorerRef.MAX_HP; i < heartList.Count; i++)
        {
            heartList[i].SetActive(false);
        }

        for (int i = 0; i < explorerRef.MAX_HP; i++)
        {
            heartList[i].SetActive(true);

            if (i < explorerRef.hp)
            {
                heartList[i].transform.GetChild(0).gameObject.SetActive(true);
            } else
            {
                heartList[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    #region Button Handler Methods
    private void handleInteractWithRitualButton()
    {
        if (explorerRef.foundRitual != null)
        {
            interactWithRitualButton.interactable = true;
        }
        else
        {
            interactWithRitualButton.interactable = false;
        }
    }

    private void handlePickUpButton()
    {
        if (explorerRef.foundItem != null)
        {
            pickUpButton.interactable = true;
        }
        else
        {
            pickUpButton.interactable = false;
        }
    }

    private void handleStruggleButton()
    {
        if (explorerRef.hp <= 0 && !explorerRef.onPodium)
        {
            struggleButton.gameObject.SetActive(true);
        } else
        {
            struggleButton.gameObject.SetActive(false);
        }
    }

    private void handleSurviveButton()
    {
        if (explorerRef.onPodium && !explorerRef.completelyCapsuled)
        {
            surviveButton.gameObject.SetActive(true);
        } else
        {
            surviveButton.gameObject.SetActive(false);
        }
    }

    private void handleChestButton()
    {
        if (explorerRef.foundChest != null)
        {
            if (explorerRef.foundChest.GetComponent<Chest>().isOpen)
            {
                interactWithChestButton.interactable = false;
            }
            else
            {
                interactWithChestButton.interactable = true;
            }
        }
        else
        {
            interactWithChestButton.interactable = false;
        }
    }

    private void handleRescueButton() 
    {
        if (explorerRef.foundExplorer != null)
        {
            PhotonView targetView = explorerRef.foundExplorer.GetComponent<PhotonView>();

            if (targetView.Controller.CustomProperties.TryGetValue(CustomProperties.EXPLORER_PODIUM_KEY, out object num))
            {
                if ((int)num >= 0 && !explorerRef.foundExplorer.GetComponent<Explorer>().completelyCapsuled)
                {
                    rescueButton.gameObject.SetActive(true);
                    return;
                }
            }
        }

        rescueButton.gameObject.SetActive(false);
    }

    private void handleRiskEscapeButton()
    {
        if (explorerRef.hp <= 0 && !explorerRef.completelyCapsuled && explorerRef.onPodium)
        {
            riskEscapeButton.gameObject.SetActive(true);
        }
        else
        {
            riskEscapeButton.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Gauge Handler Methods

    private void handleEscapeGauge()
    {
        if (explorerRef.hp <= 0 && !explorerRef.onPodium)
        {
            EscapeGaugeBG.gameObject.SetActive(true);
            EscapeGaugeFill.fillAmount = explorerRef.escapeGauge / explorerRef.MAX_ESCAPE_GAUGE;
        }
        else
        {
            EscapeGaugeBG.gameObject.SetActive(false);
        }
    }

    private void handleSurviveGauge()
    {
        if (explorerRef.onPodium && !explorerRef.completelyCapsuled)
        {
            SurviveGauge.gameObject.SetActive(true);
            SurviveGauge.fillAmount = explorerRef.survivalGauge / explorerRef.MAX_SURVIVAL_GAUGE;
        }
        else
        {
            SurviveGauge.gameObject.SetActive(false);
        }
    }

    private void handleRescueGauge()
    {
        if (explorerRef.foundExplorer != null)
        {
            PhotonView targetView = explorerRef.foundExplorer.GetComponent<PhotonView>();

            if (targetView.Controller.CustomProperties.TryGetValue(CustomProperties.EXPLORER_PODIUM_KEY, out object num))
            {
                if ((int)num >= 0 && !explorerRef.foundExplorer.GetComponent<Explorer>().completelyCapsuled)
                {
                    RescueGauge.gameObject.SetActive(true);
                    RescueGaugeFill.fillAmount = explorerRef.rescueGauge / explorerRef.MAX_RESCUE_GAUGE;
                    return;
                }
            }
        }

        RescueGauge.gameObject.SetActive(false);
    }

    #endregion

    #region Text Handler

    private void ritualCounterHandler()
    {
        GameObject[] ritualList = GameObject.FindGameObjectsWithTag("Ritual");

        int counter = 0;

        foreach(GameObject ritual in ritualList)
        {
            if (ritual.GetComponent<Ritual>().completed)
            {
                counter++;
            }
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.MAX_RITUAL_COUNTER_KEY, out object num))
        {
            ritualCounterText.text = "Rituals Completed: " + counter + " / " + (int) num;
        }
    }

    #endregion

    #region Button Methods

    public void InteractWithRitual()
    {
        if (explorerRef.foundRitual.GetComponent<Ritual>().completed)
            return;

        PhotonView targetView = explorerRef.foundRitual.GetComponent<PhotonView>();
        
        // Check who owns the ritual and whether or not they are performing it
        if (targetView.Controller.CustomProperties.TryGetValue(CustomProperties.PERFORMING_RITUAL_NUM_KEY, out object num))
        {
            if ((int) num == explorerRef.foundRitual.GetComponent<Ritual>().ritualNumber)
            {
                return;
            }
        }

        // Transfer ownership of the ritual
        targetView.TransferOwnership(PhotonNetwork.LocalPlayer);

        // Spawns specific ritual based on ritual type
        switch (explorerRef.foundRitual.GetComponent<Ritual>().ritualType)
        {
            case Ritual.RitualType.ClickFast:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.CLICK_FAST_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            case Ritual.RitualType.KeyPad:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.KEYPAD_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            case Ritual.RitualType.Simon:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.SIMON_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            case Ritual.RitualType.Shark:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.SHARK_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            case Ritual.RitualType.PictureSlider:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.PICTURE_SLIDER_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            case Ritual.RitualType.TypeSentence:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.TYPE_SENTENCE_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            case Ritual.RitualType.CardMatch:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.CARD_MATCH_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            case Ritual.RitualType.Sense:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.SENSE_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            case Ritual.RitualType.CardSwipe:
                performingRitual = Instantiate(CaptasiaResources.Instance.Ritual.CARD_SWIPE_RITUAL, parentCanvas.transform);
                performingRitual.GetComponent<RitualPanel>().ritual = explorerRef.foundRitual;
                break;
            default:
                break;
        }
    }

    public void PickUpItem()
    {
        if (explorerRef.foundItem != null)
        {
            inventory.addItem(explorerRef.foundItem.GetComponent<Item>());
            explorerRef.foundItem.GetComponent<PhotonView>().RPC("RPC_DestroyItem", RpcTarget.MasterClient);
            explorerRef.foundItem = null;
        }
    }

    public void Struggle()
    {
        explorerRef.escapeGauge += 0.5f;
    }

    public void Survive()
    {
        explorerRef.survivalGaugeFillAmount /= 10.0f;
    }

    public void RiskEscape()
    {
        int luckRoll = Random.Range(0, 100);

        if (luckRoll >= 80)
        {
            PhotonView targetView = explorerRef.GetComponent<PhotonView>();

            targetView.Controller.CustomProperties[CustomProperties.RESCUED_KEY] = true;
            targetView.Controller.SetCustomProperties(targetView.Controller.CustomProperties);
        } else
        {
            explorerRef.survivalGauge -= 20f;
        }
    }

    public void OpenChest()
    {
        if (explorerRef.foundChest != null)
        {
            PhotonView targetView = explorerRef.foundChest.GetComponent<PhotonView>();

            targetView.RPC("RPC_OpenChest", RpcTarget.MasterClient);
        }
    }

    public void Rescue()
    {
        if (explorerRef.foundExplorer != null)
        {
            explorerRef.rescueGauge += 5f;
        }
    }

    #endregion
}
