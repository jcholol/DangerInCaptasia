using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameOverManager : MonoBehaviour
{
    public GameObject GameOverCanvas;

    public GameObject CaptivatorWinPanel;
    public GameObject CaptivatorLosePanel;
    public GameObject ExplorerWinPanel;
    public GameObject ExplorerLosePanel;

    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomProperties.TEAM_KEY, out object team))
        {
            if ((string)team == "Explorer")
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.EXPLORER_WIN_KEY, out object explorerWon))
                {
                    if ((bool)explorerWon)
                    {
                        ExplorerWinPanel.SetActive(true);
                    }
                    else
                    {
                        ExplorerLosePanel.SetActive(true);
                    }
                }
            }
            else if ((string)team == "Captivator")
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.CAPTIVATOR_WIN_KEY, out object captivatorWon))
                {
                    if ((bool)captivatorWon)
                    {
                        CaptivatorWinPanel.SetActive(true);
                    }
                    else
                    {
                        CaptivatorLosePanel.SetActive(true);
                    }
                }
            }
        }
    }
}
