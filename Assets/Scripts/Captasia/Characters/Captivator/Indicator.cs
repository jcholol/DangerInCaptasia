using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject pointToObject;

    // Update is called once per frame
    void Update()
    {
        if (pointToObject == null)
        {
            return;
        }

        lookAtObject(pointToObject);
    }

    private void lookAtObject(GameObject obj)
    {
        Vector3 position = obj.transform.position;

        Vector3 dir = position - this.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
