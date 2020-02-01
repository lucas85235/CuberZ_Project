using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour 
{
	private NavMeshAgent nav_;

	[SerializeField] private Transform target_;

	public float minDistance = 2.2f;

	void Start () 
	{
		nav_ = GetComponent<NavMeshAgent> ();
		target_ = GameObject.FindGameObjectWithTag("Monster").transform;
	}
	
	void Update () 
	{
		if (Vector3.Distance(target_.position, transform.position) < minDistance)
			nav_.destination = target_.position;
	}
}