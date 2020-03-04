using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationBase : MonoBehaviour
{
    public abstract AnimatorStateInfo GetCurrentAnimationInLayerThree();   
    public abstract AnimatorStateInfo GetCurrentAnimationInLayerOne();
    public abstract void AnimationSpeed(float xAxis, float yAxis);
    public abstract void NoMovableAttack(int attackindex);
    public abstract void MovableAttack(int attackindex);
    public abstract void ActiveHit();
    public abstract void ExtraAnimationOne();
    public abstract void ExtraAnimationTwo();
    public abstract void EnterJump();
    public abstract void ExitJump();
    public abstract void EnterInSwimMode();
    public abstract void ExitInSwimMode();
    public abstract void ExitDeathState();
    public abstract void PlayDeathState();
    public abstract bool IsPlayAttackAnimation();
}
