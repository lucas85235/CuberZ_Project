using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDefaultImpl : MonoBehaviour, AnimationBase
{
    protected Animator animator_;
    protected EyeAnimation eyes_;

    public const string animationSpeed = "SPEED";

    protected virtual void Awake() 
    {
        animator_ = GetComponent<Animator>();  
        eyes_ = transform.GetChild(1).GetComponent<EyeAnimation>(); 
    }

    public virtual AnimatorStateInfo GetCurrentAnimationInLayerOne() 
    {
        return animator_.GetCurrentAnimatorStateInfo(0);
    }

    public virtual void AnimationSpeed(float xAxis, float yAxis)
    {
        animator_.SetFloat(animationSpeed, xAxis * xAxis + yAxis * yAxis);
    }

    public virtual void MovementSpeed(float speed) => animator_.SetFloat(animationSpeed, speed);

    public virtual void NoMovableAttack(int attackindex) 
    {
        animator_.SetBool("CAN-MOVE", false);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }

    public virtual void MovableAttack(int attackindex) 
    {
        animator_.SetBool("CAN-MOVE", true);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }

    public virtual void ActiveHit() 
    {
        animator_.SetTrigger("HIT");
    }

    public virtual void ExtraAnimationOne() 
    {
        animator_.SetTrigger("EXTRA-1");
    }

    public virtual void ExtraAnimationTwo() 
    {
        animator_.SetTrigger("EXTRA-2");
    }

    public virtual void EnterJump() 
    {
        animator_.SetBool("ENTER-JUMP", true);
    }
    
    public virtual void ExitJump() 
    {
        animator_.SetBool("ENTER-JUMP", false);
    }

    public virtual void EnterInSwimMode() 
    {
        animator_.SetBool("SWIM", true);
    }
    
    public virtual void ExitInSwimMode() 
    {
        animator_.SetBool("SWIM", false);
    }

    public virtual void ExitDeathState() 
    {
        animator_.SetBool("DEAD", false);
        eyes_.OpenEyes();
    }

    public virtual void PlayDeathState() 
    {
        animator_.SetBool("DEAD", true);
        eyes_.CloseEyes();
        eyes_.enabled = false;
    }

    public virtual bool IsPlayAttackAnimation()
    {
        if (!animator_.GetCurrentAnimatorStateInfo(0).IsName("Blend Tree") &&  
            !animator_.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
            !animator_.GetCurrentAnimatorStateInfo(0).IsName("EnterJump") &&
            !animator_.GetCurrentAnimatorStateInfo(0).IsName("IdleJump") &&
            !animator_.GetCurrentAnimatorStateInfo(0).IsName("ExitJump") &&
            !animator_.GetCurrentAnimatorStateInfo(0).IsName("ExitWater"))
            return true;
        else
            return false;
    }
}
