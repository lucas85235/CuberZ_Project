using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LavaBehaviuor : MonsterBase
{
    // se tiver ataques de outros tipos setar um novo enum
    // que recebera ataques personalizados
    // setar os novos ataques e dar mu override no GetAttackName

    // private AttackManager attackManager_;

    private int currentAttackIndex;

    private bool canMove;

    public enum MinitiAttacks
    {
        ToHeadButt,
        FireBall
    }

    private void Start()
    {
        #region Get Components
        boby_ = GetComponent<Rigidbody>();
        animator_ = GetComponent<Animator>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        nav_ = GetComponent<NavMeshAgent>();
        player_ = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterAbstraction>();
        attack_ = GetComponent<AttackManager>();
        #endregion   

        boby_.constraints = RigidbodyConstraints.FreezeAll;
        nav_.speed = followSpeed;

        inputLayer = LayerMask.GetMask("Input");

        attackTime_ = animator_.GetCurrentAnimatorStateInfo(0).length;
        attackTime_ -= attackDecrementTime;

        #region Set Life And Stats
        IncrementLife(maxLife);
        isDead = false;
        #endregion

        //attack_.SetRandomAttackTier(11);
        attack_.attackTier[0] = (int)MinitiAttacks.ToHeadButt;
        attack_.attackTier[1] = (int)MinitiAttacks.FireBall;

        for (int i = 0; i < (int)MinitiAttacks.FireBall+1; i++) 
        {
            attack_.SetAttackNamesInStats((MinitiAttacks)i, i);
        }
    }

    protected virtual void Update()
    {
        if (isEnabled)
        {
            var currentAnimation = animator_.GetCurrentAnimatorStateInfo(0);

            axisX = input_.GetAxisHorizontal();
            axisY = input_.GetAxisVertical();

            if (!IsPlayAttackAnimation())
            {
                Walk();
                AnimationSpeed();
            }

            // collider_.SetActive(currentAnimation.IsName("Attack"));

            if (currentAnimation.IsName("Attack"))
            {
                if (countAttackTime < attackTime_)
                {
                    boby_.velocity = transform.forward * attackSpeed;
                    countAttackTime += Time.deltaTime;
                }
            }
            else
            {
                if (ExistGround())
                    boby_.velocity = Vector3.zero;
                countAttackTime = 0;
            }

            #region Get Inputs

            //Colocar AttackDirection no attack de investida
            //if (Input.GetMouseButtonDown(0))
            //    AttackDirection();

            // Usado para testes romover na versão final
            if (Input.GetKeyDown(KeyCode.T) /*&& !inBattleMode*/)
                SwitchCharacterController(player_);

            if (input_.ExecuteActionInput())
                StartCoroutine(GetAttackName(currentAttackIndex));

            if (input_.KubberAttack1Input())
                currentAttackIndex = 0;
            if (input_.KubberAttack2Input())
                currentAttackIndex = 1;
            /* if (input_.KubberAttack3Input())
                currentAttackIndex = 2;
            if (input_.KubberAttack4Input())
                currentAttackIndex = 3; */
            
            #endregion
        }
        else
        {
            if (isFollowState)
                FollowPlayer();
        }
    }
    protected override string GetAttackName(int index)
    {
        currentAttackIndex = index;
        return ((MinitiAttacks)attack_.attackTier[index]).ToString();
    }

    public virtual IEnumerator ToHeadButt() 
    {
        animator_.SetBool("CAN-MOVE", false);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", 0);
        yield return new WaitForSeconds(attack_.GetAttackCoolDown(currentAttackIndex));
        Debug.Log(((MinitiAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator FireBall() 
    {
        animator_.SetBool("CAN-MOVE", true);
        animator_.SetTrigger("ATTACK");
        animator_.SetInteger("CHOICE-ATTACK", 1);
        yield return null;
        Debug.Log(((MinitiAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));

    }
}
