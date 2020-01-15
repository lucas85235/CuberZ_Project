using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour {

	NavMeshAgent NAV;

	void Start () {
		NAV = GetComponent<NavMeshAgent> ();
	}

	// Update is called once per frame
	void Update () {
		NAV.destination = GameObject.Find ("J1").transform.position;
	}
}