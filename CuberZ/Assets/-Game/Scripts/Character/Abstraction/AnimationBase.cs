using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AnimationBase
{
    AnimatorStateInfo GetCurrentAnimationInLayerOne();
    void AnimationSpeed(float xAxis, float yAxis);
    void MovementSpeed(float speed);
    void NoMovableAttack(int attackindex);
    void MovableAttack(int attackindex);
    void ActiveHit();
    void ExtraAnimationOne();
    void ExtraAnimationTwo();
    void EnterJump();
    void ExitJump();
    void EnterInSwimMode();
    void ExitInSwimMode();
    void ExitDeathState();
    void PlayDeathState();
    bool IsPlayAttackAnimation();
}
