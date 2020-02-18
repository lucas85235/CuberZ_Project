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

    public override AnimatorStateInfo GetCurrentAnimationInLayerOne() 
    {
        return animator_.GetCurrentAnimatorStateInfo(0);
    }

    public override AnimatorStateInfo GetCurrentAnimationInLayerThree() 
    {
        return animator_.GetCurrentAnimatorStateInfo(2);
    }

    public override void AnimationSpeed(float xAxis, float yAxis) 
    {
        animator_.SetFloat(animationSpeed, (xAxis * xAxis + yAxis * yAxis) / 2);
    }

    public override void NoMovableAttack(int attackindex) 
    {
        animator_.SetBool("CAN-MOVE", false);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }

    public override void MovableAttack(int attackindex) 
    {
        animator_.SetBool("CAN-MOVE", true);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }

    public override void ActiveHit() 
    {
        animator_.SetTrigger("HIT");
    }

    public override void ExtraAnimationOne() 
    {
        animator_.SetTrigger("EXTRA-1");
    }

    public override void ExtraAnimationTwo() 
    {
        animator_.SetTrigger("EXTRA-2");
    }

    public override void EnterJump() 
    {
        GetComponent<Animator>().SetBool("ENTER-JUMP", true);
    }
    
    public override void ExitJump() 
    {
        GetComponent<Animator>().SetBool("ENTER-JUMP", false);
    }

    public override void EnterInSwimMode() 
    {
        GetComponent<Animator>().SetBool("SWIM", true);
    }
    
    public override void ExitInSwimMode() 
    {
        GetComponent<Animator>().SetBool("SWIM", false);
    }

    public override void ExitDeathState() 
    {
        GetComponent<Animator>().SetBool("DEAD", false);
        GetComponent<Animator>().SetBool("PLAY-DEAD", false);  
    }

    public override IEnumerator PlayDeathState() 
    {
        GetComponent<Animator>().SetBool("DEAD", true);
        yield return new WaitForSeconds(0.1f);
        GetComponent<Animator>().SetBool("PLAY-DEAD", true);    }

    public override bool IsPlayAttackAnimation()
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
