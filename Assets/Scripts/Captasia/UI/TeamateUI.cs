using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TeamateUI : MonoBehaviour
{
    [Header("Explorer Reference")]
    public Explorer explorerRef;
    public PhotonView playerView;

    [Header("Teamate Index Number")]
    public int teamIndexNum;

    [Header("Teamate Panel Objects")]
    public Image playerImage;
    public Image cageImage;
    public Text playerName;
    public GameObject heartPanel;
    public List<GameObject> heartList;

    // Update is called once per frame
    void Update()
    {
        disableUI();

        int indexCounter = 0;

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            PhotonView targetView = PhotonView.Find((int)player.CustomProperties[CustomProperties.CHARACTER_VIEW_KEY]);

            if (targetView != null && targetView.GetComponent<Explorer>() != null)
            {
                if (indexCounter == teamIndexNum)
                {
                    explorerRef = targetView.GetComponent<Explorer>();
                    playerView = targetView;
                    enableUI();
                }
                indexCounter++;
            }
        }

        if (explorerRef == null)
        {
            return;
        }

        handlePlayerName();
        handlePlayerImage();
        handleHeartContainers();
    }

    private void handlePlayerImage()
    {
        string selectedCharacter = (string)playerView.Controller.CustomProperties[CustomProperties.SELECTED_CHARACTER_KEY];

        switch (selectedCharacter)
        {
            case CustomProperties.Character.MATT:
                playerImage.sprite = CaptasiaResources.Sprites.MATT_ICON;
                break;
            case CustomProperties.Character.COOLGUY:
                playerImage.sprite = CaptasiaResources.Sprites.COOLGUY_ICON;
                break;
            case CustomProperties.Character.PAM:
                playerImage.sprite = CaptasiaResources.Sprites.PAM_ICON;
                break;
            case CustomProperties.Character.JEN:
                playerImage.sprite = CaptasiaResources.Sprites.JEN_ICON;
                break;
        }

        if (explorerRef.hp <= 0 && explorerRef.onPodium)
        {
            playerImage.color = new Color(255, 0, 0);
        } else
        {
            playerImage.color = new Color(255, 255, 255);
        }

        if (explorerRef.completelyCapsuled)
        {
            cageImage.gameObject.SetActive(true);
        } else
        {
            cageImage.gameObject.SetActive(false);
        }
    }

    private void handlePlayerName()
    {
        playerName.text = playerView.Controller.NickName;
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
            }
            else
            {
                heartList[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void disableUI()
    {
        playerImage.gameObject.SetActive(false);
        cageImage.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        heartPanel.SetActive(false);
    }

    private void enableUI()
    {
        playerImage.gameObject.SetActive(true);
        playerName.gameObject.SetActive(true);
        heartPanel.SetActive(true);

        if (explorerRef != null)
        {
            if (explorerRef.completelyCapsuled)
            {
                cageImage.gameObject.SetActive(true);
            }
        }
    }
}
