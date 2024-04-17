using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameOverScript : MonoBehaviourPunCallbacks
{

    public Text text;
    private string[] textToState;
    public TextAsset textAsset;
    // Start is called before the first frame update
    void Start()
    {
        char[] archDelim = new char[] { '\r', '\n' };
        textToState = textAsset.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries);
        chooseRandomText();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void chooseRandomText()
    {
        text.text = textToState[Random.Range(0, textToState.Length)];
    }

    public void onClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
