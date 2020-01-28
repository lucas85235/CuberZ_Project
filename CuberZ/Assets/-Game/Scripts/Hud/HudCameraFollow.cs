using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudCameraFollow : MonoBehaviour
{
   
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
