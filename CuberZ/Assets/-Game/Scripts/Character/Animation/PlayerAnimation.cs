using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator playerAnimator_;
    
    #region Singleton

    public static  PlayerAnimation instance { get { return instance_; } }
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

    public void SetSubstateAndAnimation(string substate, string animation)
    {
        playerAnimator_.ResetTrigger("startjump");
        playerAnimator_.ResetTrigger("idlejump");
        playerAnimator_.ResetTrigger("falljump");
        playerAnimator_.ResetTrigger("throwcubenear");
        playerAnimator_.ResetTrigger("throwcubefar");
        playerAnimator_.ResetTrigger("bringbackmonster");
        playerAnimator_.ResetTrigger("swin");
        playerAnimator_.ResetTrigger("jump");
        playerAnimator_.ResetTrigger("cube");
        playerAnimator_.ResetTrigger("player");

        playerAnimator_.SetTrigger(substate);
        playerAnimator_.SetTrigger(animation);

    }

    public void SetAnimationOnly(string animation)
    {
        playerAnimator_.ResetTrigger("startjump");
        playerAnimator_.ResetTrigger("idlejump");
        playerAnimator_.ResetTrigger("falljump");
        playerAnimator_.ResetTrigger("throwcubenear");
        playerAnimator_.ResetTrigger("throwcubefar");
        playerAnimator_.ResetTrigger("bringbackmonster");
        playerAnimator_.ResetTrigger("swin");
        playerAnimator_.ResetTrigger("jump");
        playerAnimator_.ResetTrigger("cube");
        playerAnimator_.ResetTrigger("player");

        playerAnimator_.SetTrigger(animation);

    }


}
