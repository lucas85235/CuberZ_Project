using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : CharacterAbstraction
{
    public MonsterBase moster;
    public bool canMove_ = true;

    [Header("Jump Stats")]
    public float jumpforce;
    public bool jump;

    private Vector3 previousVelocity_;

    private void Start()
    {
        #region Get Components
        bory_ = GetComponent<Rigidbody>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        #endregion

        bory_.constraints = RigidbodyConstraints.FreezePositionX;
        bory_.constraints = RigidbodyConstraints.FreezePositionZ;
        bory_.constraints = RigidbodyConstraints.FreezeRotation;

        SetInitialCharacter();
    }

    private void Update()
    {
        if (canMove_)
        {
            if (isEnabled && !CaptureSystem.instance.capturingProcess_)
            {
                if (!jump)
                {
                    axisX = input_.GetAxisHorizontal();
                    axisY = input_.GetAxisVertical();
                }

                Movement();
                //  animation_.AnimationSpeed(axisX, axisY);

                #region Get Inputs
                if (Input.GetKeyDown(KeyCode.T))
                {
                    if (moster != null)
                    {
                        SwitchCharacterController(moster);
                        StartCoroutine(moster.StopFollow());
                    }
                }
                #endregion
            }

            if (input_.JumpInput() && isEnabled)
                if (!CaptureSystem.instance.capturingProcess_ && !CaptureSystem.instance.capturing_)
                    PlayerAnimation.instance.SetAnimatorAndAnimation(2, "startjump");
             
            if (jump)
                bory_.AddForce(-Vector3.up * 1500 * Time.deltaTime);
        }

        PlayerAnimation.instance.SpeedBlendTree(PlayerVelocity());
    }

    private float PlayerVelocity()
    {
        Vector3 speed_ = (transform.position - previousVelocity_) / Time.deltaTime;
        previousVelocity_ = transform.position;
        Debug.Log(speed_.magnitude);
        return speed_.magnitude;

    }

    private void Jump()
    {
        jump = true;
        bory_.AddForce(Vector3.up * jumpforce,ForceMode.Impulse);
    }

    private void SetInitialCharacter()
    {
        CharacterAbstraction thisCharacter = GetComponent<CharacterAbstraction>();
        CharacterAbstraction[] allCharacters = FindObjectsOfType<CharacterAbstraction>();

        foreach (CharacterAbstraction currentCharacter in allCharacters)
        {
            if (currentCharacter != thisCharacter)
                currentCharacter.isEnabled = false;
        }

        thisCharacter.isEnabled = true;
        SetCameraPropeties(transform.Find("CameraTarget"));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground" || collision.gameObject.name == "Wall")
        {
            if (jump)
            {
                PlayerAnimation.instance.SetAnimatorAndAnimation(2, "falljump");
                StartCoroutine(WaitJumpTime());
            }
        }
    }

    private IEnumerator WaitJumpTime()
    {
        jump = false;
        canMove_ = false;
        yield return new WaitForSeconds(0.3f);
        canMove_ = true;
        yield break;
    }
}
