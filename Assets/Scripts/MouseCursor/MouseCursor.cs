using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public Vector3 mouseWorldPosition;
    void Start()
    {
        Cursor.visible = false;
        DontDestroyOnLoad(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        moveMouse();
    }

    void moveMouse()
    {
        this.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
        //this.transform.position = Vector3.Lerp(this.transform.position, cursorPos, 100f);
    }
}
