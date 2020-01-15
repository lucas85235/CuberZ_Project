using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moster : MonoBehaviour
{
    private Rigidbody boby_;
    private Animator animator_;
    private GameObject collider_;
    private GameObject camera_;

    private float smooth_;

    public float speed = 15.0f;
    public float smoothTime = 0.3f;
    public float attackSpeed = 30.0f;
    public float attackDistance = 5.0f;

    void Start()
    {
        boby_ = GetComponent<Rigidbody>();
        animator_ = GetComponent<Animator>();
        camera_ = Camera.main.gameObject;

        // mudar
        // collider_ = transform.Find("COLISOR").gameObject;

        boby_.freezeRotation = true;
    }

    void Update()
    {
        var currentAnimation = animator_.GetCurrentAnimatorStateInfo(0);

        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");

        if (!PlayAttackAnimation())
        {
            Walk(X, Z);
            AnimationSpeed(X, Z);
        }

        if (Input.GetMouseButtonDown(0))
            AttackDirection();

        // collider_.SetActive(currentAnimation.IsName("Attack"));

        if (currentAnimation.IsName("Attack"))
        {
            boby_.AddForce(transform.forward * attackSpeed);
        }
        else
        {
            if (ExistGround())
                boby_.velocity = Vector3.zero;       
        }
    }

    bool ExistGround()
    {
        return Physics.Raycast(transform.position, (-1 * transform.up), 1f);
    }

    private bool PlayAttackAnimation()
    {
        var animation = animator_.GetCurrentAnimatorStateInfo(0);
        return animation.IsName("Attack");
    }

    void AttackDirection()
    {
        Ray ray = camera_.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Vector3.Distance(transform.position, hit.point) > attackDistance)
            {
                transform.LookAt(hit.point);
                animator_.SetTrigger("ATTACK");
            }
        }
    }
    void Walk(float X, float Z)
    {
        Vector2 input = new Vector2(X, Z);

        if (input != Vector2.zero)
        {
            float targetrotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetrotation + camera_.transform.eulerAngles.y, 
                ref smooth_, 
                smoothTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void AnimationSpeed(float axisX, float axisY)
    {
        animator_.SetFloat("SPEED", axisX * axisX + axisY * axisY);
    }
}
