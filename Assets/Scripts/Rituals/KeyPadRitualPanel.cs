using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class KeyPadRitualPanel : RitualPanel
{
    //public variables and whatnot
    public Text code = null;
    private int[] digit = new int[5];
    public GameObject[] buttons;
    public GameObject[] entered;
    public bool won = false;
    public int buttonsClicked = 0;
    public float lightspeed;
    public AudioClip pass;
    public AudioClip fail;
    public AudioClip buttonClick;

    Color32 red = new Color32(248, 0, 47, 255);
    Color32 white = new Color32(255, 255, 255, 255);
    Color32 green = new Color32(47, 255, 74, 255);
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(code != null);
        Debug.Assert(buttons != null);
        setFiveDigit();
        code.text = codeString();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            destroyUI();
        }
    }

    //text for code
    public string codeString()
    {
        return "Code: " + readFiveDigit();
    }

    //when button is clicked, continues to match with correct digit;
    public void buttonClickOrder(int button)
    {
        buttonsClicked++;
        AudioSource.PlayClipAtPoint(buttonClick, Vector3.zero);
        if (button == digit[buttonsClicked - 1])
        {
            Image image = entered[buttonsClicked - 1].GetComponent<Image>();
            image.enabled = true;
        }
        else
        {
            //resets the game
            StartCoroutine(PlayEffect());
            AudioSource.PlayClipAtPoint(fail, Vector3.zero);
            won = false;
            buttonsClicked = 0;
            setFiveDigit();
            code.text = codeString();
            resetImages();
        }
        if(buttonsClicked == 5)
        {
            won = true;
        }
        if(won == true)
        {
            StartCoroutine(WinEffect());
            
        }
    }

    //plays win effect
    IEnumerator WinEffect()
    {
        DisableInteractableButtons();
        AudioSource.PlayClipAtPoint(pass, Vector3.zero);
        transform.GetComponent<Image>().color = green;
        yield return new WaitForSeconds(lightspeed);
        transform.GetComponent<Image>().color = white;
        ritual.GetComponent<Ritual>().completed = true;
        destroyUI();
    }

    //plays lose effect
    IEnumerator PlayEffect()
    {
        DisableInteractableButtons();
        transform.GetComponent<Image>().color = red;
        yield return new WaitForSeconds(lightspeed);
        EnableInteractableButtons();
        transform.GetComponent<Image>().color = white;
        yield return new WaitForSeconds(lightspeed);
    }

    //disables button
    public void DisableInteractableButtons()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Button>().interactable = false;
        }
    }

    //enables button
    public void EnableInteractableButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Button>().interactable = true;
        }
    }

    //resets the image
    public void resetImages()
    {
        for(int i = 0; i < entered.Length; i++)
        {
            Image image = entered[i].GetComponent<Image>();
            image.enabled = false;
        }
    }

    //sets five digit number
    private bool setFiveDigit()
    {
        for(int i = 0; i < 5; i++)
        {
            digit[i] = Random.Range(1, 10);
        }
        return true;
    }

    //converts digit to string
    public string readFiveDigit()
    {
        string s = "";
        for(int i = 0; i < 5; i++)
        {
            s = s + digit[i];
        }
        return s;
    }

    void destroyUI()
    {
        Destroy(this.gameObject);
    }

    public void exitButton()
    {
        destroyUI();
    }
}
