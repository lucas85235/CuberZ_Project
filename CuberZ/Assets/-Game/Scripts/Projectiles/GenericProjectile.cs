using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GenericProjectile : MonoBehaviour
{
    // Pra ir apenas RETO basta:
    // Fall Down Speed 0

    // Para uma transição suave para baixo:
    // Manter Speed e FallDownSpeed em uma proporção de 1 para speed e 0.02 para fallDownSpeed

    private GameObject spawnedBy = null;
    private Rigidbody rigidbody_;
    private float timerLifeTime_;
    private float timerFallDown_;
    private bool canDie_;
    private bool canFallDown_;

    [Header("Projectile Configuration")]
    public float projectileSpeed;
    public float fallDownSpeed;

    [Range(0, 100)]
    public float projectileDamage;
    [Range(0.1f, 10)] [Tooltip("Após quanto tempo o projetil sumirá")] 
    public float lifeTime;
    [Range(0, 10)] [Tooltip("Após quanto tempo o projétil irá começar a cair")] 
    public float fallDownTimer;    
    [Tooltip("Define o comportamento do objeto dependendo de como foi instanciado.")] 
    public state objState;

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
        ControlProjectile();
    }

    private void FixedUpdate()
    {
        ProjectileBehaviour();
    }
    #endregion

    #region Funções projétil
    private void ProjectileBehaviour()
    {
        if (!canDie_)
        {
            if (canFallDown_)
            {
                rigidbody_.velocity = (transform.forward * projectileSpeed) + (new Vector3(0, -fallDownSpeed, 0));
            }
            else rigidbody_.velocity = transform.forward * projectileSpeed;
        }
    }

    private void ControlProjectile()
    {
        timerLifeTime_ -= Time.deltaTime;
        timerFallDown_ -= Time.deltaTime;

        canDie_ = (timerLifeTime_ <= 0) ? true : false;
        canFallDown_ = (timerFallDown_ <= 0) ? true : false;

        if (canDie_)
        {
            if (objState == state.Instantiate) 
            {
                Destroy(transform.gameObject);
            }
            else if (objState == state.Pooling) transform.gameObject.SetActive(false);
        }
    }

    private void Initialization()
    {
        rigidbody_ = GetComponent<Rigidbody>();
        timerLifeTime_ = lifeTime;
        timerFallDown_ = fallDownTimer;
        canDie_ = false;
        canFallDown_ = false;
        rigidbody_.useGravity = false;
    }

    public void SpawnedBy(GameObject by) 
    {
        spawnedBy = by;
    }

    private  void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Enemy" || other.tag == "Monster" && spawnedBy != null) 
		{
            spawnedBy.transform.Find("DetectCollision").
                GetComponent<DetectAttackCollision>().ProjectileAttackDamage(other);
            Destroy(this.gameObject);
		}
    }
    #endregion
}
