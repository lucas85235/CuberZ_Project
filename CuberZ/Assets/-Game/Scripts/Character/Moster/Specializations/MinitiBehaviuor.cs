using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinitiBehaviuor : MonsterBase
{
    // se tiver ataques de outros tipos setar um novo enum
    // que recebera ataques personalizados
    // setar os novos ataques e dar mu override no GetAttackName

    // private AttackManager attackManager_;

    public GameObject detectCollision;

    private int currentAttackIndex;

    private bool canFollowPlayer = true;
    private bool isAttacking = false;

    private float toHeadButtLenght_ = 1.208333f;

    public enum MinitiAttacks
    {
        ToHeadButt,
        FireBall
    }

    private void Start()
    {
        #region Get Components
        boby_ = GetComponent<Rigidbody>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        nav_ = GetComponent<NavMeshAgent>();
        player_ = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterAbstraction>();
        attack_ = GetComponent<AttackManager>();
        #endregion   

        boby_.constraints = RigidbodyConstraints.FreezeAll;
        nav_.speed = followSpeed;

        inputLayer = LayerMask.GetMask("Input");

        attackTime_ = animation_.GetCurrentAnimation().length;
        attackTime_ -= attackDecrementTime;

        #region Set Life And Stats
        IncrementLife(maxLife);
        isDead = false;
        #endregion

        #region Setar attacks 
        attack_.attackTier[0] = (int)MinitiAttacks.ToHeadButt;
        attack_.attackTier[1] = (int)MinitiAttacks.FireBall;

        for (int i = 0; i < (int)MinitiAttacks.FireBall+1; i++) 
        {
            attack_.SetAttackNamesInStats((MinitiAttacks)i, i);
        }
        #endregion   
    }

    protected virtual void Update()
    {
        if (isEnabled)
        {
            if (!isAttacking || !attack_.GetCanMove(currentAttackIndex)) 
            {
                axisX = input_.GetAxisHorizontal();
                axisY = input_.GetAxisVertical();

                if (!animation_.IsPlayAttackAnimation())
                {
                    Movement();
                    animation_.AnimationSpeed(axisX, axisY);
                }                
            }

            detectCollision.SetActive(!animation_.GetCurrentAnimation().IsName("Blend Tree"));

            #region Get Inputs
            
            if (Input.GetKeyDown(KeyCode.T)) // Usado para testes romover na versão final
                SwitchCharacterController(player_);

            if (input_.ExecuteActionInput() && !isAttacking) 
            {
                isAttacking = true;
                StartCoroutine(GetAttackName(currentAttackIndex));
            }

            if (input_.KubberAttack1Input()) 
                currentAttackIndex = (int)MinitiAttacks.ToHeadButt;
            if (input_.KubberAttack2Input())
                currentAttackIndex = (int)MinitiAttacks.FireBall;
            
            #endregion
        }
        else if (!isEnabled && canFollowPlayer)
        {
            if (isFollowState)
                FollowPlayer();
        }
        else 
        {
            if (animation_.GetCurrentAnimation().IsName("ToHeadButt"))
                boby_.velocity = transform.forward * attackSpeed;
            else
                boby_.velocity = Vector3.zero;
        }
    }

    protected override string GetAttackName(int index)
    {
        currentAttackIndex = index;
        return ((MinitiAttacks)attack_.attackTier[index]).ToString();
    }

    private void MovableSetting() 
    {
        boby_.constraints = RigidbodyConstraints.FreezeAll;
        boby_.velocity = Vector3.zero;
        isEnabled = true;
        canFollowPlayer = true;
        isAttacking = false;
    }

    private void DebugAttack() 
    {
        Debug.Log("Attack: " + ((MinitiAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log("Pode Mover: " + attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator ToHeadButt() 
    {
        canFollowPlayer = false;
        isEnabled = false;

        #region stop character walk
        while (axisX > 0 && axisY > 0) 
        {
            axisX -= Time.deltaTime * 2;
            axisY -= Time.deltaTime * 2;
            animation_.AnimationSpeed(axisX, axisY);            
        }
        #endregion

        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, inputLayer))
        {
            if (Vector3.Distance(transform.position, hit.point) > attackDistance)
            {
                transform.LookAt(hit.point);
                animation_.NoMovableAttack((int)MinitiAttacks.ToHeadButt);
                boby_.constraints = RigidbodyConstraints.None;
                boby_.freezeRotation = true;
            }
        }
        else  
        {   
            MovableSetting();
            yield break;
        }
        
        yield return new WaitForSeconds(toHeadButtLenght_);
        DebugAttack();

        MovableSetting();
    }

    public virtual IEnumerator FireBall() 
    {
        animation_.MovableAttack((int)MinitiAttacks.FireBall);

        yield return null;
        DebugAttack();

        isAttacking = false;
    }
}
