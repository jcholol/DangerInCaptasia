using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptivatorAudioManager : MonoBehaviour
{
    [Header("Explorer Reference")]
    public Captivator captivatorRef;

    [Header("Audio Sources")]
    public AudioSource footstepOne;
    public AudioSource footstepTwo;
    public AudioSource HitSound;

    // Start is called before the first frame update
    void Start()
    {
        if (captivatorRef == null)
        {
            captivatorRef = this.transform.parent.GetComponent<Captivator>();
        }
    }
}
