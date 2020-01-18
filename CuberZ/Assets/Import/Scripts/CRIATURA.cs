using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRIATURA : MonoBehaviour {

	Animator A;
	Rigidbody R;
	GameObject CAM;
	GameObject COL;

	public float SPEED;
	public float TIME;
	public float SMOOTH;

	bool GROUND () {
		return Physics.Raycast (transform.position, -transform.up, 1f);
	}

	bool CHECK () {
		var S = A.GetCurrentAnimatorStateInfo (0);
		return S.IsName ("atk");
	}

	void Start () {
		A = GetComponent<Animator> ();
		R = GetComponent<Rigidbody> ();
		CAM = GameObject.Find ("Main Camera");
		COL = transform.Find ("COLISOR").gameObject;

	}

	void Update () {

		var S = A.GetCurrentAnimatorStateInfo (0);
		float X = Input.GetAxis ("Horizontal");
		float Z = Input.GetAxis ("Vertical");
		COL.SetActive (S.IsName ("atk"));

		if (!CHECK ()) MOVE (X, Z);

		ANI (X, Z);
		ATTAKING ();
		MOUSE ();
		if (S.IsName ("atk")) {
			R.AddForce (transform.forward * 140);
		} else {
			if (GROUND ()) R.velocity = Vector3.zero;
		}

	}
	void MOUSE () {

		if (Input.GetMouseButtonDown (0)) {
			Ray ray = CAM.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (Vector3.Distance (transform.position, hit.point) > 5f) {
					transform.LookAt (hit.point);
					A.SetTrigger ("ATK");
					/*GameObject FX = Instantiate (Resources.Load<GameObject> ("FX/CUBO"));
					FX.transform.position = hit.point + Vector3.up * 10;
					Destroy(FX,6);*/
				}
			}

		}
	}
	void MOVE (float X, float Z) {
		Vector3 input = new Vector2 (X, Z);
		Vector2 inputdir = input.normalized;
		if (inputdir != Vector2.zero) {
			float targetrotation = Mathf.Atan2 (inputdir.x, inputdir.y) * Mathf.Rad2Deg;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle (transform.eulerAngles.y, targetrotation + CAM.transform.eulerAngles.y, ref SMOOTH, TIME);
			transform.position += transform.forward * SPEED * Time.deltaTime;
		}
	}

	void ATTAKING () {
		//if (Input.GetKeyDown (KeyCode.JoystickButton2) || Input.GetMouseButtonDown (0)) A.SetTrigger ("ATK");
	}

	void ANI (float X, float Z) {
		A.SetFloat ("SPEED", X * X + Z * Z);
	}

}