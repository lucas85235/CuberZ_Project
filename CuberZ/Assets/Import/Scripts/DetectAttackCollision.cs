using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAttackCollision : MonoBehaviour 
{
	private GameObject hitPrefab;
	private float hitCoolDown = 1.2f;
	private bool initAttack = false;

	public float attackForce = 5.0f;
	public float upHitPosition = 3.0f;


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
			if (!initAttack && IsAttacking())
				StartCoroutine(DamageBehaviour(other));
		}
	}

	IEnumerator DamageBehaviour(Collider other) 
	{
		initAttack = true;

		other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		other.GetComponent<Rigidbody>().freezeRotation = true;
		//other.GetComponent<Rigidbody>().AddForce(this.transform.parent.forward * attackForce);
		
		other.GetComponent<Animator> ().SetTrigger ("HIT");
		other.transform.LookAt (transform);
		
		HitEffect (other.transform);
		Debug.Log(other.name);
		transform.parent.GetComponent<MonsterBase>().DecrementLife(
			transform.parent.GetComponent<AttackManager>().attackStats[
				transform.parent.GetComponent<MonsterBase>().currentAttackIndex
					].baseDamage);
		yield return new WaitForSeconds(hitCoolDown);

		initAttack = false;
	}

	void HitEffect(Transform objectTransform) 
	{
		if (!GameObject.Find("Hit_FX")) 
		{
			GameObject hitEffect = Instantiate(hitPrefab);
			hitEffect.transform.position = objectTransform.position + (Vector3.up * upHitPosition);
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
