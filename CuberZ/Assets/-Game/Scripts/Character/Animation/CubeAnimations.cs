using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimations : MonoBehaviour
{
    private Transform captureCube_, fakeCube_;

    private void Start()
    {
        captureCube_ = transform.GetChild(0);
        fakeCube_ = transform.GetChild(1);
    }

    public void Capturing()
    {
        fakeCube_.GetComponent<Animator>().Play("Capturing", -1, 0);
    }

    public void DecreaseCaptureCube()
    {
        captureCube_.GetComponent<Animator>().Play("DiminuirCuboPequeno", -1, 0);
    }

    public void DissolveCube()
    {
        fakeCube_.GetComponent<Animator>().Play("DissolveCubo", -1, 0);
    }

    public void ExpandFakeCube()
    {
        fakeCube_.GetComponent<Animator>().Play("ExpandirCubo", -1, 0);
    }

    public void ShakeCube()
    {
        fakeCube_.GetComponent<Animator>().Play("ShakeCube", -1, 0);
    }
}
