using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeSentencesUI : MonoBehaviour
{
    public TypeSentence TypeSentenceRef;
    public Text sentence;

    private string sentenceShown;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        sentenceShown = TypeSentenceRef.GetChosenSentence();
        sentence.text = sentenceShown;
    }
}
