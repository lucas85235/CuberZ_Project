using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerEventCallAnimation))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator_;

    private void Awake()
    {
        animator_ = GetComponent<Animator>();
    }

    public void SpeedBlendTree(float speed)
    {
        animator_.SetFloat("SPEED", speed);
    }

    public void SetAnimation(string animation)
    {
        animator_.ResetTrigger("SWIM");
        animator_.ResetTrigger("ENTER-JUMP");
        animator_.ResetTrigger("CALL-MONSTER");
        animator_.ResetTrigger("THROW");
        animator_.ResetTrigger("THROW-NEAR");
        animator_.SetTrigger(animation);
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
