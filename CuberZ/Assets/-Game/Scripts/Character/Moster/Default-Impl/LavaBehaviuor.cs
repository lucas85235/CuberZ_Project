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

    protected virtual void Start()
    {
        boby_.constraints = RigidbodyConstraints.FreezeAll;
        nav_.speed = followSpeed;

        inputLayer = LayerMask.GetMask("Input");

        attackTime_ = animator_.GetCurrentAnimatorStateInfo(0).length;
        attackTime_ -= attackDecrementTime;

        #region Set Life And Stats
        IncrementLife(maxLife);
        isDead = false;
        #endregion

        attack_.SetRandomAttackTier(11);
    }

    protected virtual void Update()
    {
        if (isEnabled)
        {
            var currentAnimation = animator_.GetCurrentAnimatorStateInfo(0);

            axisX = Input.GetAxis("Horizontal");
            axisY = Input.GetAxis("Vertical");

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

            if (Input.GetKeyDown(KeyCode.T) /*&& !inBattleMode*/)
                SwitchCharacterController(player_);

            if (Input.GetMouseButtonDown(0))
                StartCoroutine(GetAttackName(currentAttackIndex));

            if (Input.GetKeyDown(KeyCode.Alpha1))
                currentAttackIndex = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                currentAttackIndex = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                currentAttackIndex = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4))
                currentAttackIndex = 3;
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
        return ((AttackManager.DefaultLavaAttacks)attack_.attackTier[index]).ToString();
    }

    public virtual IEnumerator FlameOfFire() 
    {
        yield return new WaitForSeconds(attack_.GetAttackCoolDown(currentAttackIndex));
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator FireBall() 
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));

    }

    public virtual IEnumerator FireVortex() 
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator SpitsFire() 
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator FireBearing() 
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public IEnumerator FirePunch()
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator OnslaughtOfFire()
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator LavaRain()
    {
        yield return new WaitForSeconds(1);
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator LivingFire()
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator FireRay()
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator VolcanicAttack()
    {
        yield return null;
        Debug.Log(((AttackManager.DefaultLavaAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }
}
