using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAManagerDefault : MonsterBase
{
    [Header("Variáveis Alocáveis")]
    public Transform target;

    [Header("Variáveis de Controle")] 
    // [Tooltip("Tenta achar o Target sozinho")]
    // public bool findTargetAutomatic = true;
    [Tooltip("O quão distante ele pode ir caminhando normalmente")]
    [Range(1,10)] public float walkRadius = 10;
    [Tooltip("Evita que o Kubber entre um dentro do outro")]
    [Range(2, 30)] public float stopDistance = 10; 
    [Tooltip("Tempo parado após usar uma skill")]
    [Range(0,3f)] public float waitTimer = 3;
    [Tooltip("A cada quantos segundos o Kubber procura um skill para se utilizar")]
    [Range(0,10f)] public float timerUpdate = 3;
    [Tooltip("Modos da IA")]
    public State iaState;
    [Tooltip("Controla distÂncia e qual skill será usada.")]
    public SkillsAndDistance[] skillStats;

    private Vector3 previousVelocity_;
    private Vector3 firstPos_;
    private float timerToCountUpdate_;
    private bool useSkill_;
    private bool canControl_;
    private bool goOut_;
    
    #region Métodos MonoBehaviour

    private void Start()
    {
        nav_ = GetComponent<NavMeshAgent>();
        animation_ = GetComponent<AnimationBase>();
        worldHud_ = GetComponent<HudWorldStats>();
        attack_ = GetComponent<AttackManager>();

        firstPos_ = transform.position;
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (iaState == State.Walk) 
            ControlWalkState();
        else if (iaState == State.Batlle) 
            ControlBattleState();

        RegenStamina();
    }

    private void OnTriggerStay(Collider other) 
    {
        if (Input.GetKey(KeyCode.E) && other.tag == "Player") 
        {
            if (iaState == State.Walk) 
            {
                iaState = State.Batlle;
            }    
        }
    }
    #endregion

    #region Walk State
    private void ControlWalkState()
    {
        animation_.AnimationSpeed(Velocity(), Velocity());

        if (canControl_) 
        {
            if (goOut_)
                GoBackToOrigin();
            else 
                FindSomeWhereToWalk();

            canControl_ = false;
        }
    }

    private void GoBackToOrigin()
    {
        nav_.SetDestination(firstPos_);
        goOut_ = false;
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(Random.Range(4, 6f));
        canControl_ = true;
        yield break;
    }
    
    // Função que controla para onde o Kubber vai andar.
    private void FindSomeWhereToWalk()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit = new NavMeshHit();
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        nav_.SetDestination(hit.position);
        goOut_ = true;

        StartCoroutine(Timer());
    } 
    #endregion

    #region Battle State
    private void ControlBattleState()
    {
        animation_.AnimationSpeed(Velocity(), Velocity());

        FollowTarget();
        CallSkill();
        WaitForAnimationOver();
    }

    // Usada quando o Kubber está indo atrás do target para atacar 
    private void FollowTarget()
    {
        if (target)
        {
            if (nav_.enabled && !useSkill_ && DistanceBetweenTarget() > stopDistance)
            {
                nav_.enabled = true;

                if (nav_.velocity != Vector3.zero) 
                {
                    transform.rotation = Quaternion.Lerp(
                        transform.rotation, 
                        Quaternion.LookRotation(nav_.velocity.normalized), 
                        8 * Time.deltaTime);
                }
                nav_.SetDestination(target.transform.position);
            }
            else if (!useSkill_ &&  nav_.enabled && DistanceBetweenTarget() <= stopDistance)
                nav_.enabled = false;
            else if (!useSkill_ && !nav_.enabled && DistanceBetweenTarget() > stopDistance) 
                nav_.enabled = true;
        }
        else nav_.enabled = false;
    } 

    // Verifica se existe alguma skill naquele momento e distância que possa ser usada
    private void CallSkill()
    {
        timerToCountUpdate_ -= Time.deltaTime;

        if (!useSkill_ && timerToCountUpdate_ <= 0)
        {
            for (int i = 0; i < skillStats.Length; i++)
            {
                if (i == skillStats[i].skillNumber && DistanceBetweenTarget() <= skillStats[i].skillDistance && monsterStamina >= attack_.attackStats[i].staminaCost)
                {
                    nav_.enabled = false;
                    useSkill_ = true;
                    timerToCountUpdate_ = timerUpdate;
                    animation_.NoMovableAttack(skillStats[i].skillNumber);
                    Debug.Log("Chamou a animação!");

                    DecrementStamina(attack_.attackStats[i].staminaCost);

                    return;
                }
            }
            Debug.Log("Não Chamou a animação!");
        }
    } 

    //Realiza uma ação quando a Animação de ataque é finalizada
    private void WaitForAnimationOver()
    {
        if (useSkill_)
        {
            if (!animation_.IsPlayAttackAnimation())
            {
                StartCoroutine(WaitToActAgain());
            }
        }
    } 
    
    //Um timer que espera um tempo até o monstro poder atacar novamente
    private IEnumerator WaitToActAgain()
    {
        yield return new WaitForSeconds(waitTimer);
        useSkill_ = false;
        FollowTarget();
        yield break;
    } 
    #endregion

    #region funções auxiliares
    private float Velocity()
    {
        Vector3 speed_ = (transform.position - previousVelocity_) / Time.deltaTime;
        previousVelocity_ = transform.position;
        return speed_.magnitude;
    }

    private float DistanceBetweenTarget() 
    {
        return Vector3.Distance(transform.position, target.transform.position);   
    }

    public override void DecrementLife(float decrement)
    {
        base.DecrementLife(decrement);

        if (iaState != State.Batlle)
            iaState = State.Batlle;
    }
    #endregion

    public enum State
    {
        Walk,
        Batlle,
    }

    [System.Serializable]
    public struct SkillsAndDistance
    {
        [Range(0, 3)]
        [Tooltip("Qual input da skill será usado")]
        public int skillNumber;
        [Range(1, 30)]
        [Tooltip("O quão perto o monster deve estar para que seja necessário usar essa skill")]
        public float skillDistance;
    }
}
