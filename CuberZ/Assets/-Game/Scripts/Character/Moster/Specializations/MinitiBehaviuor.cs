using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinitiBehaviuor : MonsterBase
{
    // se tiver ataques de outros tipos setar um novo enum
    // que recebera ataques personalizados
    // setar os novos ataques e dar um override no GetAttackName

    private bool canFollowPlayer = true;
    private bool canMove = true;

    [SerializeField]
    private float toHeadButtLenght_ = 1.4f; // Valor anterior = 1.208333f;

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
        body_ = GetComponent<Rigidbody>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        nav_ = GetComponent<NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
            player_ = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterAbstraction>();
        attack_ = GetComponent<AttackManager>();
        #endregion   

        body_.freezeRotation = true;
        nav_.speed = followSpeed;
        nav_.enabled = false;

        inputLayer_ = LayerMask.GetMask("Input");

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

        // Deletar
        // isEnabled = true;
        // SetCameraPropeties(transform.Find("CameraTarget"));
        // Deletar
    }

    protected virtual void Update()
    {
        if (isEnabled) 
        {
            #region Get Inputs
            if (Input.GetKeyDown(KeyCode.N)) // Key de Teste
            {
                isSwimMode = !isSwimMode;

                if (isSwimMode)
                {
                    animation_.EnterInSwimMode();
                    GameObject.Find("Ground").GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    animation_.ExitInSwimMode();
                    GameObject.Find("Ground").GetComponent<MeshRenderer>().enabled = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && !isDead) // Key de Teste
            {
                StartCoroutine(animation_.PlayDeathState());
                isDead = true;
            }

            if (Input.GetKeyDown(KeyCode.R) && isDead) // Key de Teste
            {
                animation_.ExitDeathState();
                isDead = false;
            }

            if (Input.GetKeyDown(KeyCode.F1)) // Key de Teste
                animation_.ExtraAnimationOne();

            if (Input.GetKeyDown(KeyCode.F2)) // Key de Teste
                animation_.ExtraAnimationTwo();

            if (Input.GetKeyDown(KeyCode.T) && player_ != null) // Usado para testes romover na versão final
                SwitchCharacterController(player_);

            if (input_.ExecuteAction() && !isAttacking && !isJump) 
            {
                if (characterStamina > attack_.GetStaminaCost(currentAttackIndex))
                {
                    StartCoroutine(GetAttackName(currentAttackIndex));
                }
                else Debug.Log("Você não tem stmina para realizar este attack!");
            }

            if (input_.KubberAttack1())
                currentAttackIndex = (int)MinitiAttacks.ToHeadButt;
            if (input_.KubberAttack2())
                currentAttackIndex = (int)MinitiAttacks.FireBall;
            if (input_.KubberAttack3())
                currentAttackIndex = (int)MinitiAttacks.RotatoryAttack;
            if (input_.KubberAttack4())
                currentAttackIndex = (int)MinitiAttacks.Bite;
            #endregion            
        }
    }

    protected virtual void FixedUpdate()
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
        }
        else if (!isEnabled && canFollowPlayer)
        {
            if (canFollowState)
                FollowPlayer();
        }

        if(animation_.GetCurrentAnimationInLayerOne().IsName("ToHeadButt"))
        {
            body_.velocity = transform.forward * attackSpeed;
        }
        else body_.velocity = Vector3.zero;

        RegenStamina();
    }

    protected override string GetAttackName(int index)
    {
        currentAttackIndex = index;
        return ((MinitiAttacks)attack_.attackTier[index]).ToString();
    }

    private void MovableSetting() 
    {
        body_.velocity = Vector3.zero;
        isEnabled = true;
        canFollowPlayer = true;
        isAttacking = false;
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

        if (Physics.Raycast(ray, out hit, 1000f, inputLayer_))
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
        yield break;
    }

    public IEnumerator FireBall() 
    {
        //isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.FireBall);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(toHeadButtLenght_);
        //isAttacking = false;

        DebugAttack();
    }

    public IEnumerator RotatoryAttack() 
    {
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.RotatoryAttack);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(toHeadButtLenght_);
        isAttacking = false;

        DebugAttack();
    }

    public IEnumerator Bite() 
    {
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.Bite);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(toHeadButtLenght_);
        isAttacking = false;

        DebugAttack();
    }
}
