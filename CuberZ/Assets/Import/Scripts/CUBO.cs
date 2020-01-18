using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUBO : MonoBehaviour {

	public Vector3 TMP;
	public Vector3[] INIALPOS;
	public Material MAT;
	Vector3 PS;

	int INDEX;
	GameObject[] RES;
	bool ATV;
	bool ATV2;

	void Start () {
		INIALPOS = new Vector3[27];
		GENERATECUBE ();
	}
	public void GENERATECUBE () {
		for (int x = 0; x < 3; x++) {
			for (int y = 0; y < 3; y++) {
				for (int z = 0; z < 3; z++) {
					CUBE (new Vector3 (transform.position.x + 1.02f * z, transform.position.y + 1.02f * y, transform.position.z + +1.02f * x));
					INIALPOS[INDEX] = new Vector3 (transform.position.x + 1.02f * z, transform.position.y + 1.02f * y, transform.position.z + +1.02f * x);
					INDEX++;
				}
			}
		}
	}
	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
			ATV = true;
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			DELETA ();
		}
		RECOSTROI ();
	}
	void CUBE (Vector3 POS) {
		GameObject C = GameObject.CreatePrimitive (PrimitiveType.Cube);
		C.transform.position = POS;
		C.transform.parent = transform;
		C.AddComponent<Rigidbody> ().isKinematic = true;
		C.tag = "CUBE";
		C.GetComponent<Renderer> ().material = MAT;
	}
	void ANIM () {
		RES = GameObject.FindGameObjectsWithTag ("CUBE");
		for (int T = 0; T < RES.Length; T++) {
			RES[T].transform.Rotate (0.5f, 0.5f, 0.5f);
		}
	}
	void RECOSTROI () {
		RES = GameObject.FindGameObjectsWithTag ("CUBE");
		if (ATV) {
			for (int T = 0; T < RES.Length; T++) {
				RES[T].GetComponent<Rigidbody> ().isKinematic = true;
				RES[T].transform.position = Vector3.MoveTowards (RES[T].transform.position, INIALPOS[T], 0.6f);
				RES[T].transform.eulerAngles = new Vector3 (0, 0, 0);
			}
		}
		//if (RES==INIALPOS) ATV = false; ??????????
	}
	void DELETA () {
		RES = GameObject.FindGameObjectsWithTag ("CUBE");
		for (int T = 0; T < RES.Length; T++) {
			RES[T].GetComponent<Rigidbody> ().isKinematic = false;
		}
	}

}