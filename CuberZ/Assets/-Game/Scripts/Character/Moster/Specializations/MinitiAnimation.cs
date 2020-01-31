using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinitiAnimation : AnimationBase
{
    private Animator animator_;
    private IInput input_;

    protected const string animationSpeed = "SPEED";

    private void Awake() 
    {
        input_ = Object.FindObjectOfType<InputSystem>();
        animator_ = GetComponent<Animator>();    
    }

    public override void AnimationSpeed(float xAxis, float yAxis) 
    {
        if (input_.RunInput())
            animator_.SetFloat(animationSpeed, xAxis * xAxis + yAxis * yAxis);
        else 
            animator_.SetFloat(animationSpeed, (xAxis * xAxis + yAxis * yAxis) / 2);
    }

    public override void MovableAttack(int attackindex) 
    {
        animator_.SetBool("CAN-MOVE", true);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }

    public override void NoMovableAttack(int attackindex) 
    {
        animator_.SetBool("CAN-MOVE", false);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }

    public override void ActiveHit() 
    {
        animator_.SetTrigger("HIT");
    }

    public override AnimatorStateInfo GetCurrentAnimationInLayerOne() 
    {
        return animator_.GetCurrentAnimatorStateInfo(0);
    }

    public override bool IsPlayAttackAnimation()
    {
        if (!animator_.GetCurrentAnimatorStateInfo(0).IsName("Blend Tree") &&  
            !animator_.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return true;
        else
            return false;
    }
}
