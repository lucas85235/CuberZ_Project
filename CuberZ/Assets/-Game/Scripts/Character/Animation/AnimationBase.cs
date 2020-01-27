using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationBase : MonoBehaviour
{
    public abstract void AnimationSpeed(float xAxis, float yAxis);
    public abstract void MovableAttack(int attackindex);
    public abstract void NoMovableAttack(int attackindex);
    public abstract void ActiveHit();
    public abstract AnimatorStateInfo GetCurrentAnimation();
    public abstract bool IsPlayAttackAnimation();
}
