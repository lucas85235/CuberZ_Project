using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimations : MonoBehaviour
{
    private Transform smallCube_, bigCube_;


    private void Start()
    {
        smallCube_ = transform.GetChild(0);
        bigCube_ = transform.GetChild(1);
    }



    public void AnimationSet_Capturing()
    {
        bigCube_.GetComponent<Animator>().Play("Capturing", -1, 0);

    }

    public void AnimationSet_DiminuirSmallCube()
    {
        smallCube_.GetComponent<Animator>().Play("DiminuirCuboPequeno", -1, 0);
    }

    public void AnimationSet_DissolveCube()
    {
        bigCube_.GetComponent<Animator>().Play("DissolveCubo", -1, 0);
    }

    public void AnimationSet_ExpandirBigCube()
    {
        bigCube_.GetComponent<Animator>().Play("ExpandirCubo", -1, 0);
    }

    public void AnimationSet_ShakeCube()
    {
        bigCube_.GetComponent<Animator>().Play("ShakeCube", -1, 0);
    }

}
