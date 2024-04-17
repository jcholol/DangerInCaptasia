using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeTask : RitualPanel
{
    public SwipePoint[] points;
    public float countDown = 0.25f;
    public GameObject button;

    private int currentSwipeIndex = 0;
    private float currCountDown = 0;
    Color32 green = new Color32(47, 255, 74, 255);
    public AudioClip passed;
    public AudioClip failed;
    public bool isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            destroyUI();
        }
        currCountDown -= Time.deltaTime;
    }

    private IEnumerator FinishTask(bool win)
    {
        if(win)
        {
            Debug.Log(currentSwipeIndex + " " + points.Length);
            button.GetComponent<Image>().color = green;
            AudioSource.PlayClipAtPoint(passed, Vector3.zero);
            ritual.GetComponent<Ritual>().completed = true;
            yield return new WaitForSeconds(0.5f);
            destroyUI();
        }
        else
        {

            if (!isPlaying)
            {
                isPlaying = true;
                AudioSource.PlayClipAtPoint(failed, Vector3.zero);
            }
            yield return new WaitForSeconds(0.5f);
            isPlaying = false;
        }
    }
    private void destroyUI()
    {
        Destroy(this.gameObject);
    }

    public void SwipePointTrigger(SwipePoint swipePoint)
    {
        Debug.Log(currentSwipeIndex);
        if(swipePoint == points[currentSwipeIndex])
        {
            currentSwipeIndex++;
            currCountDown = countDown;
        }
        else if (swipePoint != points[currentSwipeIndex] || ( currCountDown <= 0f && currCountDown >= -.35f))
        {
            Debug.Log("Ran here");
            currentSwipeIndex = 0;
            StartCoroutine(FinishTask(false));
        }
        if(currentSwipeIndex >= points.Length && points.Length!= 0)
        {
            Debug.Log(currentSwipeIndex + " " + points.Length);
            currentSwipeIndex = 0;
            StartCoroutine(FinishTask(true));
        }

    }
    public void exitClick()
    {
        destroyUI();
    }
}
