using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FishScript : MonoBehaviour
{
    public float speed = 4f;
    public GameObject Scuba;
    public GameObject Bound;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(Bound != null);
        Debug.Assert(Scuba != null);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        speed += (speed * Time.smoothDeltaTime);
        if(transform.localPosition.x - Scuba.transform.localPosition.x > Bound.transform.localPosition.x - Scuba.transform.localPosition.x)
        //if (((transform.localPosition - Scuba.transform.localPosition).magnitude) > ((Bound.transform.localPosition - Scuba.transform.localPosition).magnitude))
        {
            Destroy(transform.gameObject);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Bound")
        {
            Destroy(transform.gameObject);
        }
        if (collision.gameObject.tag == "Shark")
        {
            Destroy(transform.gameObject);
        }
    }
}
