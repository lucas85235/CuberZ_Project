﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterBase : CharacterAbstraction
{
    protected CharacterAbstraction player_;
    protected NavMeshAgent nav_;
    protected AttackManager attack_;

    protected float attackTime_;
    protected float countAttackTime = 0;

    protected LayerMask inputLayer;

    [Header("Attack Stats")]
    public float attackSpeed = 20.0f;
    public float attackDistance = 5.0f;
    public float attackDecrementTime = 0.3f;

    [Header("Life Stats")]
    protected float mosterLife;
    protected float maxLife = 100;
    protected bool isDead = false;

    [Header("Stamina Stats")]
    protected float mosterStamina;
    protected float maxStamina;

    [Header("IA config")]
    public float minDistance = 12.0f;
    public float followSpeed = 10.0f;
    protected bool isFollowState = true;

    #region AI Behaviour
    protected void FollowPlayer()
    {
        if (Vector3.Distance(player_.transform.position, transform.position) > minDistance)
        {
            nav_.isStopped = false;
            nav_.speed = followSpeed;
            nav_.SetDestination(player_.transform.position);
            #region Set moster walk animation  
            axisX = followSpeed / 2;
            AnimationSpeed();
            #endregion
        }
        else
        {
            if(isFollowState && nav_.isStopped == false)
                StartCoroutine(StopFollow());
        }
    }

    public IEnumerator StopFollow()
    {
        nav_.speed = 0;
        nav_.isStopped = true;
        #region stop moster walk animation
        axisX = 0;
        axisY = 0;
        AnimationSpeed();
        #endregion
        isFollowState = false;
        yield return new WaitForSeconds(0.4f);
        isFollowState = true;
    }
    #endregion

    protected virtual void AttackDirection()
    {
        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, inputLayer))
        {
            if (Vector3.Distance(transform.position, hit.point) > attackDistance)
            {
                transform.LookAt(hit.point);
                animator_.SetTrigger("ATTACK");
            }
        }
    }

    protected virtual bool IsPlayAttackAnimation()
    {
        return animator_.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    protected virtual bool ExistGround()
    {
        return Physics.Raycast(transform.position, (-1 * transform.up), 1f);
    }

    protected virtual string GetAttackName(int index)
    {
        throw new System.NotImplementedException();
    }

    #region Life and Stamina increment and decrement 
    public void IncrementLife(float increment)
    {
        mosterLife += increment;

        if (mosterLife > maxLife)
        {
            mosterLife = maxLife;
        }
    }

    public void DecrementLife(float decrement)
    {
        mosterLife -= decrement;

        if (mosterLife <= 0)
        {
            isDead = true;
            Debug.Log("Life < 0, You Are Dead!");
        }
    }

    public void IncrementStamina(float increment)
    {
        mosterStamina += increment;

        if (mosterStamina > maxStamina)
        {
            mosterStamina = maxStamina;
        }
    }

    public void DecrementStamina(float decrement)
    {
        mosterStamina -= decrement;

        if (!HaveStamina())
        {
            mosterStamina = 0;
            Debug.Log("You not have stamina!");
        }
    }

    public bool HaveStamina()
    {
        return mosterStamina > 0;
    }
    #endregion
}