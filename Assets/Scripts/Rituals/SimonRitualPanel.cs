using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimonRitualPanel : RitualPanel
{
    public AudioClip TaskCompleted;
    public AudioClip TaskFailed;
    public AudioClip buttonPushedAudio;

    private Button button;

    int[] RandomSequence = new int[kNumRounds];

    int CurrRound = 1;
    int shownSequence = 0;
    int userInputRound = 0;

    int userInputNum;

    private const int kNumRounds = 5;
    private const float kWaitTime = 0.5f;
    private float timer = 0.0f;

    private bool userInputComplete = false;
    private bool inputReceived = false;
    private bool failed = false;
    private bool completed = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the random sequence order
        for (int i = 0; i < kNumRounds; i++)
        {
            RandomSequence[i] = Random.Range(1, 9);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (completed)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(this.gameObject);
        }

        if (failed)
        {
            // Set the random sequence order
            for (int i = 0; i < kNumRounds; i++)
            {
                RandomSequence[i] = Random.Range(1, 9);
            }

            userInputComplete = false;
            inputReceived = false;
            CurrRound = 0;
            timer = 0;

            NextRound();
            return;
        }

        // Auto selects the random button after kWaitTime
        if (shownSequence < CurrRound)
        {
            timer += Time.deltaTime;
            if (timer > kWaitTime)
            {
                PressButton(RandomSequence[shownSequence++]);
                timer = 0f;
            }
        }

        // The sequence for the round has been shown but user input not complete
        if (shownSequence == CurrRound && !userInputComplete)
        {
            // Button pressed
            if (inputReceived)
            {
                // Correct button pressed
                if (userInputNum == RandomSequence[userInputRound])
                {
                    inputReceived = false;
                    userInputRound++;
                    if (userInputRound == CurrRound)
                    {
                        userInputComplete = true;
                    }
                }
                // Incorrect button pressed
                else
                {
                    StartCoroutine(taskFailed());
                }
            }
        }
        // Advances to next round if the current sequence has been shown and user has inputted
        else if (shownSequence == CurrRound && userInputComplete)
        {
            NextRound();
        }

        // If all rounds completed, VICTORY
        if (CurrRound > kNumRounds)
        {
            ritual.GetComponent<Ritual>().completed = true;
            StartCoroutine(taskCompleted());
        }
    }

    /// <summary>
    /// Updates all variables appropriately for the next round
    /// </summary>
    void NextRound()
    {
        CurrRound++;
        shownSequence = 0;
        userInputRound = 0;
        userInputComplete = false;
    }

    /// <summary>
    /// Finds the button given and presses it, waits, and then unpresses
    /// </summary>
    /// <param name="buttonNum"></param>
    void PressButton(int buttonNum)
    {
        button = GameObject.Find(buttonNum.ToString()).GetComponent<Button>();
        OnButtonPress();
        StartCoroutine(waitForPress());
    }

    /// <summary>
    /// Used in PressButton() to allow the program to wait for 0.2 seconds between press and unpress
    /// </summary>
    IEnumerator waitForPress()
    {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        yield return new WaitForSeconds(0.2f);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
    }

    /// <summary>
    /// Called when the user pushes an incorrect button to allow the TaskFailed audio to play 0.5 seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator taskFailed()
    {
        failed = true;
        AudioSource.PlayClipAtPoint(TaskFailed, Vector3.zero, 1.0F);
        yield return new WaitForSeconds(0.5f);
        failed = false;
    }

    /// <summary>
    /// Called when the user completes the task to allow TaskCompleted audio to play 0.5 seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator taskCompleted()
    {
        AudioSource.PlayClipAtPoint(TaskCompleted, Vector3.zero, 1.0F);
        completed = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Receives the name of the button that the user presses and parses it to int
    /// </summary>
    public void UserSelectButton()
    {
        OnButtonPress();
        inputReceived = true;
        string name = EventSystem.current.currentSelectedGameObject.name;
        System.Int32.TryParse(name, out userInputNum);
    }

    /// <summary>
    /// Plays buttonPushedAudio when a button is pressed
    /// </summary>
    public void OnButtonPress()
    {
        Debug.Log("Play Press");
        AudioSource.PlayClipAtPoint(buttonPushedAudio, Camera.main.transform.position, 0.2F);
    }
    public void exitButton()
    {
        Destroy(this.gameObject);
    }
}
