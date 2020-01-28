using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRIATURA : MonoBehaviour {

	Animator animator;
	Rigidbody body;
	GameObject cameraObject;
	GameObject COL;

	public float SPEED;
	public float TIME;
	public float SMOOTH;

	bool GROUND () 
	{
		return Physics.Raycast (transform.position, -transform.up, 1f);
	}

	bool CHECK () 
	{
		var S = animator.GetCurrentAnimatorStateInfo (0);
		return S.IsName ("atk");
	}

	void Start () 
	{
		animator = GetComponent<Animator> ();
		body = GetComponent<Rigidbody> ();
		cameraObject = GameObject.Find ("Main Camera");
		COL = transform.Find ("COLISOR").gameObject;
	}

	void Update () 
	{
		var animatorState = animator.GetCurrentAnimatorStateInfo (0);

		float axisX = Input.GetAxis ("Horizontal");
		float axisY = Input.GetAxis ("Vertical");

		COL.SetActive (animatorState.IsName ("atk"));

		if (!CHECK ()) 
			MOVE (axisX, axisY);

		ANI (axisX, axisY);
		MOUSE ();

		if (animatorState.IsName ("atk")) 
		{
			body.AddForce (transform.forward * 140);
		} 
		else 
		{
			if (GROUND ()) body.velocity = Vector3.zero;
		}

	}
	
	void MOUSE () 
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			Ray ray = cameraObject.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) 
			{
				if (Vector3.Distance (transform.position, hit.point) > 5f) 
				{
					transform.LookAt (hit.point);
					animator.SetTrigger ("ATK");
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
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle (transform.eulerAngles.y, targetrotation + cameraObject.transform.eulerAngles.y, ref SMOOTH, TIME);
			transform.position += transform.forward * SPEED * Time.deltaTime;
		}
	}

	void ANI (float X, float Z) 
	{
		animator.SetFloat ("SPEED", X * X + Z * Z);
	}


}