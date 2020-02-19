using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator_;

    private void Awake()
    {
        animator_ = GetComponent<Animator>();
    }

    public void MovimentSpeed(float speed)
    {
        animator_.SetFloat("SPEED", speed);
    }

    public void AnimationSet_SWIN()
    {
        animator_.ResetTrigger("ENTER-JUMP");
        animator_.ResetTrigger("CALL-MONSTER");
        animator_.ResetTrigger("THROW");
        animator_.ResetTrigger("THROW-NEAR");
        animator_.SetTrigger("SWIM");
    }

    public void AnimationSet_ENTERJUMP()
    {
        animator_.ResetTrigger("SWIM");
        animator_.ResetTrigger("CALL-MONSTER");
        animator_.ResetTrigger("THROW");
        animator_.ResetTrigger("THROW-NEAR");
        animator_.SetTrigger("ENTER-JUMP");
    }

    public void AnimationSet_CALLMONSTER()
    {
        animator_.ResetTrigger("SWIM");
        animator_.ResetTrigger("ENTER-JUMP");
        animator_.ResetTrigger("THROW");
        animator_.ResetTrigger("THROW-NEAR");
        animator_.SetTrigger("CALL-MONSTER");
    }

    public void AnimationSet_THROWCUBE()
    {
        animator_.ResetTrigger("SWIM");
        animator_.ResetTrigger("ENTER-JUMP");
        animator_.ResetTrigger("CALL-MONSTER");
        animator_.ResetTrigger("THROW-NEAR");
        animator_.SetTrigger("THROW");
    }

    public void AnimationSet_THROWCUBENEAR()
    {
        animator_.ResetTrigger("SWIM");
        animator_.ResetTrigger("ENTER-JUMP");
        animator_.ResetTrigger("CALL-MONSTER");
        animator_.ResetTrigger("THROW");
        animator_.ResetTrigger("THROW-NEAR");
    }


    public void ResetAll()
    {
        animator_.ResetTrigger("SWIM");
        animator_.ResetTrigger("ENTER-JUMP");
        animator_.ResetTrigger("CALL-MONSTER");
        animator_.ResetTrigger("THROW");
        animator_.ResetTrigger("THROW-NEAR");
    }
}
