using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinitiBehaviuor : MonsterBase
{
    // se tiver ataques de outros tipos setar um novo enum
    // que recebera ataques personalizados
    // setar os novos ataques e dar mu override no GetAttackName

    // private AttackManager attackManager_;

    public GameObject detectCollision;

    private bool canFollowPlayer = true;
    private bool canMove = true;
    public bool isSwimMode = false;

    private float toHeadButtLenght_ = 1.208333f;

    #region Jump Config
    public float initialJumpImpulse = 8.0f;
    public float jumpForce = 0.03f;
    public float jumpTime = 0.3f;

    private float countJumpTime = 0;
    private bool isJump = false;
    private bool startJumpTime = false;
    #endregion

    public enum MinitiAttacks
    {
        ToHeadButt,
        FireBall,
        RotatoryAttack,
        Bite,
        FireBall2
    }

    private void Start()
    {
        #region Get Components
        boby_ = GetComponent<Rigidbody>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        nav_ = GetComponent<NavMeshAgent>();
        player_ = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterAbstraction>();
        attack_ = GetComponent<AttackManager>();
        #endregion   

        boby_.constraints = RigidbodyConstraints.FreezeAll;
        nav_.speed = followSpeed;
        nav_.enabled = false;

        inputLayer = LayerMask.GetMask("Input");

        #region Set Life And Stats
        IncrementLife(maxLife);
        IncrementStamina(maxStamina);
        isDead = false;
        #endregion

        #region Setar attacks 
        attack_.attackTier[0] = (int)MinitiAttacks.ToHeadButt;
        attack_.attackTier[1] = (int)MinitiAttacks.FireBall;
        attack_.attackTier[2] = (int)MinitiAttacks.RotatoryAttack;
        attack_.attackTier[3] = (int)MinitiAttacks.Bite;

        for (int i = 0; i < (int)MinitiAttacks.FireBall2+1; i++) 
        {
            attack_.SetAttackNamesInStats((MinitiAttacks)i, i);
        }
        #endregion   
    }

    protected virtual void Update()
    {
        if (isEnabled)
        {
            if (!isAttacking || canMove) 
            {
                axisX = input_.GetAxisHorizontal();
                axisY = input_.GetAxisVertical();

                if (!animation_.IsPlayAttackAnimation())
                {
                    Movement();
                    animation_.AnimationSpeed(axisX, axisY);
                }                
            }

            Jump();

            //detectCollision.SetActive(!animation_.GetCurrentAnimation().IsName("Blend Tree"));

            #region Get Inputs
            if (Input.GetKeyDown(KeyCode.N))
            {
                isSwimMode = !isSwimMode;
            }

            if (isSwimMode) 
            {
                GetComponent<Animator>().SetBool("SWIM", true);
                GameObject.Find("Ground").GetComponent<MeshRenderer>().enabled = false;
            }
            else 
            {
                GetComponent<Animator>().SetBool("SWIM", false);
                GameObject.Find("Ground").GetComponent<MeshRenderer>().enabled = true;
            }

            if (Input.GetKeyDown(KeyCode.F) && !isDead) 
                StartCoroutine(Death());

            if (Input.GetKeyDown(KeyCode.R) && isDead) 
            {
                GetComponent<Animator>().SetBool("PLAY-DEAD", false);
                GetComponent<Animator>().SetBool("DEAD", false);
                isDead = false;
            }

            if (Input.GetKeyDown(KeyCode.F1))
                GetComponent<Animator>().SetTrigger("EXTRA-1");
            if (Input.GetKeyDown(KeyCode.F2))
                GetComponent<Animator>().SetTrigger("EXTRA-2");

            if (Input.GetKeyDown(KeyCode.T)) // Usado para testes romover na versão final
                SwitchCharacterController(player_);

            if (input_.ExecuteActionInput() && !isAttacking) 
                StartCoroutine(GetAttackName(currentAttackIndex));

            if (input_.KubberAttack1Input())
                currentAttackIndex = (int)MinitiAttacks.ToHeadButt;
            if (input_.KubberAttack2Input())
                currentAttackIndex = (int)MinitiAttacks.FireBall;
            if (input_.KubberAttack3Input())
                currentAttackIndex = (int)MinitiAttacks.RotatoryAttack;
            if (input_.KubberAttack4Input())
                currentAttackIndex = (int)MinitiAttacks.Bite;
            #endregion
        }
        else if (!isEnabled && canFollowPlayer)
        {
            if (canFollowState)
                FollowPlayer();
        }
        else 
        {
            if (animation_.GetCurrentAnimationInLayerOne().IsName("ToHeadButt"))
                boby_.velocity = transform.forward * attackSpeed;
            else
                boby_.velocity = Vector3.zero;
        }
    }

    IEnumerator Death() 
    {
        isDead = true;
        GetComponent<Animator>().SetBool("DEAD", true);
        yield return new WaitForSeconds(0.1f);
        GetComponent<Animator>().SetBool("PLAY-DEAD", true);
    }

    private void Jump() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJump) 
        {
            boby_.constraints = RigidbodyConstraints.None;
            boby_.freezeRotation = true;
            boby_.AddForce(Vector3.up * initialJumpImpulse, ForceMode.Impulse);
            startJumpTime = true;
            isJump = true;
            GetComponent<Animator>().SetBool("ENTER-JUMP", true);
        }

        if (Input.GetKey(KeyCode.Space) && isJump) 
        {
            boby_.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("PULANDO");
        }

        if (startJumpTime && countJumpTime < jumpTime)
            countJumpTime += Time.deltaTime;

        if (ExistGround() && countJumpTime >= jumpTime)
        {
            countJumpTime = 0;
            startJumpTime = false;
            boby_.constraints = RigidbodyConstraints.FreezeAll;
            isJump = false;
            Debug.Log("ExistGround");
            GetComponent<Animator>().SetBool("ENTER-JUMP", false);
        }
    }

    protected override string GetAttackName(int index)
    {
        currentAttackIndex = index;
        return ((MinitiAttacks)attack_.attackTier[index]).ToString();
    }

    private void MovableSetting() 
    {
        boby_.constraints = RigidbodyConstraints.FreezeAll;
        boby_.velocity = Vector3.zero;
        isEnabled = true;
        canFollowPlayer = true;
        isAttacking = false;
        //detectCollision.SetActive(false);
    }

    private void DebugAttack() 
    {
        Debug.Log("Attack: " + ((MinitiAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log("Pode Mover: " + attack_.GetCanMove(currentAttackIndex));
    }

    public IEnumerator ToHeadButt() 
    {
        canFollowPlayer = false;
        isEnabled = false;
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, inputLayer))
        {
            if (Vector3.Distance(transform.position, hit.point) > attackDistance)
            {
                #region stop character walk
                axisX = 0;
                axisY = 0;
                animation_.AnimationSpeed(axisX, axisY);            
                #endregion

                transform.LookAt(hit.point);
                animation_.NoMovableAttack((int)MinitiAttacks.ToHeadButt);
                boby_.constraints = RigidbodyConstraints.None;
                boby_.freezeRotation = true;

                DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));
            }
            else  
            {   
                MovableSetting();
                yield break;
            }
        }
        else  
        {   
            MovableSetting();
            yield break;
        }
        
        yield return new WaitForSeconds(toHeadButtLenght_);
        DebugAttack();

        MovableSetting();
    }

    public IEnumerator FireBall() 
    {
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.FireBall);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(toHeadButtLenght_);

        DebugAttack();
    }

    public IEnumerator RotatoryAttack() 
    {
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.RotatoryAttack);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(toHeadButtLenght_);

        DebugAttack();
    }

    public IEnumerator Bite() 
    {
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.Bite);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(toHeadButtLenght_);

        DebugAttack();
    }
}
