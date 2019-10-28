using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer : MonoBehaviour
{

    void Start()
    {
        
    }
    

    void LateUpdate()
    {
        var mousePos = Input.mousePosition;
        mousePos.x -= Screen.width / 2;
        mousePos.y -= Screen.height / 2;
        transform.LookAt(new Vector3(mousePos.x, 0, mousePos.y));
    }
}
