using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAttackCollision : MonoBehaviour 
{
	[SerializeField] private GameObject hitNumericPrefab_;
	[SerializeField] private GameObject hitSimplePrefab_;
	[SerializeField] private GameObject hitSimpleRedPrefab_;

	private Transform thisKubber_;
	private IAManagerDefault enemy_;
	private AttackManager attack_;
	private MonsterBase monster_;

	private float hitCoolDown_ = 1.2f; // ajustar dependendo do attack
	private float waitForDamagePerSecond = 0.2f; // padrão mas deve ser ajustado dinamicamente
	private bool initAttack_ = false;
	private bool canLookAt_ = true;
	private bool damagePerSecond = false;

	public float adjustHitPosition = 2.5f;

	private void Start() 
	{
		thisKubber_ = transform.parent;
	}
	
	#region Fluxo de colisão 
	private void OnTriggerEnter (Collider other) 
	{
		if (other.tag == "Enemy" && !damagePerSecond)
		{
			if (!initAttack_ && IsAttacking())
				StartCoroutine(DamageBehaviour(other));
		}
		if (other.tag == "Projectile") 
		{
			// ativar efeito do projetiu e destruir projetil
		}
	}

	private void OnTriggerStay (Collider other) 
	{
		if (other.tag == "Enemy" && damagePerSecond) 
		{
			if (!initAttack_ && IsAttacking())
				StartCoroutine(DamageBehaviour(other));
		}
	}

	private void OnTriggerExit(Collider other) 
	{
		if (other.tag == "Enemy" && damagePerSecond) 
		{
			damagePerSecond = false;
		}
	}
	#endregion
	
	private IEnumerator DamageBehaviour(Collider other) 
	{
		initAttack_ = true;
		
		other.GetComponent<AnimationBase>().ActiveHit();

		enemy_ = other.GetComponent<IAManagerDefault>();
		monster_ = thisKubber_.GetComponent<MonsterBase>();
		attack_ = thisKubber_.GetComponent<AttackManager>();

		// e um attack por segundo?
		// mudar attackEffect de array para variavel unica
		
		// adicionar caroutine para esperar o tempo em que o dano pode ser recebido

		// if ()  { verificar se e um attack de longa distancia / de projetil 

		int damage = attack_.attackStats[monster_.currentAttackIndex].baseDamage;

		enemy_.DecrementLife(damage);

		ChoiceAttackEffect(other.transform);

		GameObject damageEffect = Instantiate(hitNumericPrefab_);
		damageEffect.transform.position = other.transform.position + (Vector3.forward * (
			Random.Range(0, 1) == 1 ? 
				Random.Range(-1, -adjustHitPosition) : Random.Range(1, adjustHitPosition))
		);
		
		damageEffect.transform.parent = other.transform;
		damageEffect.GetComponent<NumericDamageEffect>().SetUpEffect(damage);
		Destroy(damageEffect, 1f);	
		// }

		yield return new WaitForSeconds(hitCoolDown_);

		initAttack_ = false;
		canLookAt_ = true;
	}

	private void ChoiceAttackEffect(Transform spawInTransform) 
	{
		switch (attack_.GetAttackEffect(monster_.currentAttackIndex))
		{
			case AttackManager.Lineage.Lava:
				SpawHitEffect(spawInTransform, hitSimpleRedPrefab_);
				break;
			case AttackManager.Lineage.Original:
				SpawHitEffect(spawInTransform, hitSimplePrefab_);
				break;
		}
	}

	public void SpawHitEffect(Transform objectTransform, GameObject effect) 
	{
		if (!GameObject.Find("Hit_FX"))
		{
			GameObject hitEffect = Instantiate(effect);
			hitEffect.transform.position = objectTransform.position + (Vector3.up * adjustHitPosition);
			hitEffect.transform.parent = objectTransform;
			hitEffect.name = "Hit_FX";
			Destroy (hitEffect, 1);
		}
	}

	private bool IsAttacking() 
	{
		// verificar se e um ataque de longa distancia se for retornar false
		return transform.parent.GetComponent<MonsterBase>().isAttacking;
	}
}
