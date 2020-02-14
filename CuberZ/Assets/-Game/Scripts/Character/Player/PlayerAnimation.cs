using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public RuntimeAnimatorController[] allAnimators;
    private Animator animator_;

    private void Awake()
    {
        animator_ = GetComponent<Animator>();
    }

    public void SpeedBlendTree(float speed)
    {
        animator_.SetFloat("Speed", speed);
    }

    public void GoToWalkAnimator()
    {
        CaptureSystem.instance.throwbool = false;
        SetAnimatorAndAnimation(0);
    }

    public void SetAnimatorAndAnimation(int animatornumber, string animation = null)
    {
        /// <summary>
        /// animatornumber = 0 -> Walk
        /// animatornumber = 1 -> Capture
        /// animatornumber = 2 -> Jump
        /// animatornumber = 3 -> Swin
        /// </summary>

        switch (animatornumber)
        {
            case 0:
                animator_.runtimeAnimatorController = allAnimators[animatornumber];
                break;

            case 1:
                animator_.runtimeAnimatorController = allAnimators[animatornumber];
                animator_.ResetTrigger("throwfar");
                animator_.ResetTrigger("thrownear");
                animator_.ResetTrigger("bringback");
                animator_.SetTrigger(animation);
                break;

            case 2:
                animator_.runtimeAnimatorController = allAnimators[animatornumber];
                animator_.ResetTrigger("startjump");
                animator_.ResetTrigger("idlejump");
                animator_.ResetTrigger("falljump");
                animator_.SetTrigger(animation);
                break;

            case 3:
                //depois faz  o do swin
                break;
        }
    }
}
