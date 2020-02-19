using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : CharacterAbstraction
{
    public MonsterBase moster;
    public bool canMove_ = true;

    private Vector3 previousVelocity_;
    private PlayerAnimation playerAnimation_;
    private CaptureSystem captureSystem;


    [Header("Jump Stats")]
    public float jumpforce;
    public bool jump;


    private void Start()
    {
        #region Get Components
        body_ = GetComponent<Rigidbody>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        playerAnimation_ = GetComponent<PlayerAnimation>();
        captureSystem = GetComponent<CaptureSystem>();
        #endregion

        body_.constraints = RigidbodyConstraints.FreezePositionX;
        body_.constraints = RigidbodyConstraints.FreezePositionZ;
        body_.constraints = RigidbodyConstraints.FreezeRotation;

        SetInitialCharacter();
    }

    private void FixedUpdate()
    {
        if (canMove_)
        {
            if (isEnabled && !captureSystem.capturingProcess)
            {
                if (!jump)
                {
                    axisX = input_.GetAxisHorizontal();
                    axisY = input_.GetAxisVertical();
                }

                Movement();
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
            {
                if (!captureSystem.capturingProcess && !captureSystem.capturing) playerAnimation_.AnimationSet_ENTERJUMP();
            }
             
            if (jump)  body_.AddForce(-Vector3.up * 1500 * Time.deltaTime);
        }

        playerAnimation_.MovimentSpeed(PlayerVelocity());
    }

    protected override void Movement() 
    {
        Vector2 input = new Vector2(axisX, axisY);

        if (input != Vector2.zero)
        {
            float targetrotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetrotation + Camera.main.transform.eulerAngles.y, 
                ref smooth_, 
                smoothTime);

            if (!captureSystem.capturing)
            {
                if (!input_.RunInput())
                    transform.position += transform.forward * walkSpeed * Time.deltaTime;
                else
                    transform.position += transform.forward * runSpeed * Time.deltaTime;
            }
            else
            {
              transform.position += transform.forward * walkSpeed/3f * Time.deltaTime;
            }
        }
    }
    
    private float PlayerVelocity()
    {
        Vector3 speed_ = (transform.position - previousVelocity_) / Time.deltaTime;
        previousVelocity_ = transform.position;
        return speed_.magnitude;
    }

    private void Jump()
    {
        jump = true;
        body_.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
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
                playerAnimation_.ResetAll();
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
        captureSystem.GoToWalkAnimator();
        yield break;
    }
}
