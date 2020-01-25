using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUB_O : MonoBehaviour {

	public Texture2D MAP;
	public GameObject[] PREFABS;
	public Color[] COLORS;
	public float CUBESCALE;
	public bool MATERIALIZAR;
	public bool DESMATERIALIZAR;
	int INDEX;

	public void Start () {
		CUBESCALE = 0.6f;
		COLORS = new Color[27];
	}
	public void Update () {
		if (MATERIALIZAR) {
			MATERIALIZE ();
		}
		if (DESMATERIALIZAR) DESMATERIALIZE ();
	}
	public void GET_COLORS () {
		for (int i = 0; i < MAP.width; i++) {
			for (int j = 0; j < MAP.height; j++) {
				COLORS[INDEX] = MAP.GetPixel (i, j);
				INDEX++;
			}
		}
		INDEX = 0;
		GENERATE_CUBE (transform.position);
	}
	public void GENERATE_CUBE (Vector3 POS) {
		for (int X = 0; X < 3; X++) {
			for (int Y = 0; Y < 3; Y++) {
				for (int Z = 0; Z < 3; Z++) {
					if (COLORS[INDEX] == Color.black) ADD_CUBE (new Vector3 (POS.x + CUBESCALE * X, POS.y + CUBESCALE * Y, POS.z + CUBESCALE * Z), 0, "BASE");
					if (COLORS[INDEX] == Color.blue) ADD_CUBE (new Vector3 (POS.x + CUBESCALE * X, POS.y + CUBESCALE * Y, POS.z + CUBESCALE * Z), 1, "BORDA");
					if (COLORS[INDEX] == Color.red) ADD_CUBE (new Vector3 (POS.x + CUBESCALE * X, POS.y + CUBESCALE * Y, POS.z + CUBESCALE * Z), 2, "BOTAO");
					if (COLORS[INDEX] == Color.white) ADD_CUBE (new Vector3 (POS.x + CUBESCALE * X, POS.y + CUBESCALE * Y, POS.z + CUBESCALE * Z), 3, "NUCLEO");
					INDEX++;
				}
			}
		}
	}
	public void ADD_CUBE (Vector3 POS, int ID, string NAME) {
		GameObject CB = Instantiate (PREFABS[ID]);
		CB.transform.position = POS;
		CB.transform.parent = transform;
		CB.transform.localScale = Vector3.zero;
		CB.name = NAME;
		CB.GetComponent<Renderer> ().material.SetFloat ("_Progress", 0f);
		CB.tag = "cb";
	}
	public void MATERIALIZE () {
		GameObject[] ALL_CUBES = GameObject.FindGameObjectsWithTag ("cb");
		for (int CB = 0; CB < ALL_CUBES.Length; CB++) {
			ALL_CUBES[CB].transform.localScale = Vector3.MoveTowards (ALL_CUBES[CB].transform.localScale, new Vector3 (1, 1, 1), 0.1f);
			ALL_CUBES[CB].GetComponent<Renderer> ().material.SetFloat ("_Progress", Mathf.Lerp (ALL_CUBES[CB].GetComponent<Renderer> ().material.GetFloat ("_Progress"), 1f, 0.1f));
		}
	}
	public void DESMATERIALIZE () {
		GameObject[] ALL_CUBES = GameObject.FindGameObjectsWithTag ("cb");
		for (int CB = 0; CB < ALL_CUBES.Length; CB++) {
			ALL_CUBES[CB].transform.localScale = Vector3.MoveTowards (ALL_CUBES[CB].transform.localScale, new Vector3 (1, 1, 1), 0.1f);
			ALL_CUBES[CB].GetComponent<Renderer> ().material.SetFloat ("_Progress", Mathf.Lerp (ALL_CUBES[CB].GetComponent<Renderer> ().material.GetFloat ("_Progress"), 0f, 0.1f));
		}
	}
	public void ADD_PHYSICS () {
		GameObject[] ALL_CUBES = GameObject.FindGameObjectsWithTag ("cb");
		for (int CB = 0; CB < ALL_CUBES.Length; CB++) {
			if (!ALL_CUBES[CB].GetComponent<Rigidbody> ()) ALL_CUBES[CB].AddComponent<Rigidbody> ();
			if (!ALL_CUBES[CB].GetComponent<BoxCollider> ()) ALL_CUBES[CB].AddComponent<BoxCollider> ().size = new Vector3 (3, 3, 3);
			ALL_CUBES[CB].transform.localScale = Vector3.MoveTowards (ALL_CUBES[CB].transform.localScale, new Vector3 (0.6f, 0.6f, 0.6f), 0.8f);
			ALL_CUBES[CB].transform.parent = null;
		}
		StartCoroutine (DISABLE ());
	}
	IEnumerator DISABLE () {
		GameObject[] ALL_CUBES = GameObject.FindGameObjectsWithTag ("cb");
		yield return new WaitForSeconds (1);
		for (int CBS = 0; CBS < ALL_CUBES.Length; CBS++) {
			ALL_CUBES[CBS].GetComponent<BoxCollider> ().size = new Vector3 (1, 1, 1);
		}
		yield return new WaitForSeconds (1);
		for (int CBS = 0; CBS < ALL_CUBES.Length; CBS++) {
			if (!ALL_CUBES[CBS].GetComponent<BoxCollider> ()) Destroy (ALL_CUBES[CBS].GetComponent<BoxCollider> ());
			if (!ALL_CUBES[CBS].GetComponent<Rigidbody> ()) Destroy (ALL_CUBES[CBS].GetComponent<Rigidbody> ());
		}
		yield return new WaitForSeconds (1);

		MATERIALIZAR = false;
		DESMATERIALIZAR = true;
		yield return new WaitForSeconds (.5f);
		INDEX = 0;
		DESMATERIALIZAR = false;

		for (int CBS = 0; CBS < ALL_CUBES.Length; CBS++) {
			Destroy (ALL_CUBES[CBS]);
		}
		GameObject.Find ("CUBO_LAN").GetComponent<CUBO_LAN> ().PHYSICS = false;
		GameObject.Find ("CUBO_LAN").GetComponent<CUBO_LAN> ().RESET = true;

	}

}