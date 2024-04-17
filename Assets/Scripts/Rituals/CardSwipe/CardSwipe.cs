using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSwipe : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Canvas canvas;

    public Vector3 initialPos;
    //Necessary to get location of card in canvas
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        initialPos = this.transform.localPosition;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out pos);
        pos.y = Mathf.Clamp(pos.y, initialPos.y-250f, initialPos.y + 150f);
        //Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //point.x = Mathf.Clamp(point.x, -50f, 50f);
        //point.y = Mathf.Clamp(point.y, -50f, 2.25f);
        //point.z = Mathf.Clamp(point.z, 0f, 0f);
        //transform.position = point;
        transform.position = canvas.transform.TransformPoint(pos);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.localPosition = initialPos;
    }


}
