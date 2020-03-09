using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureHelper : MonoBehaviour
{

    #region Chamado por eventos na animação, do filho do prefab Cube
    public void DisableThisObject()
    {
        transform.gameObject.SetActive(false);
    }

    public void CaptureAnimationFinished()
    {
        FindObjectOfType<CaptureSystemNew>().finishCubeCatchAnimator = true;
    }

    #endregion
}
