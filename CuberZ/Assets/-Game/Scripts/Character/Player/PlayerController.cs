using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : CharacterAbstraction
{
    private PlayerAnimation playerAnimation_;
    private CaptureSystem captureSystem;
    private HudWorldStats worldHud_;
    private CameraProperties camera_;

    private bool canGetInputs_ = false;
    private bool canMove_ = true;

    [Header("Kubber Team")]
    public MonsterBase currentKubberSpawned;
    public GameObject[] monster = { null, null, null, null };

    public bool CanMove_ { get => canMove_; set => canMove_ = value; }

    #region Inversão de depencia
    protected virtual void Construt(IInput newInputInterface, CameraProperties newCamera)
    {
        input_ = newInputInterface;
        camera_ = newCamera;
    }

    protected override void Awake()
    {
        Construt(Object.FindObjectOfType<DesktopInputImpl>(),
            Camera.main.GetComponent<CameraProperties>());
    }
    #endregion

    private void Start()
    {
        #region Get Components
        body_ = GetComponent<Rigidbody>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        playerAnimation_ = GetComponent<PlayerAnimation>();
        captureSystem = GetComponent<CaptureSystem>();
        worldHud_ = GetComponent<HudWorldStats>();
        #endregion

        body_.freezeRotation = true;

        SetInitialCharacter();

        characterStamina = maxStamina;
        worldHud_.HudUpdateStamina(characterStamina, maxStamina);

        camera_.SetMinAngle(1f);
        camera_.SetMinAngle(1f);
    }

    private void Update()
    {
        if (canGetInputs_ && isEnabled)
        {
            if (input_.Jump() && !isJump && canJump_ && characterStamina >= 10.0f)
            {
                body_.AddForce(Vector3.up * initialJumpImpulse, ForceMode.Impulse);
                startJumpTime = true;
                isJump = true;
                playerAnimation_.EnterJump();
                DecrementStamina(10f);
            }

            #region Get Inputs
            if (Input.GetKeyDown(KeyCode.T) && currentKubberSpawned)
            {
                if (currentKubberSpawned.isActiveAndEnabled)
                    SwitchCharacterController(currentKubberSpawned);
            }

            if (Input.GetKeyDown(KeyCode.N)) // Key de Teste
            {
                isSwimMode = !isSwimMode;

                if (isSwimMode)
                {
                    playerAnimation_.EnterInSwimMode();
                    GameObject.Find("Ground").GetComponent<Terrain>().enabled = false;
                }
                else
                {
                    playerAnimation_.ExitInSwimMode();
                    GameObject.Find("Ground").GetComponent<Terrain>().enabled = true;
                }
            }

            if (input_.GetAxisHorizontal() == 0 && input_.GetAxisVertical() == 0)
                playerAnimation_.MovimentSpeed(0);
            #endregion
        }
    }

    private void FixedUpdate()
    {
        if (canMove_)
        {
            if (isEnabled && !captureSystem.captureProcess)
            {
                JumpBehaviour();

                axisX = input_.GetAxisHorizontal();
                axisY = input_.GetAxisVertical();

                Movement();

                canGetInputs_ = true;
            }
            else canGetInputs_ = false;
        }
        else playerAnimation_.MovimentSpeed(0);

        RegenStamina();
    }

    protected override void Movement()
    {
        Vector2 input = new Vector2(axisX, axisY);

        if (input != Vector2.zero)
        {
            float targetrotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg; 

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetrotation + Camera.main.transform.eulerAngles.y,
                ref smooth_,
                smoothTime);

            if (!captureSystem.inCaptureMode)
            {
                if (input_.RunInput())
                    inRunInput = true;
                else
                    inRunInput = false;

                if (input_.RunInput() && inRunInput && !isJump && !endedStamina)
                {
                    transform.position += transform.forward * runSpeed * Time.deltaTime;
                    playerAnimation_.MovimentSpeed(axisX * axisX + axisY * axisY);
                    DecrementStamina(0.5f);

                    if (characterStamina == 0)
                        StartCoroutine(EndedRegenTime());
                }
                else if (input_.RunInput() && inRunInput && isJump)
                {
                    transform.position += transform.forward * (walkSpeed * 1.8f) * Time.deltaTime;
                }
                else
                {
                    transform.position += transform.forward * walkSpeed * Time.deltaTime;
                    float speed = axisX * axisX + axisY * axisY / 2;
                    playerAnimation_.MovimentSpeed(speed >= 0.5 ? 0.5f : speed);

                    if (!input_.RunInput())
                        inRunInput = false;
                    else
                        inRunInput = true;
                }
            }

            if (captureSystem.inCaptureMode)
            {
                if (input_.GetAxisHorizontal() != 0 || input_.GetAxisVertical() != 0)
                {
                    transform.position += transform.forward * (walkSpeed / 2) * Time.deltaTime;
                    playerAnimation_.MovimentSpeed(0.45f);
                }
                else playerAnimation_.MovimentSpeed(0);
            }

            if (captureSystem.captureProcess)
                StopWalk();
        }
    }

    protected override void JumpBehaviour()
    {
        if (input_.Jump() && isJump)
            body_.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (startJumpTime && countJumpTime < jumpTime)
            countJumpTime += Time.deltaTime;

        if (!ExistGround() && countJumpTime >= jumpTime)
            body_.AddForce(Vector3.down * dowmSpeed);

        if (ExistGround() && countJumpTime >= jumpTime && !startJumpWait_)
            StartCoroutine(JumpWaitTime());
    }

    protected override bool ExistGround()
    {
        return Physics.Raycast(transform.position, (-1 * transform.up), 0.4f);
    }

    public void SetInitialCharacter()
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

    public override void SwitchCharacterController(CharacterAbstraction switchCharacter)
    {
        CharacterAbstraction thisCharacter = GetComponent<CharacterAbstraction>();
        CharacterAbstraction[] allCharacters = FindObjectsOfType<CharacterAbstraction>();

        foreach (CharacterAbstraction currentCharacter in allCharacters)
        {
            if (currentCharacter != switchCharacter)
                currentCharacter.isEnabled = false;
        }

        #region stop character walk
        thisCharacter.axisX = 0;
        thisCharacter.axisY = 0;
        playerAnimation_.MovimentSpeed(0);
        #endregion

        if (switchCharacter.GetComponent<MonsterBase>() != null)
        {
            StartCoroutine(switchCharacter.GetComponent<MonsterBase>().StopFollow());
        }
        SetCameraPropeties(switchCharacter.transform.Find("CameraTarget"));
        StartCoroutine(WaitTimeForEnable(switchCharacter));
    }

    protected virtual IEnumerator JumpWaitTime()
    {
        startJumpWait_ = true;
        playerAnimation_.ExitJump();
        yield return new WaitForSeconds(0.2f);
        StopWalk();
        isJump = false;
        startJumpTime = false;
        countJumpTime = 0;
        startJumpWait_ = false;
    }

    public override void IncrementStamina(float increment)
    {
        characterStamina += increment;

        if (characterStamina > maxStamina)
        {
            characterStamina = maxStamina;
        }

        worldHud_.HudUpdateStamina(characterStamina, maxStamina);
    }

    public override void DecrementStamina(float decrement)
    {
        characterStamina -= decrement;

        if (!HaveStamina())
        {
            characterStamina = 0;
            Debug.Log("You not have stamina!");
        }

        worldHud_.HudUpdateStamina(characterStamina, maxStamina);
    }

    public void StopWalk()
    {
        axisX = 0;
        axisY = 0;
        playerAnimation_.MovimentSpeed(0);
        body_.velocity = Vector3.zero;
    }
}
