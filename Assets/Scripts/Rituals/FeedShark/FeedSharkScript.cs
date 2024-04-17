using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


public class FeedSharkScript : RitualPanel
{

    public GameObject scuba;
    //public SharkScript shark;
    public GameObject button;
    public AudioClip pass;
    public Text text;
    public int hit = 0;
    public float lightspeed = 1f;
    public float cooldown = 3;

    Color32 green = new Color32(47, 255, 74, 255);
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(scuba != null);
        //Debug.Assert(shark != null);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            destroyUI();
        }
        text.text = texthit();
    }
    public void gameOver()
    {
        ritual.GetComponent<Ritual>().completed = true;
        StartCoroutine(WinEffect());
    }

    IEnumerator WinEffect()
    {
        DisableInteractableButton();
        AudioSource.PlayClipAtPoint(pass, Vector3.zero);
        yield return new WaitForSeconds(lightspeed);
        destroyUI();
    }
    public void DisableInteractableButton()
    {
        button.GetComponent<Button>().interactable = false;
    }
    public void destroyUI()
    {
        Destroy(gameObject);
    }

    public void buttonClick()
    {
            GameObject manage = this.transform.Find("Scuba").gameObject;
            ScubaScript other = manage.GetComponent<ScubaScript>();
            other.ButtonClick();
            StartCoroutine(CoolDown());
    }
    public void EnableInteractableButton()
    {
        button.GetComponent<Button>().interactable = true;
    }
    IEnumerator CoolDown()
    {
        DisableInteractableButton();
        yield return new WaitForSeconds(1.75f);
        EnableInteractableButton();
    }
    public void IncrementHit()
    {
        hit++;
    }

    public string texthit()
    {
        return "Amount hit: (" + hit + ")";
    }

    public void exitButton()
    {
        destroyUI();
    }
}
