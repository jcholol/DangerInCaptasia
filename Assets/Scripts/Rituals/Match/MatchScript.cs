using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchScript : RitualPanel
{
    public Sprite[] spriteImages;

    public int[] cards = new int[8] { 0, 0, 1, 1, 2, 2, 3, 3 };
    public GameObject[] cardObj;
    public Sprite cardBack;
    private bool[] matched = new bool[8] { false, false, false, false, false, false, false, false };
    private int revealed = 0;
    private int shown = -1;
    private int prevIndex = -1;
    private bool win = false;
    public AudioClip pass;
    public AudioClip Button;
    public AudioClip fail;
    public GameObject panel;

    Color32 white = new Color32(255, 255, 255, 255);
    Color32 green = new Color32(47, 255, 74, 255);
    // Start is called before the first frame update
    void Start()
    {
        shuffle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            destroyUI();
        }
    }

    //shuffles deck
    void shuffle()
    {
        recurseShuffle(cards.Length);
    }

    //recursively shuffles
    void recurseShuffle(int n)
    {
        if(n == 0)
        {
            return;
        }
        else
        {
            int swapIndex = Random.Range(0, n);
            int tmp = cards[swapIndex];
            cards[swapIndex] = cards[n - 1];
            cards[n - 1] = tmp;
            recurseShuffle(n - 1);
        }
    }
    //OnButtonClick, will pass an int.
    //if the int matches with shown,
    public void onButtonClick(int id)
    {
        AudioSource.PlayClipAtPoint(Button, Vector3.zero);
        StartCoroutine(HideCards(id));
        checkWin();
        if(win)
        {
           StartCoroutine(WinRoutine());
        }
    }
    IEnumerator WinRoutine()
    {
        AudioSource.PlayClipAtPoint(pass, Vector3.zero);
        transform.GetComponent<Image>().color = green;
        yield return new WaitForSeconds(1);
        transform.GetComponent<Image>().color = white;
        ritual.GetComponent<Ritual>().completed = true;
        destroyUI();
    }


    void destroyUI()
    {
        Destroy(panel);
    }
    IEnumerator HideCards(int id)
    {
        DisableInteractableButtons();
        Image image = cardObj[id].GetComponent<Image>();
        image.sprite = spriteImages[cards[id]];
            revealed++;
            if (revealed == 2)
            {
                if (shown == cards[id])
                {
                    matched[id] = true;
                }
                else
                {
                AudioSource.PlayClipAtPoint(fail, Vector3.zero);
                if (prevIndex != -1)
                    {
                        matched[prevIndex] = false;
                    }
                }
                prevIndex = -1;
                shown = -1;
                revealed = 0;
                yield return new WaitForSeconds(1);

            }
            else
            {
                shown = cards[id];
                prevIndex = id;
                matched[id] = true;
            }
        FlipCards();
        EnableInteractableButtons();
    }
    public void DisableInteractableButtons()
    {
        for (int i = 0; i < cardObj.Length; i++)
        {
            cardObj[i].GetComponent<Button>().interactable = false;
        }
    }

    public void EnableInteractableButtons()
    {
        for (int i = 0; i < cardObj.Length; i++)
        {
            if (!matched[i])
            {
                cardObj[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    public void FlipCards()
    {
        for(int i = 0; i < cardObj.Length; i++)
        {
            if(!matched[i])
            {
                Image image = cardObj[i].GetComponent<Image>();
                image.sprite = cardBack;
            }
        }
    }

    public void checkWin()
    {
        for(int i = 0;i <matched.Length; i++)
        {
            if(!matched[i])
            {
                return;
            }
        }
        win = true;
    }

    public void exitClick()
    {
        destroyUI();
    }
}
