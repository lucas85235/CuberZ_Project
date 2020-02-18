using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TigranofireBehaviour : MonsterBase
{
    // se tiver ataques de outros tipos setar um novo enum
    // que recebera ataques personalizados
    // setar os novos ataques e dar mu override no GetAttackName

    private bool canFollowPlayer = true;
    private bool canMove = true;

    private float flameWheelDuration = 3;

    public enum TigrofireAttacks
    {
        FireWheel,
        FlameThrower,
        FireSequence,
        HyperBeam,
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

        body_.constraints = RigidbodyConstraints.FreezeAll;
        nav_.speed = followSpeed;
        nav_.enabled = false;

        inputLayer_ = LayerMask.GetMask("Input");

        #region Set Life And Stats
        IncrementLife(maxLife);
        IncrementStamina(maxStamina);
        isDead = false;
        #endregion

        #region Setar attacks 
        attack_.attackTier[0] = (int)TigrofireAttacks.FireWheel;
        attack_.attackTier[1] = (int)TigrofireAttacks.FlameThrower;
        attack_.attackTier[2] = (int)TigrofireAttacks.FireSequence;
        attack_.attackTier[3] = (int)TigrofireAttacks.HyperBeam;

        for (int i = 0; i < (int)TigrofireAttacks.HyperBeam + 1; i++)
        {
            attack_.SetAttackNamesInStats((TigrofireAttacks)i, i);
        }
        #endregion

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

            if (input_.ExecuteActionInput() && !isAttacking)
                StartCoroutine(GetAttackName(currentAttackIndex));

            if (input_.KubberAttack1Input())
                currentAttackIndex = (int)TigrofireAttacks.FireWheel;
            if (input_.KubberAttack2Input())
                currentAttackIndex = (int)TigrofireAttacks.FlameThrower;
            if (input_.KubberAttack3Input())
                currentAttackIndex = (int)TigrofireAttacks.FireSequence;
            if (input_.KubberAttack4Input())
                currentAttackIndex = (int)TigrofireAttacks.HyperBeam;
            #endregion
        }
        else if (!isEnabled && canFollowPlayer)
        {
            if (canFollowState)
                FollowPlayer();
        }
        else
        {
            if (animation_.GetCurrentAnimationInLayerOne().IsName("FireWheel"))
                body_.velocity = transform.forward * attackSpeed;
            else
                body_.velocity = Vector3.zero;
        }
    
        RegenStamina();
    }

    protected override string GetAttackName(int index)
    {
        currentAttackIndex = index;
        return ((TigrofireAttacks)attack_.attackTier[index]).ToString();
    }

    private void MovableSetting()
    {
        body_.constraints = RigidbodyConstraints.FreezeAll;
        body_.velocity = Vector3.zero;
        isEnabled = true;
        canFollowPlayer = true;
        isAttacking = false;
    }

    private void DebugAttack()
    {
        Debug.Log("Attack: " + ((TigrofireAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log("Pode Mover: " + attack_.GetCanMove(currentAttackIndex));
    }


    public IEnumerator FireWheel()
    {
        canFollowPlayer = false;
        isEnabled = false;
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, inputLayer_))
        {
            #region stop character walk
            axisX = 0;
            axisY = 0;
            animation_.AnimationSpeed(axisX, axisY);
            #endregion

            transform.LookAt(hit.point);
            body_.constraints = RigidbodyConstraints.None;
            animation_.NoMovableAttack((int)TigrofireAttacks.FireWheel);
            body_.freezeRotation = true;
            DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));
        }
        else
        {
            MovableSetting();
            yield break;
        }

        yield return new WaitForSeconds(flameWheelDuration);
        DebugAttack();

        MovableSetting();
        yield break;
    }


    public IEnumerator FlameThrower()
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
                animation_.NoMovableAttack((int)TigrofireAttacks.FlameThrower);
                body_.constraints = RigidbodyConstraints.None;
                body_.freezeRotation = true;

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

        yield return new WaitForSeconds(flameWheelDuration);
        DebugAttack();

        MovableSetting();
    }


    public IEnumerator FireSequence()
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
                animation_.NoMovableAttack((int)TigrofireAttacks.FireSequence);
                body_.constraints = RigidbodyConstraints.None;
                body_.freezeRotation = true;

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

        DebugAttack();

        MovableSetting();
    }


    public IEnumerator HyperBeam()
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
                animation_.NoMovableAttack((int)TigrofireAttacks.HyperBeam);
                body_.constraints = RigidbodyConstraints.None;
                body_.freezeRotation = true;

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

        DebugAttack();

        MovableSetting();
    }

}
