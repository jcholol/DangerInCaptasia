using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SenseScript : RitualPanel
{

    public GameObject passButton;
    public GameObject[] answerButton;
    Color32 green = new Color32(47, 255, 74, 255);
    Color32 red = new Color32(248, 0, 47, 255);
    Color32 black = new Color32(0, 0, 0, 255);
    Color32 white = new Color32(255, 255, 255, 255);
    public Sprite[] sprites;
    private string[] animals = new string[] { "bear", "bird", "cat", "cow" };
    private int[] random = new int[5] { 0, 0, 0, 0, 0 };
    public GameObject revealPanel;
    private int answerIndex;
    public Text notePad;
    private int pass = 0;
    private int toPass = 1;

    public AudioClip passed;
    public AudioClip Button;
    public AudioClip fail;

    public bool interactable = true;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameContinue());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            destroyUI();
        }
    }

    public void onClick(string animal)
    {
        if (interactable)
        {
            interactable = false;
            AudioSource.PlayClipAtPoint(Button, Vector3.zero);
            if (animal == animals[random[answerIndex]])
            {
                passButton.GetComponent<Image>().color = green;
                pass++;
            }
            else
            {
                AudioSource.PlayClipAtPoint(fail, Vector3.zero);
                resetPass();
                pass = 0;
            }
            if (pass  >= toPass)
            {
                ritual.GetComponent<Ritual>().completed = true;
                StartCoroutine(WinEffect());
            }
            else
            {
                StartCoroutine(GameContinue());
            }
        }
    }
    void resetPass()
    {
        passButton.GetComponent<Image>().color = red;
        //for(int i = 0; i < passButton.Length; i++)
        //{
        //    passButton[i].GetComponent<Image>().color = red;
        //}
    }

    //void updatePass()
    //{
    //    for(int i = 0; i < pass; i++)
    //    {
    //        passButton[i].GetComponent<Image>().color = green;
    //    }
    //}
    void randomize()
    {
        for (int i = 0; i < random.Length; i++)
        {
            random[i] = Random.Range(0, 4);
        }
        answerIndex = Random.Range(0, 5);
    }

    IEnumerator GameContinue()
    {
        yield return new WaitForSeconds(1f);
        DisableInteractableButtons();
        eraseText();
        randomize();
        for(int i = 0; i < random.Length; i++)
        {
            revealPanel.GetComponent<Image>().color = white;
            Debug.Log(i);
            revealPanel.GetComponent<Image>().sprite = sprites[random[i]];
            yield return new WaitForSeconds(1f);
            revealPanel.GetComponent<Image>().color = black;
            yield return new WaitForSeconds(1f);
        }
        changeText();
        yield return new WaitForSeconds(1);
        EnableInteractableButtons();
        interactable = true;
    }

    IEnumerator WinEffect()
    {
        DisableInteractableButtons();
        AudioSource.PlayClipAtPoint(passed, Vector3.zero);
        this.gameObject.GetComponent<Image>().color = green;
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<Image>().color = white;
        this.destroyUI();
    }

    private void destroyUI()
    {
        Destroy(this.gameObject);
    }
    public void eraseText()
    {
        notePad.text = "";
    }

    public void DisableInteractableButtons()
    {
        for (int i = 0; i < answerButton.Length; i++)
        {
            answerButton[i].GetComponent<Button>().interactable = false;
        }
    }
    public void EnableInteractableButtons()
    {
        for (int i = 0; i < answerButton.Length; i++)
        {
            answerButton[i].GetComponent<Button>().interactable = true;
        }
    }

    public void changeText()
    {
        string prefix = "";
        switch(answerIndex)
        {
            case 0:
                prefix = "first";
                break;
            case 1:
                prefix = "second";
                break;
            case 2:
                prefix = "third";
                break;
            case 3:
                prefix = "fourth";
                break;
            case 4:
                prefix = "fifth";
                break;
            default:
                break;
        }
        notePad.text = "What animal was shown " + prefix + " ?";
    }
    public void exitButton()
    {
        destroyUI();
    }
}