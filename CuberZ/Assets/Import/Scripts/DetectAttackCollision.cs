using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAttackCollision : MonoBehaviour 
{
	private GameObject hitPrefab;
	private float hitCoolDown = 1.2f; // ajustar dependendo do attack
	private bool initAttack = false;
	private bool canLookAt = true;

	public float adjustHitPosition = 2.5f;

	private void Start() 
	{
		hitPrefab = Resources.Load<GameObject>("FX/HIT");
	
	}
	
	void OnTriggerEnter (Collider other) 
	{
		if (other.tag == "Enemy")
		{
			if (!initAttack && IsAttacking())
				StartCoroutine(DamageBehaviour(other));
		}
	}

	void OnTriggerStay (Collider other) 
	{
		if (other.tag == "Enemy") 
		{
			Debug.Log(other.name);

			if (!initAttack && IsAttacking())
				StartCoroutine(DamageBehaviour(other));
		}
	}

	IEnumerator DamageBehaviour(Collider other) 
	{
		initAttack = true;
		
		if (canLookAt) 
		{
			other.transform.LookAt (transform);
			canLookAt = false;
		}
		
		other.GetComponent<Animator> ().SetTrigger ("HIT");

		// mudar para decrementar a vida do enemy ista decrementa a vida do player
		transform.parent.GetComponent<MonsterBase>().DecrementLife(
			transform.parent.GetComponent<AttackManager>().attackStats[
				transform.parent.GetComponent<MonsterBase>().currentAttackIndex
					].baseDamage);

		SpawHitEffect (other.transform);
		yield return new WaitForSeconds(hitCoolDown);

		initAttack = false;
		canLookAt = true;
	}

	void SpawHitEffect(Transform objectTransform) 
	{
		if (!GameObject.Find("Hit_FX")) 
		{
			GameObject hitEffect = Instantiate(hitPrefab);
			hitEffect.transform.position = objectTransform.position + (Vector3.up * adjustHitPosition);
			hitEffect.transform.parent = objectTransform;
			hitEffect.name = "Hit_FX";
			Destroy (hitEffect, 1);
		}
	}

	private bool IsAttacking() 
	{
		return transform.parent.GetComponent<MonsterBase>().isAttacking;
	}
}
