using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointer : MonoBehaviour
{
    private Explorer explorerRef;

    [Header("Map Pointer Duration")]
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        explorerRef = this.transform.parent.GetComponent<Explorer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (duration <= 0)
        {
            Destroy(this.gameObject);
        }

        if (explorerRef.performingRitual)
        {
            Destroy(this.gameObject);
        }

        duration -= Time.deltaTime;

        pointToNearestInCompleteRitual();
    }

    private void pointToNearestInCompleteRitual()
    {
        GameObject[] ritualObjectList = GameObject.FindGameObjectsWithTag("Ritual");

        Ritual closestInCompleteRitual = null;

        foreach(GameObject ritualObject in ritualObjectList)
        {
            Ritual ritual = ritualObject.GetComponent<Ritual>();

            if (!ritual.completed)
            {
                if (closestInCompleteRitual == null)
                {
                    closestInCompleteRitual = ritual;
                } else
                {
                    if (Vector2.Distance(this.transform.position, closestInCompleteRitual.transform.position) >
                        Vector2.Distance(this.transform.position, ritual.transform.position))
                    {
                        closestInCompleteRitual = ritual;
                    }
                }
            }
        }

        Debug.Log(closestInCompleteRitual.gameObject);

        if (closestInCompleteRitual != null)
        {
            lookAtRitual(closestInCompleteRitual.gameObject);
        }
    }

    private void lookAtRitual(GameObject ritual)
    {
        Vector3 position = ritual.transform.position;

        Vector3 dir = position - this.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
