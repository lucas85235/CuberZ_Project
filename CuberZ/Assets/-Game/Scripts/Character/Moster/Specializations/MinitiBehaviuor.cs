using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinitiBehaviuor : MonsterBase
{
    // se tiver ataques de outros tipos setar um novo enum
    // que recebera ataques personalizados
    // setar os novos ataques e dar um override no GetAttackName

    [SerializeField]
    private float toHeadButtLenght_ = 1.4f; // Valor anterior = 1.208333f;

    public enum MinitiAttacks
    {
        ToHeadButt,
        FireBall,
        RotatoryAttack,
        Bite,
        FireBall2
    }

    private void Start()
    {
        #region Get Components
        body_ = GetComponent<Rigidbody>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        nav_ = GetComponent<NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
            player_ = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterAbstraction>();
        attack_ = GetComponent<AttackManager>();
        attackCollision = transform.Find("DetectCollision").GetComponent<DetectAttackCollision>();
        #endregion   

        body_.freezeRotation = true;
        nav_.speed = followSpeed;
        nav_.enabled = false;

        inputLayer_ = LayerMask.GetMask("Input");

        #region Set Life And Stats
        IncrementLife(maxLife);
        IncrementStamina(maxStamina);
        isDead = false;
        #endregion

        #region Setar attacks 
        attack_.attackTier[0] = (int)MinitiAttacks.ToHeadButt;
        attack_.attackTier[1] = (int)MinitiAttacks.FireBall;
        attack_.attackTier[2] = (int)MinitiAttacks.RotatoryAttack;
        attack_.attackTier[3] = (int)MinitiAttacks.Bite;

        attackCollision.UpdateCurrentAttackStats(attack_.attackStats[currentAttackIndex]);

        for (int i = 0; i < (int)MinitiAttacks.FireBall2+1; i++) 
        {
            attack_.SetAttackNamesInStats((MinitiAttacks)i, i);
        }
        #endregion

        // Deletar
        isEnabled = true;
        SetCameraPropeties(transform.Find("CameraTarget"));
        // Deletar
    }

    protected override void Update()
    {
        base.Update();

        #region Individual Skills
        if (isEnabled) 
        {
            if (input_.KubberAttack1())
                currentAttackIndex = (int)MinitiAttacks.ToHeadButt;
                attackCollision.UpdateCurrentAttackStats(attack_.attackStats[currentAttackIndex]);
            }
            else if (input_.KubberAttack2())
            {
                currentAttackIndex = (int)MinitiAttacks.FireBall;
                attackCollision.UpdateCurrentAttackStats(attack_.attackStats[currentAttackIndex]);
            }
            else if (input_.KubberAttack3())
            {
                currentAttackIndex = (int)MinitiAttacks.RotatoryAttack;
                attackCollision.UpdateCurrentAttackStats(attack_.attackStats[currentAttackIndex]);
            }
            else if (input_.KubberAttack4())
            {
                currentAttackIndex = (int)MinitiAttacks.Bite;
        }
        #endregion
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        #region AnimationBehaviour

        if (animation_.GetCurrentAnimationInLayerOne().IsName("ToHeadButt"))
        {
            body_.velocity = transform.forward * attackSpeed;
        }
        else body_.velocity = Vector3.zero;

        #endregion

    }

    protected override string GetAttackName(int index)
    {
        currentAttackIndex = index;
        return ((MinitiAttacks)attack_.attackTier[index]).ToString();
    }

    private void MovableSetting() 
    {
        body_.velocity = Vector3.zero;
        isEnabled = true;
        canFollowPlayer = true;
        isAttacking = false;
    }

    private void DebugAttack() 
    {
        // Debug.Log("Attack: " + ((MinitiAttacks)attack_.attackTier[currentAttackIndex]).ToString());
        // Debug.Log("Pode Mover: " + attack_.GetCanMove(currentAttackIndex));
    }

    public IEnumerator ToHeadButt() 
    {
        canFollowPlayer = false;
        isEnabled = false;
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, inputLayer_))
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
        
        yield return new WaitForSeconds(attack_.GetAttackAnimationTime(currentAttackIndex));
        DebugAttack();
        MovableSetting();
        yield break;
    }

    public IEnumerator FireBall() 
    {
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.FireBall);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(attack_.GetAttackAnimationTime(currentAttackIndex));
        isAttacking = false;

        DebugAttack();
    }

    public IEnumerator RotatoryAttack() 
    {
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.RotatoryAttack);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(attack_.GetAttackAnimationTime(currentAttackIndex));
        isAttacking = false;

        DebugAttack();
    }

    public IEnumerator Bite() 
    {
        isAttacking = true;
        canMove = attack_.GetCanMove(currentAttackIndex);

        animation_.MovableAttack((int)MinitiAttacks.Bite);
        DecrementStamina(attack_.GetStaminaCost(currentAttackIndex));

        yield return new WaitForSeconds(attack_.GetAttackAnimationTime(currentAttackIndex));
        isAttacking = false;

        DebugAttack();
    }
}
