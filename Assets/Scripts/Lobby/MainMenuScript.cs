using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenuScript : MonoBehaviourPunCallbacks
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject audioPanel;

    [Header("Audio Components")]
    public Slider masterVolumeSlider;

    [Header("Button Components")]
    public Button leaveGameButton;
    public GameObject leaveGame;

    [Header("Text Components")]
    public Text openMenuButtonText;

    bool audioPanelOpen = false;

    void Awake()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
    }

    void Update()
    {
        this.transform.SetAsLastSibling();

        // Set and save settings
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);

        // Set volume
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);

        handleLeaveGameButton();
    }

    private void handleLeaveGameButton()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" && !audioPanelOpen)
        {
            leaveGameButton.gameObject.SetActive(true);
        } else
        {
            leaveGameButton.gameObject.SetActive(false);
        }
    }

    #region PUN Callbacks

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Left Room");

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    #endregion

    #region Button Methods

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void OpenAudioPanel()
    {
        audioPanelOpen = true;
        audioPanel.SetActive(true);
    }

    public void CloseAudioPanel()
    {
        audioPanelOpen = false;
        audioPanel.SetActive(false);
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion
}
