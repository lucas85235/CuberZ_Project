using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureHelper : MonoBehaviour
{
    [HideInInspector]
    public bool canGo;

    private void OnEnable()
    {
        CantGo();
    }

    public void DisableThisObject()
    {
        transform.gameObject.SetActive(false);
    }

    public void CanGo()
    {
        canGo = true;
    }

    public void CantGo()
    {
        canGo = false;
    } 
}
