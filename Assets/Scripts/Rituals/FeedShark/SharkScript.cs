using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkScript : MonoBehaviour
{
    public GameObject upperBound;
    public GameObject lowerBound;
    public GameObject manager;
    bool moveUp = true;
    bool win = false;
    float speed = 0.5f;

    public int hunger = 1;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(manager != null);
    }

    // Update is called once per frame
    void Update()
    {
        moveUpAndDown();
        if(win)
        {
            enabled = false;
        }
        
    }

    public void moveUpAndDown()
    {
        Vector3 pos = transform.localPosition;
        if(moveUp)
        {
            pos = Vector3.Lerp(pos, upperBound.transform.localPosition, speed * Time.deltaTime);
        }
        else
        {
            pos = Vector3.Lerp(pos, lowerBound.transform.localPosition, speed * Time.deltaTime);
        }
        transform.localPosition = pos;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bound")
        {
            moveUp = !moveUp;
        }
        if(collision.gameObject.tag == "Fish")
        {
            hunger -= 1;
            Debug.Log(hunger);
            GameObject manage = this.transform.parent.gameObject;
            FeedSharkScript other = manage.GetComponent<FeedSharkScript>();
            other.IncrementHit();
            if (hunger <= 0)
            {
                transform.GetComponent<PolygonCollider2D>().enabled = false;
                other.gameOver();
                win = true;
            }
            
        }
    }
}
