using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public RuntimeAnimatorController[] playerAllAnimators;
    private Animator playerAnimator_;

    #region Singleton

    public static PlayerAnimation instance { get { return instance_; } }
    private static PlayerAnimation instance_;

    private void Awake()
    {
        instance_ = this;
        playerAnimator_ = GetComponent<Animator>();
    }
    #endregion



    public void SpeedBlendTree(float speed)
    {
        playerAnimator_.SetFloat("Speed", speed);
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
                playerAnimator_.runtimeAnimatorController = playerAllAnimators[animatornumber];
                break;

            case 1:
                playerAnimator_.runtimeAnimatorController = playerAllAnimators[animatornumber];
                playerAnimator_.ResetTrigger("throwfar");
                playerAnimator_.ResetTrigger("thrownear");
                playerAnimator_.ResetTrigger("bringback");
                playerAnimator_.SetTrigger(animation);
                break;

            case 2:
                playerAnimator_.runtimeAnimatorController = playerAllAnimators[animatornumber];
                playerAnimator_.ResetTrigger("startjump");
                playerAnimator_.ResetTrigger("idlejump");
                playerAnimator_.ResetTrigger("falljump");
                playerAnimator_.SetTrigger(animation);
                break;

            case 3:
                //depois faz  o do swin

                break;


        }




    }


}
