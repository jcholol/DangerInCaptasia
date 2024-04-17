using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeSentence : RitualPanel
{
    public AudioClip TaskCompleted;
    public AudioClip TaskFailed;
    public InputField enterText;
    public Image backgroundImage;

    private string sentenceChosen;

    private bool completed = false;

    private static List<string> listOfSentences;
    // Start is called before the first frame update
    void Start()
    {
        listOfSentences = new List<string>();
        InstantiateSentences();
        ChooseSentence();
    }

    // Update is called once per frame
    void Update()
    {
        if (completed)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (enterText.text.Equals(GetChosenSentence()))
            {
                StartCoroutine(taskCompleted());
            }
            else
            {
                StartCoroutine(taskFailed());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(this.gameObject);
        }
    }
    /// <summary>
    /// This method instantiate and adds the sentences to the list of sentences.
    /// </summary>
    void InstantiateSentences()
    {
        string sentenceZero = "My name is Jacob, I like making chicken strips for lunch! If you want some chicken strips, contact me please!";
        AddSentencesToList(sentenceZero);
        string sentenceOne = "I was very proud of my nickname throughout high school but today- I couldn't be any different to what my nickname was.";
        AddSentencesToList(sentenceOne);
        string sentenceTwo = "Art doesn't have to be intentional. It may or may not sometimes be an accident.";
        AddSentencesToList(sentenceTwo);
        string sentenceThree = "He played the game as if his life depended on it and the truth was that it did.";
        AddSentencesToList(sentenceThree);
        string sentenceFour = "If you die in the game, you die in real life.";
        AddSentencesToList(sentenceFour); 
        string sentenceFive = "She tilted her head back and let whip cream stream into her mouth while taking a bath.";
        AddSentencesToList(sentenceFive); 
        string sentenceSix = "The three-year-old girl ran down the beach as the kite flew behind her.";
        AddSentencesToList(sentenceSix);
        string sentenceSeven = "The delicious aroma from the kitchen was ruined by cigarette smoke.";
        AddSentencesToList(sentenceSeven); 
        string sentenceEight = "The opportunity of a lifetime passed before him as he tried to decide between a cone or a cup.";
        AddSentencesToList(sentenceEight);
        string sentenceNine = "The tattered work gloves speak of the many hours of hard labor he endured throughout his life.";
        AddSentencesToList(sentenceNine); 
        string sentenceTen = "There was no ice cream in the freezer, nor did they have money to go to the store.";
        AddSentencesToList(sentenceTen);
    }

    /// <summary>
    /// Adds the sentences to the list of sentences
    /// </summary>
    /// <param name="sentence">string (sentence to input) </param>
    void AddSentencesToList(string sentence)
    {
        listOfSentences.Add(sentence);
    }

    /// <summary>
    /// Chooses a random sentence
    /// </summary>
    public string ChooseSentence()
    {
        int chooseRand = Random.Range(0, listOfSentences.Count);
        sentenceChosen = listOfSentences[chooseRand];
        return sentenceChosen;
    }

    /// <summary>
    /// getter for the chosen sentence
    /// </summary>
    /// <returns> the chosen sentence</returns>
    public string GetChosenSentence()
    {
        return sentenceChosen;
    }

    /// <summary>
    /// Called when the user completes the task to allow TaskCompleted audio to play 2 seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator taskCompleted()
    {
        completed = true;
        ritual.GetComponent<Ritual>().completed = true;
        AudioSource.PlayClipAtPoint(TaskCompleted, Vector3.zero, 1.0F);
        backgroundImage.color = Color.green;
        yield return new WaitForSeconds(1f); 
        Destroy(backgroundImage);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Called when the user pushes an incorrect button to allow the TaskFailed audio to play 0.2 seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator taskFailed()
    {
        AudioSource.PlayClipAtPoint(TaskFailed, Vector3.zero, 1.0F);
        backgroundImage.color = Color.red;
        enterText.text = "";
        ChooseSentence();
        yield return new WaitForSeconds(0.5f);
        backgroundImage.color = Color.white;
    }
    public void exitButton()
    {
        Destroy(this.gameObject);
    }
}
