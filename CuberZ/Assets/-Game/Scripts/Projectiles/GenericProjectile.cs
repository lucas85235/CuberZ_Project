using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GenericProjectile : MonoBehaviour
{

    /// <summary>
    /// Predefinições
    /// 
    /// Pra ir apenas RETO basta:
    /// Fall Down Speed 0
    /// 
    /// Para uma transição suave para baixo:
    /// Manter Speed e FallDownSpeed em uma proporção de 1 para speed e 0.02 para fallDownSpeed
    /// 
    /// 
    /// 
    /// </summary>


    #region Variaveis Publicas
    [Header("Projectile Configuration")]
    [Range(0.1f, 10)] [Tooltip("Após quanto tempo o projetil sumirá")] public float lifeTime;
    [Range(0, 10)] [Tooltip("Após quanto tempo o projétil irá começar a cair")] public float fallDownTimer;
    [Tooltip("Velocidade do projétil")] public float speed;
    [Tooltip("Velocidade da decida do projétil")] public float fallDownSpeed;
    [Tooltip("Define o comportamento do objeto dependendo de como foi instanciado.")] public state objState;
    #endregion

    #region Variaveis Privadas
    private Rigidbody myBody_;
    private float timerLifeTime_;
    private float timerFallDown_;
    private bool canDie_;
    private bool canFallDown_;
    #endregion


    public enum state
    {
        [Tooltip("Marcar quando o objeto for ser destruido no final")]
        Instantiate,
        [Tooltip("Marcar caso o objeto for ser usado em pooling")]
        Pooling
    }


    #region Funções MonoBehaviour
    void Start()
    {
        Initialization();
    }

    private void OnEnable()
    {
        Initialization();
    }

    void Update()
    {
        AnalyticsProjectile();
    }

    private void FixedUpdate()
    {
        PhysicsProjectile();
    }
    #endregion


    #region Funções projétil

    private void PhysicsProjectile()
    {
        if (!canDie_)
            myBody_.velocity = (canFallDown_) ? (transform.forward * speed) + (new Vector3(0, -fallDownSpeed, 0)) : transform.forward * speed;
    }

    private void AnalyticsProjectile()
    {
        timerLifeTime_ -= Time.deltaTime;
        timerFallDown_ -= Time.deltaTime;

        canDie_ = (timerLifeTime_ <= 0) ? true : false;
        canFallDown_ = (timerFallDown_ <= 0) ? true : false;

        if (canDie_)
        {
            if (objState == state.Instantiate) Destroy(transform.gameObject);
            else if (objState == state.Pooling) transform.gameObject.SetActive(false);
        }
    }

    private void Initialization()
    {
        myBody_ = GetComponent<Rigidbody>();
        timerLifeTime_ = lifeTime;
        timerFallDown_ = fallDownTimer;
        canDie_ = false;
        canFallDown_ = false;
        myBody_.useGravity = false;
    }

    #endregion
}
