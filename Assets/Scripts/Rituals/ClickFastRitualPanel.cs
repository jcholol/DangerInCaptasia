using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ClickFastRitualPanel : RitualPanel
{
    //game object  and the likes
    public GameObject progressBar;
    private float progressBarHeight;
    public bool win;
    public Image fill;
    private float goal;
    Color32 green = new Color32(47, 255, 74, 255);
    public AudioClip pass;
    public AudioClip clicked;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(progressBar != null);
        //height is initially 0
        progressBarHeight = 0;
        goal = transform.GetComponent<RectTransform>().rect.height;
        fill = progressBar.GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            destroyUI();
        }
        progressBarHeight -= 5f * Time.deltaTime;
        if (progressBarHeight <= 0)
        {
            progressBarHeight = 0;
        }
        updateBar();
    }

    //increase progressbar height
    public void buttonClick()
    {
        AudioSource.PlayClipAtPoint(clicked, Vector3.zero);
        progressBarHeight += 10;
    }

    //fills up bar according to the scale of height and goal
    public void updateBar()
    {
        float scale = progressBarHeight / goal;
        if (scale >= 1)
        {
            scale = 1;
            win = true;
            enabled = false;
            ritual.GetComponent<Ritual>().completed = true;
            StartCoroutine(WinEffect());

        }
        fill.fillAmount = scale;
    }

    public void destroyUI()
    {
        Destroy(this.gameObject);
    }

    //plays wineffect when game is won;
    IEnumerator WinEffect()
    {
        AudioSource.PlayClipAtPoint(pass, Vector3.zero);
        progressBar.transform.GetComponent<Image>().color = green;
        yield return new WaitForSeconds(1);
        destroyUI();
    }

    public void exitButton()
    {
        destroyUI();
    }
}
