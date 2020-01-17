using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moster : CharacterAbstraction
{
    private float attackTime_;
    private float countAttackTime = 0;
    public float attackDecrementTime = 0.3f;
    public float attackSpeed = 30.0f;

    private override void Start()
    {
        boby_ = GetComponent<Rigidbody>();
        animator_ = GetComponent<Animator>();
        camera_ = Camera.main.gameObject;
        cameraController_ = Camera.main.GetComponent<CameraController>();

        // mudar
        // collider_ = transform.Find("COLISOR").gameObject;

        boby_.freezeRotation = true;

        attackTime_ = animator_.GetCurrentAnimatorStateInfo(0).length;
        attackTime_ -= attackDecrementTime;

        // se existir save atribuir o save
        // isDead = data.CurrentMosterLifeState();

        characterLife = maxLife;
    }

    private override void Update()
    {
        var currentAnimation = animator_.GetCurrentAnimatorStateInfo(0);

        axisX = Input.GetAxis("Horizontal");
        axisY = Input.GetAxis("Vertical");

        if (!IsPlayAttackAnimation())
        {
            Walk();
            AnimationSpeed();
        }
        
        if (Input.GetMouseButtonDown(0))
            AttackDirection();

        // collider_.SetActive(currentAnimation.IsName("Attack"));
        
        if (currentAnimation.IsName("Attack"))
        {
            if(countAttackTime < attackTime_)
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
    }
}
