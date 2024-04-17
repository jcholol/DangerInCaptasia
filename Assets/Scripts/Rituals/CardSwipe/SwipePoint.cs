using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipePoint : MonoBehaviour
{

    private SwipeTask swipetask;
    // Start is called before the first frame update

    private void Awake()
    {
        swipetask = GetComponentInParent<SwipeTask>();   
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Card")
        {
            swipetask.SwipePointTrigger(this);
        }
    }
}
