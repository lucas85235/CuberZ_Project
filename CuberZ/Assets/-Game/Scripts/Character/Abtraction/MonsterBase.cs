using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMesh))]
[RequireComponent(typeof(AttackManager))]

public abstract class MonsterBase : CharacterAbstraction
{
    protected AnimationBase animation_;
    protected CharacterAbstraction player_;
    protected NavMeshAgent nav_;
    protected AttackManager attack_;
    protected LayerMask inputLayer_;
    protected HudWorldStats worldHud_;

    [SerializeField] public bool isAttacking { get; set; }
    [SerializeField] public int currentAttackIndex;
    [SerializeField] public bool isSwimMode = false;

    [Header("Jump Config")]
    public float initialJumpImpulse = 10.0f;
    public float jumpForce = 0.03f;
    public float jumpTime = 0.12f;
    public float dowmSpeed = 15;

    private bool startJumpWait_ = false;

    protected bool canJump_ = true;
    protected bool isJump = false;
    protected bool startJumpTime = false;
    protected float countJumpTime = 0;

    [Header("Attack Stats")]
    public float attackSpeed = 20.0f;
    public float attackDistance = 5.0f;

    [Header("Life Stats")]
    protected float monsterLife;
    protected float maxLife = 100f;
    protected bool isDead = false;

    [Header("Stamina Stats")]
    public float staminaRegen = 0.03f;
    public float regenStartTime = 1.0f;  

    [SerializeField] protected float monsterStamina;
    protected float maxStamina = 100f;
    protected float countRegenStartTime = 0;
    protected bool startRegenProcess = false;
    protected bool inRunInput = false;
    protected bool endedStamina = false;
    private bool startEnded = false;
    
    [Header("IA config")]
    public float minDistance = 12.0f;
    public float followSpeed = 10.0f;
    protected bool canFollowState = true;

    [HideInInspector]
    public bool beenCapture { get { return beenCapture_; } set { beenCapture_ = value; } }
    private bool beenCapture_;

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
    }
    #endregion

    #region AI Behaviour
    protected void FollowPlayer()
    {
        if (player_ != null )
        {
            if (Vector3.Distance(player_.transform.position, transform.position) > minDistance)
            {
                if (!nav_.enabled)
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

    private void StopFollowFunction()
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
            nav_.isStopped = true;
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
            
            if (Input.GetKeyUp(KeyCode.LeftShift))
                inRunInput = false;
            else if (Input.GetKeyDown(KeyCode.LeftShift))
                inRunInput = true;

            if (input_.RunInput() && inRunInput && !isJump && !endedStamina) 
            {
                transform.position += transform.forward * runSpeed * Time.deltaTime;
                DecrementStamina(0.5f);

                if (monsterStamina == 0) 
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
    
    private IEnumerator EndedRegenTime() 
    {
        startEnded = true;
        endedStamina = true; 
        yield return new WaitUntil(()=> inRunInput == false && monsterStamina > 5f);
        endedStamina = false;
        startEnded = false;
    }

    protected virtual void Jump() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJump && canJump_) 
        {
            body_.AddForce(Vector3.up * initialJumpImpulse, ForceMode.Impulse);
            startJumpTime = true;
            isJump = true;
            animation_.EnterJump();
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

    protected virtual bool ExistGround()
    {
        return Physics.Raycast(transform.position, (-1 * transform.up), 0.2f);
    }

    protected virtual string GetAttackName(int index)
    {
        throw new System.NotImplementedException();
    }

    protected override void SwitchCharacterController(CharacterAbstraction switchCharacter) 
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

        SetCameraPropeties(switchCharacter.transform.Find("CameraTarget"));
        StartCoroutine(WaitTime(switchCharacter));
    }

    #region Life and Stamina increment and decrement 
    public void IncrementLife(float increment)
    {
        monsterLife += increment;

        if (monsterLife > maxLife)
        {
            monsterLife = maxLife;
        }

        worldHud_.HudUpdateLife(monsterLife, maxLife); // Singleton Recebe um valor e divide por outro vida/vidamax
    }

    public void DecrementLife(float decrement)
    {
        monsterLife -= decrement;

        if (monsterLife <= 0)
        {
            isDead = true;
            Debug.Log("Life < 0, You Are Dead!");
        }

        worldHud_.HudUpdateLife(monsterLife, maxLife); // Singleton Recebe um valor e divide por outro vida/vidamax
    }

    public void IncrementStamina(float increment)
    {
        monsterStamina += increment;

        if (monsterStamina > maxStamina)
        {
            monsterStamina = maxStamina;
        }

        worldHud_.HudUpdateStamina(monsterStamina, maxStamina); // Singleton Recebe um valor e divide por outro stamina/staminamax
    }

    public void DecrementStamina(float decrement)
    {
        monsterStamina -= decrement;

        if (!HaveStamina())
        {
            monsterStamina = 0;
            Debug.Log("You not have stamina!");
        }

        worldHud_.HudUpdateStamina(monsterStamina, maxStamina); // Singleton Recebe um valor e divide por outro stamina/staminamax
    }

    public void RegenStamina() 
    {
        if (monsterStamina < maxStamina) 
        {
            if (!startRegenProcess) 
            {
                startRegenProcess = true;
                countRegenStartTime = 0;
            }
            else  countRegenStartTime += Time.deltaTime;

            if (countRegenStartTime >= regenStartTime) 
                IncrementStamina(staminaRegen);
            
        }
    }

    public void RestartRegenProcess() 
    {
        startRegenProcess = false;
    }

    public bool HaveStamina()
    {
        return monsterStamina > 0;
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
            canJump_ = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
            canJump_ = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && isJump)
        {
            transform.position += new Vector3(0, -0.1f, 0);
            body_.AddForce(Vector3.down * dowmSpeed);
        }
    }
}
