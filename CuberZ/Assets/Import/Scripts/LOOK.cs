using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOK : MonoBehaviour {

	GameObject CAM;

	void Start () {
		CAM = GameObject.Find ("Main Camera");
	}

	void Update () {
		gameObject.GetComponent<SpriteRenderer> ().enabled = Input.GetKey (KeyCode.LeftShift);
		transform.LookAt (CAM.transform);
	}
}