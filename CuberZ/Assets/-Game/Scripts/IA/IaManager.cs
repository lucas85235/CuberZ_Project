using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IaManager : MonoBehaviour
{

    #region Variáveis Públicas
    [Header("Variáveis Alocáveis")]
    public Transform target;

    [Header("Variáveis de Controle")] [Tooltip("Tenta achar o Target sozinho")]
    public bool findTargetAutomatic = true;
    [Range(1,10)] [Tooltip("O quão distante ele pode ir caminhando normalmente")]
    public float walkRadius = 10;
    [Range(2, 30)] [Tooltip("Evita que o Kubber entre um dentro do outro")]
    public float stopDistance = 10;
    [Range(0,3f)] [Tooltip("Tempo em que o Kubber fica parado após usar uma skill")]
    public float waitTimer = 3;
    [Range(0,10f)] [Tooltip("A cada quantos segundos o Kubber procura um skill para se utilizar")]
    public float timerUpdate = 3;
    [Tooltip("Modos da IA")]
    public State iaState;
    [Tooltip("Controla distÂncia e qual skill será usada.")]
    public SkillsAndDistance[] skillStats;
    #endregion

    #region Variáveis Privadas
    private bool useSkill_;
    private NavMeshAgent agent_;
    private Animator anim_;
    private Vector3 firstPos_;
    private bool canControl_;
    private bool goOut_;
    private float timerToCountUpdate_;
    private Vector3 previousVelocity_;
    #endregion

    #region Métodos MonoBehaviour

    private void Awake()
    {
        agent_ = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        firstPos_ = transform.position;
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (iaState == State.Walk) ControlWalkState();

        if (iaState == State.Batlle) ControlBattleState(); 
    }

    #endregion

    #region Walk State

    private float Velocity()
    {
        Vector3 speed_ = (transform.position - previousVelocity_) / Time.deltaTime;
        previousVelocity_ = transform.position;
        return speed_.magnitude;

    }

    private void GoBackToOrigin()
    {

        agent_.SetDestination(firstPos_);
        goOut_ = false;
        StartCoroutine(Timer());

    } //Função que faz o Kubber voltar para o local de origem.

    private void ControlWalkState()
    {
        GetComponent<AnimationBase>().AnimationSpeed(Velocity(),Velocity());
        if (canControl_) StartCoroutine(ControlWalkStateCoroutine());

    } //Função que controla todo o enum de andar.

    private void PlaySomeRandomAnimation()
    {
        GetComponent<AnimationBase>().ExtraAnimationOne();
    } //Função que controla para o Kubber tocar animação aleatória de vez em quando.

    private IEnumerator ControlWalkStateCoroutine()
    {

        if (goOut_)
        {
            // if (Random.Range(0, 10) != 0) 
            //  else PlaySomeRandomAnimation();
            GoBackToOrigin();

        }

        else FindSomeWhereToWalk();

        canControl_ = false;
        yield break;

    } // Função que organiza o tempo de execução.

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(Random.Range(4, 6f));
        canControl_ = true;
        yield break;

    } // Função que server como um Timer.

    private void FindSomeWhereToWalk()
    {

        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        agent_.SetDestination(finalPosition);
        goOut_ = true;
        StartCoroutine(Timer());
    } // Função que controla para onde o Kubber vai andar.
    #endregion

    #region Battle State

    private void ControlBattleState()
    {
        GetComponent<AnimationBase>().AnimationSpeed(Velocity(), Velocity());
        FollowTarget();
        CallSkill();
        WaitForAnimationOver();

    }

    private void FollowTarget()
    {

        if (findTargetAutomatic)
        {
            if(PlayerController.instance.moster)  target = PlayerController.instance.moster.transform;
        }

        if (agent_.enabled && !useSkill_ && target && Vector3.Distance(transform.position, target.transform.position) > stopDistance)
        {
            agent_.enabled = true;
            if (agent_.velocity != Vector3.zero) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(agent_.velocity.normalized), 8 * Time.deltaTime);
            agent_.SetDestination(target.transform.position);

        }

        else if (agent_.enabled && !useSkill_ && target && Vector3.Distance(transform.position, target.transform.position) <= stopDistance) agent_.enabled = false;

        if (!target) agent_.enabled = false;

        else if (target && !useSkill_ && !agent_.enabled && Vector3.Distance(transform.position, target.transform.position) > stopDistance) agent_.enabled = true;

      

    } // Usada para quando o Kubber está indo atrás do target para atacar 

    private void CallSkill()
    {
        timerToCountUpdate_ -= Time.deltaTime;

        if (!useSkill_ && timerToCountUpdate_ <= 0)
        {
            for (int i = 0; i < skillStats.Length; i++)
            {
                if (i == skillStats[i].skillNumber && Vector3.Distance(transform.position, target.transform.position) <= skillStats[i].skillDistance)
                {
                    agent_.enabled = false;
                    useSkill_ = true;
                    timerToCountUpdate_ = timerUpdate;
                    GetComponent<AnimationBase>().NoMovableAttack(skillStats[i].skillNumber);
                    break;

                }

            }
        }

    } // Verifica se existe alguma skill naquele momento e distância que possa ser usada

    private void WaitForAnimationOver()
    {
        if (useSkill_)
        {
            if (!GetComponent<AnimationBase>().IsPlayAttackAnimation())
            {
                StartCoroutine(WaitToActAgain());
            }

        }

    } //Realiza uma ação quando a Animação de ataque é finalizada

    private IEnumerator WaitToActAgain()
    {
        yield return new WaitForSeconds(waitTimer);
        useSkill_ = false;
        FollowTarget();
        yield break;
    } //Um timer que espera um tempo até o monstro poder atacar novamente

    #endregion

    #region Enum e Struct

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

    #endregion

}
