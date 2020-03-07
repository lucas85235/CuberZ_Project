using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapdoramaAnimation : AnimationDefaultImpl
{
    public override void NoMovableAttack(int attackindex)
    {
        animator_.SetBool("OVERRIDE", false);
        animator_.SetBool("CAN-MOVE", false);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }

    public override void MovableAttack(int attackindex)
    {
        animator_.SetBool("OVERRIDE", false);
        animator_.SetBool("CAN-MOVE", true);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }

    public void OverrideMovableAttack(int attackindex)
    {
        animator_.SetBool("OVERRIDE", true);
        animator_.SetBool("CAN-MOVE", true);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", attackindex);
    }
}
