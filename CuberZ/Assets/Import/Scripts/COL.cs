using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COL : MonoBehaviour {

	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
	void OnTriggerEnter (Collider C) {
		if (C.gameObject.tag == "enemy") {
			C.gameObject.GetComponent<Rigidbody> ().AddForce (GameObject.Find ("J1").transform.forward * 150);
			C.gameObject.GetComponent<Animator> ().SetTrigger ("HIT");
			C.gameObject.transform.LookAt (transform);
			HIT (C.gameObject.transform.position, C.gameObject);
		}
	}
	void OnTriggerStay (Collider C) {
		if (C.gameObject.tag == "enemy") {
			C.gameObject.GetComponent<Rigidbody> ().AddForce (GameObject.Find ("J1").transform.forward * 150);
			C.gameObject.GetComponent<Animator> ().SetTrigger ("HIT");
			C.gameObject.transform.LookAt (transform);
			HIT (C.gameObject.transform.position, C.gameObject);
		}
	}
	void HIT (Vector3 POS, GameObject OBJ) {
		if (!GameObject.Find ("FX")) {
			GameObject FX = Instantiate (Resources.Load<GameObject> ("FX/HIT"));
			FX.transform.position = POS + Vector3.up * 5;
			FX.transform.parent = OBJ.transform;
			FX.name = "FX";
			Destroy (FX, 1);
		}
	}
}