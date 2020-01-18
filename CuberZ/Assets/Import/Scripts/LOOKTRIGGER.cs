using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOKTRIGGER : MonoBehaviour {

	GameObject[] ENE;
	GameObject CAM;

	void Start () {
		ENE = GameObject.FindGameObjectsWithTag ("enemy");
		CAM = GameObject.Find ("Main Camera");
	}

	void Update () {

		foreach (GameObject J in ENE) {
			if (J) {
				if (Vector3.Distance (transform.position, J.transform.position) <20) {
					if (Input.GetKey (KeyCode.LeftShift)) {
						CAM.transform.LookAt (J.transform);
					}
				}
			}
		}

	}

}