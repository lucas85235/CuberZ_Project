using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EYE : MonoBehaviour {

	SkinnedMeshRenderer SK;

	public float WAIT;
	public float TIME;
	public float TIME2;
	public float MORPH;

	public int INDEX;

	public bool OPEN;

	void Start () {

		SK = GetComponent<SkinnedMeshRenderer> ();
		MORPH = 0;
	}

	void Update () {

		SK.SetBlendShapeWeight (0, Mathf.Lerp (SK.GetBlendShapeWeight (0), MORPH,0.4f));

		if (INDEX < 2) {
			if (TIME < WAIT) {
				TIME += 1f;
			} else {
				if (MORPH <=1) {
					MORPH = 100;
					WAIT = 3;
				} else {
					MORPH = 0;
					WAIT = 50;
				}
				INDEX++;
				TIME = 0;
			}
		}
		if (INDEX >= 2) {
			if (TIME2 < WAIT*2) {
				TIME2 += 1;
			} else {
				INDEX = 0;
				TIME2 = 0;
			}
		}

	}
}