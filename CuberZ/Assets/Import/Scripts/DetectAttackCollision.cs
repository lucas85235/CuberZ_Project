using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAttackCollision : MonoBehaviour 
{
	void OnTriggerEnter (Collider collider) 
	{
		if (collider.gameObject.tag == "enemy")
		{
			collider.gameObject.GetComponent<Rigidbody> ().AddForce (GameObject.Find ("J1").transform.forward * 150);
			collider.gameObject.GetComponent<Animator> ().SetTrigger ("HIT");
			collider.gameObject.transform.LookAt (transform);
			HitEffect (collider.gameObject.transform);
		}
	}

	void OnTriggerStay (Collider collider) 
	{
		if (collider.gameObject.tag == "enemy") 
		{
			collider.gameObject.GetComponent<Rigidbody> ().AddForce (GameObject.Find ("J1").transform.forward * 150);
			collider.gameObject.GetComponent<Animator> ().SetTrigger ("HIT");
			collider.gameObject.transform.LookAt (transform);
			HitEffect (collider.gameObject.transform);
		}
	}

	void HitEffect(Transform objectTransform) 
	{
		if (!GameObject.Find ("FX")) 
		{
			GameObject hitEffect = Instantiate (Resources.Load<GameObject> ("FX/HIT"));
			hitEffect.transform.position = objectTransform.position + Vector3.up * 5;
			hitEffect.transform.parent = objectTransform;
			hitEffect.name = "FX";
			Destroy (hitEffect, 1);
		}
	}
}