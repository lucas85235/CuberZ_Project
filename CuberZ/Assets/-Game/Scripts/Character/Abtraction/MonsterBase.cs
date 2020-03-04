using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMesh))]
[RequireComponent(typeof(AttackManager))]

public abstract class MonsterBase : CharacterAbstraction
{
    [Header("Spawned By Player")]
    public bool spawnByPlayer;

    protected AnimationBase animation_;
    protected CharacterAbstraction player_;
    protected NavMeshAgent nav_;
    protected AttackManager attack_;
    protected LayerMask inputLayer_;
    protected HudWorldStats worldHud_;

    [SerializeField] public bool isAttacking { get; set; }
    [SerializeField] public int currentAttackIndex;

    [Header("Attack Stats")]
    public float attackSpeed = 20.0f;
    public float attackDistance = 5.0f;

    [Header("Life Stats")]
    protected float monsterLife;
    protected float maxLife = 100f;
    protected bool isDead = false;
    [SerializeField] public bool IsDead { get; }
    
    [Header("IA config")]
    public float minDistance = 12.0f;
    public float followSpeed = 10.0f;
    protected bool canFollowState = true;

    #region Inversão de depencia
    protected virtual void Construt(IInput newInputInterface, AnimationBase newAnimation) 
    {
        input_ = newInputInterface;
        animation_ = newAnimation;
    }

    protected override void Awake() 
    {
        Construt(Object.FindObjectOfType<InputSystem>(), GetComponent<AnimationBase>());
        worldHud_ = GetComponent<HudWorldStats>();

        nav_ = GetComponent<NavMeshAgent>();

        // isEnabled = true;
    }
    #endregion

    #region AI Behaviour
    protected void FollowPlayer()
    {
        if (player_ != null)
        {
            Debug.Log("Seguindo o Player!");

            if (Vector3.Distance(player_.transform.position, transform.position) > minDistance)
            {
                if (!nav_.enabled && !isEnabled)
                    nav_.enabled = true;

                if (Vector3.Distance(player_.transform.position, transform.position) > minDistance)
                {
                    nav_.isStopped = false;
                    nav_.speed = followSpeed;
                    nav_.SetDestination(player_.transform.position);
                    #region Set moster walk animation  
                    axisX = followSpeed / 2;
                    animation_.AnimationSpeed(axisX, axisY);
                    #endregion
                }
                else
                {
                    if (canFollowState)
                        StartCoroutine(StopFollow());
                }
            }
            else
            {
                if (canFollowState)
                    StartCoroutine(StopFollow());
            }
        }
    }

    private void StopWalkFunction()
    {
        nav_.speed = 0;
        #region stop moster walk animation
        axisX = 0;
        axisY = 0;
        animation_.AnimationSpeed(axisX, axisY);
        #endregion

    }

    public IEnumerator StopFollow()
    {
        if (nav_.enabled) 
        {
            nav_.speed = 0;
            nav_.enabled = false;
        }

        #region stop moster walk animation
        axisX = 0;
        axisY = 0;
        animation_.AnimationSpeed(axisX, axisY);       
        #endregion
        canFollowState = false;
        yield return new WaitForSeconds(0.4f);
        canFollowState = true;
    }
    #endregion

    protected override void Movement()
    {
        Vector2 input = new Vector2(axisX, axisY);

        if (input != Vector2.zero)
        {
            float targetrotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg; ;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetrotation + Camera.main.transform.eulerAngles.y,
                ref smooth_,
                smoothTime);
            
            if (input_.RunInputUp()) // Quando solta o LeftControl
                inRunInput = false;
            else if (input_.RunInputOnce()) //Quando aperta uma única vez
                inRunInput = true;

            if (input_.RunInput() && inRunInput && !isJump && !endedStamina) 
            {
                transform.position += transform.forward * runSpeed * Time.deltaTime;
                DecrementStamina(0.5f);

                if (characterStamina == 0) 
                    StartCoroutine(EndedRegenTime());           
            }
            else
            {
                transform.position += transform.forward * walkSpeed * Time.deltaTime;
                
                if (!input_.RunInput())
                    inRunInput = false;
                else
                    inRunInput = true;
            }
        }
    }

    protected virtual void Jump() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJump && canJump_ && characterStamina >= 10.0f) 
        {
            body_.AddForce(Vector3.up * initialJumpImpulse, ForceMode.Impulse);
            startJumpTime = true;
            isJump = true;
            animation_.EnterJump();
            DecrementStamina(10f);
        }

        if (Input.GetKey(KeyCode.Space) && isJump)
            body_.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (startJumpTime && countJumpTime < jumpTime)
            countJumpTime += Time.deltaTime;

        if (!ExistGround() && countJumpTime >= jumpTime)
            body_.AddForce(Vector3.down * dowmSpeed);

        if (ExistGround() && countJumpTime >= jumpTime && !startJumpWait_)
            StartCoroutine(JumpWaitTime());
    }

    protected virtual IEnumerator JumpWaitTime() 
    {
        startJumpWait_ = true;
        animation_.ExitJump();
        yield return new WaitForSeconds(0.2f);
        isJump = false;
        startJumpTime = false;
        countJumpTime = 0;
        startJumpWait_ = false;    
    }

    protected virtual string GetAttackName(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void SwitchCharacterController(CharacterAbstraction switchCharacter) 
    {
        CharacterAbstraction thisCharacter = GetComponent<CharacterAbstraction>();
        CharacterAbstraction[] allCharacters = FindObjectsOfType<CharacterAbstraction>();

        foreach(CharacterAbstraction currentCharacter in allCharacters)
        {
            if (currentCharacter != switchCharacter)
                currentCharacter.isEnabled = false;
        }

        #region stop character walk
        thisCharacter.axisX = 0;
        thisCharacter.axisY = 0;
        animation_.AnimationSpeed(axisX, axisY);
        #endregion

        StartCoroutine(StopFollow());
        SetCameraPropeties(switchCharacter.transform.Find("CameraTarget"));
        StartCoroutine(WaitTime(switchCharacter));
    }

    #region Life and Stamina increment and decrement 
    public virtual void IncrementLife(float increment)
    {
        monsterLife += increment;

        if (monsterLife > maxLife)
        {
            monsterLife = maxLife;
        }

        worldHud_.HudUpdateLife(monsterLife, maxLife); // Singleton Recebe um valor e divide por outro vida/vidamax
    }

    public virtual void DecrementLife(float decrement)
    {
        monsterLife -= decrement;

        if (monsterLife <= 0)
        {
            isDead = true;
            Debug.Log("Life < 0, You Are Dead!");
        }

        worldHud_.HudUpdateLife(monsterLife, maxLife); // Singleton Recebe um valor e divide por outro vida/vidamax
    }

    public override void IncrementStamina(float increment)
    {
        characterStamina += increment;

        if (characterStamina > maxStamina)
        {
            characterStamina = maxStamina;
        }

        worldHud_.HudUpdateStamina(characterStamina, maxStamina); // Singleton Recebe um valor e divide por outro stamina/staminamax
    }

    public override void DecrementStamina(float decrement)
    {
        characterStamina -= decrement;

        if (!HaveStamina())
        {
            characterStamina = 0;
            Debug.Log("You not have stamina!");
        }

        worldHud_.HudUpdateStamina(characterStamina, maxStamina); // Singleton Recebe um valor e divide por outro stamina/staminamax
    }
    #endregion
}
