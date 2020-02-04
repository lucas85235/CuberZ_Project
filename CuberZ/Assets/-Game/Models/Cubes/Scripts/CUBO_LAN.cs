using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUBO_LAN : MonoBehaviour {

	CUB_O CUB_O;
	Rigidbody RB;
	BoxCollider BX;
	Vector3 POS;
	int INDEX;

	[HideInInspector] public bool RESET;
	[HideInInspector] public bool PHYSICS;

	void Start () {
		POS = transform.position;
		CUB_O = transform.Find ("CUB_O").GetComponent<CUB_O> ();
		RB = GetComponent<Rigidbody> ();
		BX = GetComponent<BoxCollider> ();
	}

	void Update () {
		if (!PHYSICS) {
			if (Input.GetKeyDown (KeyCode.Return) && !CUB_O.MATERIALIZAR) {
				CUB_O.MATERIALIZAR = true;
				CUB_O.GET_COLORS ();
			}
			if (Input.GetKeyDown (KeyCode.Space) && CUB_O.MATERIALIZAR) {
				GRAVITY ();
				RESET = false;
				PHYSICS = true;
			}
		}
		if (RESET) RESETAR ();
	}
	void OnTriggerEnter () {
		CUB_O.ADD_PHYSICS ();
		RB.isKinematic = true;
		GameObject FX = Instantiate (Resources.Load<GameObject> ("FX/FX"));
		FX.transform.position =transform.position + Vector3.up / 2;
	}
	void GRAVITY () {
		RB.isKinematic = false;
		BX.size = new Vector3 (6, 6, 6);
	}
	void RESETAR () {
		transform.position = Vector3.MoveTowards (transform.position, POS, 0.8f);
		BX.size = new Vector3 (2, 2, 2);
	}
}