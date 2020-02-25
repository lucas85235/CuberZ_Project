using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaptoramaBehaviuor : MonsterBase
{
    // se tiver ataques de outros tipos setar um novo enum
    // que recebera ataques personalizados
    // setar os novos ataques e dar mu override no GetAttackName

    private bool canFollowPlayer = true;
    private bool canMove = true;

    private float defaultAnimationTime_ = 1.208333f;

    private RapdoramaAnimation attackAnimations_;

    public enum RaptoramaAttacks
    {
        FlyAttack,
        Flamethrower,
        Bite,
        Rollout,
        TripleAttack,
        FireBlast
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
        attackAnimations_ = GetComponent<RapdoramaAnimation>();
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
        attack_.attackTier[0] = (int)RaptoramaAttacks.FlyAttack;
        attack_.attackTier[1] = (int)RaptoramaAttacks.Flamethrower;
        attack_.attackTier[2] = (int)RaptoramaAttacks.Bite;
        attack_.attackTier[3] = (int)RaptoramaAttacks.Rollout;

        for (int i = 0; i < (int)RaptoramaAttacks.FireBlast + 1; i++)
        {
            attack_.SetAttackNamesInStats((RaptoramaAttacks)i, i);
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

            if (input_.ExecuteAction() && !isAttacking)
                StartCoroutine(GetAttackName(currentAttackIndex));

            if (input_.KubberAttack1())
                currentAttackIndex = (int)RaptoramaAttacks.FlyAttack;
            if (input_.KubberAttack2())
                currentAttackIndex = (int)RaptoramaAttacks.Flamethrower;
            if (input_.KubberAttack3())
                currentAttackIndex = (int)RaptoramaAttacks.Bite;
            if (input_.KubberAttack4())
                currentAttackIndex = (int)RaptoramaAttacks.Rollout;
            #endregion
        }
        else if (!isEnabled && canFollowPlayer)
        {
            if (canFollowState)
                FollowPlayer();
        }
        else
        {
            if (animation_.GetCurrentAnimationInLayerOne().IsName("FlyAttack"))
                body_.velocity = transform.forward * attackSpeed;
            else
                body_.velocity = Vector3.zero;
        }

        RegenStamina();
    }

    protected override string GetAttackName(int index)
    {
        currentAttackIndex = index;
        return ((RaptoramaAttacks)attack_.attackTier[index]).ToString();
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
        Debug.Log("Attack: " + ((RaptoramaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log("Pode Mover: " + attack_.GetCanMove(currentAttackIndex));
    }

    public IEnumerator FlyAttack()
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
                AttackAnimation(false, false, (int)RaptoramaAttacks.FlyAttack);
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

        yield return new WaitForSeconds(defaultAnimationTime_);
        DebugAttack();

        MovableSetting();
    }

    public IEnumerator Flamethrower()
    {
        canMove = attack_.GetCanMove(currentAttackIndex);
        bool overrideAnimation = false;

        AttackAnimation(canMove, overrideAnimation, (int)RaptoramaAttacks.Flamethrower);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(defaultAnimationTime_);

        DebugAttack();
    }

    public IEnumerator Bite()
    {
        canMove = attack_.GetCanMove(currentAttackIndex);
        bool overrideAnimation = false;

        AttackAnimation(canMove, overrideAnimation, (int)RaptoramaAttacks.Bite);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(defaultAnimationTime_);

        DebugAttack();
    }

    public IEnumerator Rollout()
    {
        canMove = attack_.GetCanMove(currentAttackIndex);
        bool overrideAnimation = true;

        AttackAnimation(canMove, overrideAnimation, (int)RaptoramaAttacks.Rollout);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(defaultAnimationTime_);

        DebugAttack();
    }

    public IEnumerator TripleAttack()
    {
        canMove = attack_.GetCanMove(currentAttackIndex);
        bool overrideAnimation = false;

        AttackAnimation(canMove, overrideAnimation, (int)RaptoramaAttacks.TripleAttack);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(defaultAnimationTime_);

        DebugAttack();
    }

    public IEnumerator FireBlast()
    {
        canMove = attack_.GetCanMove(currentAttackIndex);
        bool overrideAnimation = false;

        AttackAnimation(canMove, overrideAnimation, (int)RaptoramaAttacks.FireBlast);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(defaultAnimationTime_);

        DebugAttack();
    }

    private void AttackAnimation(bool movable, bool overrideAnimation, int animation)
    {
        if (overrideAnimation)
            attackAnimations_.OverrideMovableAttack(animation);
        else if (movable)
            attackAnimations_.MovableAttack(animation);
        else
            attackAnimations_.NoMovableAttack(animation);
    }
}
