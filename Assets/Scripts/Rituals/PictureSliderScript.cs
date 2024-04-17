using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureSliderScript : RitualPanel
{
    public AudioClip SlideSound;
    public AudioClip TaskCompleted;

    public List<int> RandomOrder = new List<int>(kNumButtons);
    GameObject[] buttons = new GameObject[kNumButtons];
    public Vector3[] correctPos = new Vector3[kNumButtons];

    const int kNumButtons = 9;
    bool completed;

    public GameObject buttonOne = null;
    public GameObject buttonTwo = null;

    // Start is called before the first frame update
    void Start()
    {
        RandomizeBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (completed)
            return;

        if (WinCondition())
        {
            StartCoroutine(taskCompleted());
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Randomizes the game board
    /// </summary>
    public void RandomizeBoard()
    {
        for (int i = 0; i < kNumButtons; i++)
        {
            RandomOrder.Add(i);
        }
        for (int i = 0; i < kNumButtons; i++)
        {
            int randomIndex = Random.Range(0, RandomOrder.Count);
            buttons[i] = GameObject.Find((RandomOrder[randomIndex]) + "");
            RandomOrder.RemoveAt(randomIndex);
        }
        for (int i = 0; i < kNumButtons; i++)
        {
            buttons[i].transform.localPosition = correctPos[i];
        }
    }

    /// <summary>
    /// Checks for win condition
    /// </summary>
    /// <returns>True if game won</returns>
    bool WinCondition()
    {
        for (int i = 0; i < kNumButtons; i++)
        {
            if (buttons[i].name != (i + ""))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Called when the user clicks on a button and passes in the button clicked.
    /// Swaps the button clicked with the empty spot on the board as long as its adjacent.
    /// </summary>
    /// <param name="button">ButtonClicked</param>
    public void SwapTile(GameObject buttonOne, GameObject buttonTwo)
    {

        int buttonOneIndex = 0;
        int buttonTwoIndex = 0;

        for (int i = 0; i < kNumButtons; i++)
        {
            if (buttonOne.name == buttons[i].name)
            {
                buttonOneIndex = i;
            }
            if (buttonTwo.name == buttons[i].name)
            {
                buttonTwoIndex = i;
            }
        }

        Vector3 buttonOneLocation = buttonOne.transform.localPosition;
        Vector3 buttonTwoLocation = buttonTwo.transform.localPosition;

        AudioSource.PlayClipAtPoint(SlideSound, Camera.main.transform.position, 0.1f);
        buttonOne.transform.localPosition = buttonTwoLocation;
        buttonTwo.transform.localPosition = buttonOneLocation;

        buttons[buttonOneIndex] = buttonTwo;
        buttons[buttonTwoIndex] = buttonOne;
    }

    public void buttonClicked(GameObject button)
    {
        if (buttonOne == null)
        {
            buttonOne = button;
            TurnGray(buttonOne);
        }
        else if (buttonTwo == null)
        {
            buttonTwo = button;
            TurnGray(buttonTwo);
        }

        if (buttonOne != null && buttonTwo != null)
        {
            SwapTile(buttonOne, buttonTwo);
            TurnWhite(buttonOne);
            TurnWhite(buttonTwo);
            buttonOne = null;
            buttonTwo = null;
        }
    }

    public void TurnWhite(GameObject button)
    {
        button.GetComponent<Image>().color = Color.white;
    }

    public void TurnGray(GameObject button)
    {
        button.GetComponent<Image>().color = Color.gray;
    }


    /// <summary>
    /// Turns the gameboard green and plays completed audio
    /// </summary>
    /// <returns></returns>
    IEnumerator taskCompleted()
    {
        AudioSource.PlayClipAtPoint(TaskCompleted, Camera.main.transform.position, 0.75F);
        for (int i = 0; i < kNumButtons; i++)
        {
            buttons[i].GetComponent<Image>().color = Color.green;
        }
        completed = true;
        ritual.GetComponent<Ritual>().completed = true;
        yield return new WaitForSeconds(1.75f);
        Destroy(this.gameObject);
    }

    public void exitButton()
    {
        Destroy(this.gameObject);
    }
}
