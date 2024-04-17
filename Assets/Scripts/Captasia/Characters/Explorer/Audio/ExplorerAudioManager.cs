using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorerAudioManager : MonoBehaviour
{
    [Header("Explorer Reference")]
    public Explorer explorerRef;

    [Header("Audio Sources")]
    public AudioSource footstepOne;
    public AudioSource footstepTwo;
    public AudioSource footstepThree;

    // Heart Beat
    public AudioSource heartBeat;

    // Got hit sounds
    public AudioSource FemaleScream;
    public AudioSource MaleGrunt;
    public AudioSource HitSound;

    // Start is called before the first frame update
    void Start()
    {
        if (explorerRef == null)
        {
            explorerRef = this.transform.parent.GetComponent<Explorer>();
        }
    }
}
