using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAttackCollision : MonoBehaviour 
{
	[SerializeField] private GameObject hitNumericPrefab_;
	[SerializeField] private GameObject hitSimplePrefab_;
	[SerializeField] private GameObject hitSimpleRedPrefab_;

	private AttackManager.AttackStats currentAttackStats;

	private IAAbstraction enemy_;
	private AttackManager attack_;
	private MonsterBase monster_;
	private LevelManager levelManager_;
	private Transform thisKubber_;
	private Collider thisCollider_;

	private bool initAttack_ = false;
	private bool damagePerSecond_ = false;
	private bool isAttackingInStay_ = false;
	private bool inStayCollision = false;

	public float adjustHitPosition = 2.5f;

	// Pronto
	// 1 limpar o cogigo
	// 3 ajustar ataque iniciado no stay colluider
	// 2 ajustar a attack com o tempo de animação

	// Falta
	// 4 ajustar para o attack ser dado apos o tempo atual ser maior que o min e menor que o maximo
	// 5 ajustar attack de projetil 
	// 6 ajustar attack de dano por segundo

	private void Start() 
	{
		thisKubber_ = transform.parent;

		levelManager_ = thisKubber_.GetComponent<LevelManager>();
	}

	public void UpdateCurrentAttackStats(AttackManager.AttackStats newStats) 
	{
		currentAttackStats.attackName = newStats.attackName;

        currentAttackStats.baseDamage = newStats.baseDamage;
        currentAttackStats.staminaCost = newStats.staminaCost;

        currentAttackStats.attackCoolDown = newStats.attackCoolDown;
        currentAttackStats.damagePerSecond = newStats.damagePerSecond;
        currentAttackStats.startDamageTime = newStats.startDamageTime;
        currentAttackStats.endDamageTime = newStats.endDamageTime;
        currentAttackStats.attackAnimationTime = newStats.attackAnimationTime;

        currentAttackStats.canMove = newStats.canMove;
        currentAttackStats.canStartInStay = newStats.canStartInStay;
        currentAttackStats.isDamagePerSencond = newStats.isDamagePerSencond;
        currentAttackStats.isProjectileAttack = newStats.isProjectileAttack;

        currentAttackStats.attackEffect = newStats.attackEffect;
        currentAttackStats.attackTypes = newStats.attackTypes;
	}

	#region Fluxo de colisão 
	private void OnTriggerEnter (Collider other) 
	{
		if (other.tag == "Enemy")
		{
			if (!initAttack_ && IsAttacking() && !currentAttackStats.isProjectileAttack)
				StartCoroutine(AttackEnterBehaviour(other));
		}
		// Detectar Colisão de Projeteis Fica No GenericProjectile.cs
	}

	private void OnTriggerStay (Collider other) 
	{
		if (!currentAttackStats.isDamagePerSencond)
		{	
			if (other.tag == "Enemy" && currentAttackStats.canStartInStay) 
			{
				if (!initAttack_ && IsAttacking() && !isAttackingInStay_) 
				{
					StartCoroutine(AttackInStayBehaviour(other));
				}
				inStayCollision = true;
			}
			else inStayCollision = false;
		}
	}

	private void OnTriggerExit(Collider other) 
	{
		if (other.tag == "Enemy" && damagePerSecond_) 
		{
			damagePerSecond_ = false;
			inStayCollision = false;
		}
	}
	#endregion
	
	public IEnumerator AttackEnterBehaviour(Collider other) 
	{
		initAttack_ = true;

		enemy_ = other.GetComponent<IAAbstraction>();
		monster_ = thisKubber_.GetComponent<MonsterBase>();
		attack_ = thisKubber_.GetComponent<AttackManager>();

		int damage = attack_.attackStats[monster_.currentAttackIndex].baseDamage;

		enemy_.DecrementLife(damage);
		other.GetComponent<AnimationBase>().ActiveHit();
		ChoiceAttackEffect(other.transform, attack_);
		NumericDamageEffect(other.transform, damage);

		yield return new WaitForSeconds(currentAttackStats.attackAnimationTime);

		initAttack_ = false;
		isAttackingInStay_ = false;
	}

	private IEnumerator AttackInStayBehaviour(Collider other) 
	{
		initAttack_ = true;
		isAttackingInStay_ = true;

		enemy_ = other.GetComponent<IAAbstraction>();
		monster_ = thisKubber_.GetComponent<MonsterBase>();
		attack_ = thisKubber_.GetComponent<AttackManager>();

		// se e um attack por segundo		
		// adicionar eventos para setar o tempo em que o dano pode ser recebido

		yield return new WaitForSeconds(currentAttackStats.startDamageTime);

		if (inStayCollision) 
		{
			int damage = attack_.attackStats[monster_.currentAttackIndex].baseDamage;

			other.GetComponent<AnimationBase>().ActiveHit();
			enemy_.DecrementLife(damage);

			// se o inimigo estiver morto 
			// entregar a experiencia a este kubber

			// if (enemy_.)

			ChoiceAttackEffect(other.transform, attack_);
			NumericDamageEffect(other.transform, damage);			
		}

		yield return new WaitForSeconds(currentAttackStats.attackAnimationTime - currentAttackStats.startDamageTime);

		initAttack_ = false;
		isAttackingInStay_ = false;
	}

	public void ProjectileAttackDamage(Collider other) 
	{
		enemy_ = other.GetComponent<IAAbstraction>();
		monster_ = thisKubber_.GetComponent<MonsterBase>();
		attack_ = thisKubber_.GetComponent<AttackManager>();

		int damage = attack_.attackStats[monster_.currentAttackIndex].baseDamage;

		enemy_.DecrementLife(damage);
		other.GetComponent<AnimationBase>().ActiveHit();
		ChoiceAttackEffect(other.transform, attack_);
		NumericDamageEffect(other.transform, damage);
		
		isAttackingInStay_ = false;
	}

	private bool IsAttacking() 
	{
		return transform.parent.GetComponent<MonsterBase>().isAttacking;
	}

	#region Effect Manager
	public void NumericDamageEffect(Transform spawPosition, int damage) 
	{
		GameObject damageEffect = Instantiate(hitNumericPrefab_);
		damageEffect.transform.position = spawPosition.position + (Vector3.forward * (
			Random.Range(0, 1) == 1 ? 
				Random.Range(-1, -adjustHitPosition) : Random.Range(1, adjustHitPosition))
		);
		
		damageEffect.transform.SetParent(spawPosition);
		damageEffect.GetComponent<NumericDamageEffect>().SetUpEffect(damage);
		Destroy(damageEffect, 1f);	
	}

	public void ChoiceAttackEffect(Transform spawInTransform, AttackManager attack)
	{
		switch (attack.GetAttackEffect(monster_.currentAttackIndex))
		{
			case AttackManager.Lineage.Lava:
				SpawHitEffect(spawInTransform, hitSimpleRedPrefab_);
				break;
			case AttackManager.Lineage.Original:
				SpawHitEffect(spawInTransform, hitSimplePrefab_);
				break;
			default:
				SpawHitEffect(spawInTransform, hitSimplePrefab_);
				break;
		}
	}

	public void SpawHitEffect(Transform objectTransform, GameObject effect) 
	{
		// impede que o efeito seja instanciado caso haja outro efeito no objeto em que ele sera instanciado

		if (!GameObject.Find("Hit_FX"))
		{
			GameObject hitEffect = Instantiate(effect);
			hitEffect.transform.position = objectTransform.position + (Vector3.up * adjustHitPosition);
			hitEffect.transform.SetParent(objectTransform);
			hitEffect.name = "Hit_FX";
			Destroy (hitEffect, 1);
		}
	}
	#endregion	
}
