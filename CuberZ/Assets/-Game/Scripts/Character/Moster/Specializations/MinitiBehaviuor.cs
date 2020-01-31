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

    private bool canFollowPlayer = true;
    private bool canMove = true;

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

        #region Set Life And Stats
        IncrementLife(maxLife);
        IncrementStamina(maxStamina);
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

            //detectCollision.SetActive(!animation_.GetCurrentAnimation().IsName("Blend Tree"));

            #region Get Inputs
            
            if (Input.GetKeyDown(KeyCode.T)) // Usado para testes romover na versão final
                SwitchCharacterController(player_);

            if (input_.ExecuteActionInput() && !isAttacking) 
                StartCoroutine(GetAttackName(currentAttackIndex));

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
            if (animation_.GetCurrentAnimationInLayerOne().IsName("ToHeadButt"))
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
        //detectCollision.SetActive(false);
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
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, inputLayer))
        {
            if (Vector3.Distance(transform.position, hit.point) > attackDistance)
            {
                #region stop character walk
                axisX = 0;
                axisY = 0;
                animation_.AnimationSpeed(axisX, axisY);            
                #endregion

                transform.LookAt(hit.point);
                animation_.NoMovableAttack((int)MinitiAttacks.ToHeadButt);
                boby_.constraints = RigidbodyConstraints.None;
                boby_.freezeRotation = true;

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
        
        yield return new WaitForSeconds(toHeadButtLenght_);
        DebugAttack();

        MovableSetting();
    }

    public virtual IEnumerator FireBall() 
    {
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.FireBall);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(toHeadButtLenght_);

        DebugAttack();
    }
}
