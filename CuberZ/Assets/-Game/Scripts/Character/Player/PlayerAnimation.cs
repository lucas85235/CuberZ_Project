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

    public void EnterInSwimMode()
    {
        animator_.SetBool("SWIM", true);
    }

    public void ExitInSwimMode()
    {
        animator_.SetBool("SWIM", false);
    }

    public void EnterJump()
    {
        animator_.SetBool("ENTER-JUMP", true);
    }

    public void ExitJump()
    {
        animator_.SetBool("ENTER-JUMP", false);
    }

    public void CallMoster()
    {
        animator_.SetTrigger("CALL-MONSTER");
    }

    public void ThrowCube()
    {
        animator_.SetTrigger("THROW");
    }

    public void ThrowCubeNear()
    {
        animator_.SetTrigger("THROW-NEAR");
    }

    public void ResetAll()
    {
        animator_.SetBool("SWIM", false);
        animator_.SetBool("ENTER-JUMP", false);
        animator_.ResetTrigger("CALL-MONSTER");
        animator_.ResetTrigger("THROW");
        animator_.ResetTrigger("THROW-NEAR");
    }
}
