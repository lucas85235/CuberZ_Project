using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBase : CharacterAbstraction
{
    private CharacterAbstraction player_;

    private float attackTime_;
    private float countAttackTime = 0;

    [Header("Attack Stats")]
    public float attackSpeed = 20.0f;
    public float attackDistance = 5.0f;
    public float attackDecrementTime = 0.3f;

    [Header("Layer(s) Mask")]
    public LayerMask inputLayer;

    protected override void Start()
    {
        boby_ = GetComponent<Rigidbody>();
        animator_ = GetComponent<Animator>();
        camera_ = Camera.main.gameObject;
        cameraController_ = Camera.main.GetComponent<CameraController>();
        player_ = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<CharacterAbstraction>();
        // mudar
        // collider_ = transform.Find("COLISOR").gameObject;

        boby_.freezeRotation = true;

        attackTime_ = animator_.GetCurrentAnimatorStateInfo(0).length;
        attackTime_ -= attackDecrementTime;
        
        inputLayer = LayerMask.GetMask("Input");
        characterLife = maxLife;
        isDead = false;
    }

    protected override void Update()
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

        if (Input.GetMouseButtonDown(0))
            AttackDirection();

        if (Input.GetKeyDown(KeyCode.T))
            SwitchCharacterController(player_);
    }

    protected virtual void AttackDirection()
    {
        Ray ray = camera_.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

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
}
