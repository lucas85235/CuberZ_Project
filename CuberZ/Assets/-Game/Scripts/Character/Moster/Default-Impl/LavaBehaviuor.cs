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
            axisX = input_.GetAxisHorizontal();
            axisY = input_.GetAxisVertical();

            if (!animation_.IsPlayAttackAnimation())
            {
                Movement();
                animation_.AnimationSpeed(axisX, axisY);
            }

            // collider_.SetActive(currentAnimation.IsName("Attack"));

            if (animation_.GetCurrentAnimation().IsName("ToHeadButt"))
            {
                boby_.constraints = RigidbodyConstraints.None;
                boby_.freezeRotation = true;
                boby_.velocity = transform.forward * attackSpeed;
            }
            else
                if (ExistGround())
                {
                    boby_.constraints = RigidbodyConstraints.FreezeAll;
                    boby_.velocity = Vector3.zero;
                }

            #region Get Inputs
            
            if (Input.GetKeyDown(KeyCode.T)) // Usado para testes romover na versão final
                SwitchCharacterController(player_);

            if (input_.ExecuteActionInput())
                StartCoroutine(GetAttackName(currentAttackIndex));

            if (input_.KubberAttack1Input()) 
                currentAttackIndex = (int)MinitiAttacks.ToHeadButt;
            if (input_.KubberAttack2Input())
                currentAttackIndex = (int)MinitiAttacks.FireBall;
            
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
        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, inputLayer))
        {
            if (Vector3.Distance(transform.position, hit.point) > attackDistance)
            {
                animation_.NoMovableAttack((int)MinitiAttacks.ToHeadButt);
            }
        }
        else yield break;
    
        yield return new WaitForSeconds(attack_.GetAttackCoolDown(currentAttackIndex));
        Debug.Log(((MinitiAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }

    public virtual IEnumerator FireBall() 
    {
        animation_.MovableAttack((int)MinitiAttacks.FireBall);

        yield return null;
        Debug.Log(((MinitiAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        Debug.Log(attack_.GetCanMove(currentAttackIndex));
    }
}
