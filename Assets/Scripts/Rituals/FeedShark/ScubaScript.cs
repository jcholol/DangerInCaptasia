using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScubaScript : MonoBehaviour
{
    public AudioClip buttonClick;
    public float cooldown = 0.5f;
    public GameObject fishPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnFish()
    {
        Debug.Log(transform.position.x);
        GameObject fish = Instantiate(fishPrefab, transform.position, transform.rotation, this.transform);
        fish.transform.position = new Vector3(transform.position.x,transform.position.y,0);
    }
    public void ButtonClick()
    {
        AudioSource.PlayClipAtPoint(buttonClick, Vector3.zero);
        SpawnFish();
        
    }
}
