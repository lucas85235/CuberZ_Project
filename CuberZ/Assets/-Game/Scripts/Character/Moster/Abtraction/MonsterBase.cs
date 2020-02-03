﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMesh))]
[RequireComponent(typeof(AttackManager))]

public abstract class MonsterBase : CharacterAbstraction
{
    protected CharacterAbstraction player_;
    protected NavMeshAgent nav_;
    protected AttackManager attack_;
    protected LayerMask inputLayer;

    [SerializeField] public bool isAttacking { get; set; }
    [SerializeField] public int currentAttackIndex;
    [SerializeField] public bool isSwimMode = false;

    [Header("Jump Config")]
    public float initialJumpImpulse = 8.0f;
    public float jumpForce = 0.03f;
    public float jumpTime = 0.3f;

    protected float countJumpTime = 0;
    protected bool isJump = false;
    protected bool startJumpTime = false;
    
    [Header("Attack Stats")]
    public float attackSpeed = 20.0f;
    public float attackDistance = 5.0f;

    [Header("Life Stats")]
    protected float mosterLife;
    protected float maxLife = 100f;
    protected bool isDead = false;

    [Header("Stamina Stats")]
    protected float mosterStamina;
    protected float maxStamina = 100f;

    [Header("IA config")]
    public float minDistance = 12.0f;
    public float followSpeed = 10.0f;
    protected bool canFollowState = true;

    #region AI Behaviour
    protected void FollowPlayer()
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
            if (canFollowState && nav_.isStopped == false)
                StartCoroutine(StopFollow());
        }
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

    protected virtual void Jump() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJump) 
        {
            boby_.constraints = RigidbodyConstraints.None;
            boby_.freezeRotation = true;
            boby_.AddForce(Vector3.up * initialJumpImpulse, ForceMode.Impulse);
            startJumpTime = true;
            isJump = true;
            animation_.EnterJump();
        }

        if (Input.GetKey(KeyCode.Space) && isJump) 
            boby_.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (startJumpTime && countJumpTime < jumpTime)
            countJumpTime += Time.deltaTime;

        if (ExistGround() && countJumpTime >= jumpTime)
        {
            countJumpTime = 0;
            startJumpTime = false;
            boby_.constraints = RigidbodyConstraints.FreezeAll;
            isJump = false;
            Debug.Log("ExistGround");
            animation_.ExitJump();
        }
    }

    protected virtual bool ExistGround()
    {
        return Physics.Raycast(transform.position, (-1 * transform.up), 0.25f);
    }

    protected virtual string GetAttackName(int index)
    {
        throw new System.NotImplementedException();
    }

    #region Life and Stamina increment and decrement 
    public void IncrementLife(float increment)
    {
        mosterLife += increment;
        Debug.Log(gameObject.name + ": " + mosterLife);

        if (mosterLife > maxLife)
        {
            mosterLife = maxLife;
        }

        HudWorldStats.instance.HudUpdateLife(transform, mosterLife, maxLife); // Singleton Recebe um valor e divide por outro vida/vidamax

    }

    public void DecrementLife(float decrement)
    {
        mosterLife -= decrement;
        Debug.Log(gameObject.name + ": " + mosterLife);

        if (mosterLife <= 0)
        {
            isDead = true;
            Debug.Log("Life < 0, You Are Dead!");
        }

        HudWorldStats.instance.HudUpdateLife(transform, mosterLife, maxLife); // Singleton Recebe um valor e divide por outro vida/vidamax
    }

    public void IncrementStamina(float increment)
    {
        mosterStamina += increment;
        Debug.Log(gameObject.name + ": " + mosterStamina);

        if (mosterStamina > maxStamina)
        {
            mosterStamina = maxStamina;
        }

        HudWorldStats.instance.HudUpdateStamina(transform, mosterStamina, maxStamina); // Singleton Recebe um valor e divide por outro stamina/staminamax
    }

    public void DecrementStamina(float decrement)
    {
        mosterStamina -= decrement;
        Debug.Log(gameObject.name + ": " + mosterStamina);

        if (!HaveStamina())
        {
            mosterStamina = 0;
            Debug.Log("You not have stamina!");
        }

        HudWorldStats.instance.HudUpdateStamina(transform, mosterStamina, maxStamina); // Singleton Recebe um valor e divide por outro stamina/staminamax
    }

    public bool HaveStamina()
    {
        return mosterStamina > 0;
    }
    #endregion
}
