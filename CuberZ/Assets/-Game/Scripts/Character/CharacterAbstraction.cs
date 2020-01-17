using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbstraction : MonoBehaviour
{
    [Header("Basic Components")]
    protected Rigidbody boby_;
    protected Animator animator_;
    protected GameObject collider_;
    protected GameObject camera_;
    protected CameraController cameraController_;

    protected float axisX;
    protected float axisY;

    [Header("Adjust Camera Propeties")]
    public float cameraDistance = 16.0f;

    [Header("Walk Stats")]
    public float characterSpeed = 15.0f;
    private float smooth_;
    public float smoothTime = 0.3f;

    [Header("Attack Stats")]
    public float attackSpeed = 30.0f;

    public float attackDistance = 5.0f;

    [Header("Life Stats")]
    protected float characterLife;
    protected float maxLife = 100;
    protected bool isDead = false;

    protected virtual void Start() 
    {
        boby_ = GetComponent<Rigidbody>();
        animator_ = GetComponent<Animator>();
        camera_ = Camera.main.gameObject;
        cameraController_ = Camera.main.GetComponent<CameraController>();

        // mudar
        // collider_ = transform.Find("COLISOR").gameObject; AttackCollider

        boby_.freezeRotation = true;

        // pegar dado do save se for um mostro
        // redefinir com as regras especificas para o player e moster's
        isDead = false;
        characterLife = maxLife;

        // se for o player
        // SetCameraPropeties();
        // e de um override no start
        
        // o player deve pegar todos as referencias ao component CharacterAbstraction
        // e desativar esse component se esse não for o seu
        // e somente o player o player deve chamar o
        // SwitchCharacterController();
        // no metodo Start
    }

    protected virtual void Update() 
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
            boby_.velocity = transform.forward * attackSpeed;
        }
        else
        {
            if (ExistGround())
                boby_.velocity = Vector3.zero;
        }
    }

    protected virtual void Walk() 
    {
        Vector2 input = new Vector2(axisX, axisY);

        if (input != Vector2.zero)
        {
            float targetrotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetrotation + camera_.transform.eulerAngles.y, 
                ref smooth_, 
                smoothTime);
            transform.position += transform.forward * characterSpeed * Time.deltaTime;
        }
    }

    protected virtual void AttackDirection() 
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

    protected virtual bool IsPlayAttackAnimation() 
    {
        return animator_.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    protected virtual void AnimationSpeed() 
    {
        animator_.SetFloat("SPEED", axisX * axisX + axisY * axisY);
    }

    protected virtual bool ExistGround() 
    {
        return Physics.Raycast(transform.position, (-1 * transform.up), 1f);
    }

    protected void IncrementLife(float increment) 
    {
        characterLife += increment;

        if (characterLife > maxLife) {
            characterLife = maxLife;
        }
    }

    protected void DecrementLife(float decrement) 
    {
        characterLife -= decrement;

        if (characterLife <= 0) {
            isDead = true;
            Debug.Log("Life < 0, You Are Dead!");
        }
    }

    protected void SwitchCharacterController(CharacterAbstraction character) 
    {
        // Implementação da troca
        // Ativar
        // o player deve pegar todos as referencias ao component CharacterAbstraction
        // e desativar esse component se esse não for o seu
        SetCameraPropeties();
    }

    protected void SetCameraPropeties() 
    {
        cameraController_.SetTarget(this.transform);
        cameraController_.SetCameraDistance(cameraDistance);
    }
}
