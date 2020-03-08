using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAManagerDefault : IAAbstraction
{
    [Header("Variáveis Alocáveis")]
    public Transform target;

    [Header("Variáveis de Controle")] 

    [Range(1,10)] [Tooltip("O quão longe pode ir caminhando")]
    public float walkRadius = 10;

    [Range(2, 30)] [Tooltip("Evita que o Kubber entre em outro")]
    public float stopDistance = 10; 

    [Range(0,3f)] [Tooltip("Tempo parado após usar uma skill")]
    public float waitTimer = 3;
    
    [Range(0,10f)] [Tooltip("Tempo ate fazer a troca de skills randomicamente")]
    public float timerUpdate = 3;

    [Tooltip("Estado atual da IA")] public State currentIaState;
    
    [Tooltip("Controla a distancia e skill que será usada.")]
    public SkillsAndDistance[] skillStats;

    private NavMeshAgent nav_;
    private AnimationBase animation_;
    private HudWorldStats worldHud_;
    private AttackManager attack_;

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

        monsterStamina = maxStamina;
        monsterLife = maxLife;

        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (!isDead) 
        {
            if (currentIaState == State.Walk)
            {
                ControlWalkState();
            }
            else if (currentIaState == State.Batlle)
            {
                ControlBatlleState();
            }

            RegenStamina();
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if (Input.GetKey(KeyCode.E) && other.tag == "Player") 
        {
            if (currentIaState == State.Walk) 
            {
                currentIaState = State.Batlle;
            }    
        }
    }
    #endregion

    #region Walk State
    protected override void ControlWalkState()
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
    
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(Random.Range(4, 6f));
        canControl_ = true;
        yield break;
    }
    #endregion

    #region Battle State
    protected override void ControlBatlleState()
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

                    DecrementStamina(attack_.attackStats[i].staminaCost);

                    return;
                }
            }
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
    
    // Um timer que espera um tempo até o monstro poder atacar novamente
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
    #endregion

    #region Life and Stamina increment and decrement 
    public override void IncrementStamina(float increment)
    {
        monsterStamina += increment;

        if (monsterStamina > maxStamina)
        {
            monsterStamina = maxStamina;
        }

        worldHud_.HudUpdateStamina(monsterStamina, maxStamina);
    }

    public override void DecrementStamina(float decrement)
    {
        monsterStamina -= decrement;

        if (monsterStamina <= 0)
        {
            monsterStamina = 0;
            Debug.Log("You not have stamina!");
        }

        worldHud_.HudUpdateStamina(monsterStamina, maxStamina);
    }

    public override void IncrementLife(float increment)
    {
        monsterLife += increment;

        if (monsterLife > maxLife)
        {
            monsterLife = maxLife;
        }

        worldHud_.HudUpdateLife(monsterLife, maxLife);

        if (isDead && monsterLife > 0) 
        {
            animation_.ExitDeathState();
            nav_.enabled = true;
        }
    }

    public override void DecrementLife(float decrement)
    {
        monsterLife -= decrement;

        if (monsterLife <= 0)
        {
            isDead = true;
            animation_.PlayDeathState();

            nav_.enabled = false;                
            this.StopAllCoroutines();

            Debug.Log("Life < 0, You Are Dead!");
        }

        worldHud_.HudUpdateLife(monsterLife, maxLife);

        if (currentIaState != State.Batlle && !isDead)
            currentIaState = State.Batlle;
    }
    #endregion

    [System.Serializable]
    public struct SkillsAndDistance
    {
        [Range(0, 3)] [Tooltip("Qual input da skill será usado")]
        public int skillNumber;

        [Range(1, 30)] [Tooltip("O quão perto o monster deve estar para que seja necessário usar essa skill")]
        public float skillDistance;
    }
}
