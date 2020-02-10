using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour 
{
	private NavMeshAgent nav_;

	[SerializeField] 
	private Transform target_;

	public float minDistance = 2.2f;

	void Start () 
	{
		nav_ = GetComponent<NavMeshAgent> ();
	}
	
	void Update () 
	{
		if (target_ != null)
			if (MinDistanceFollow())
				nav_.destination = target_.position;
	}

	private void SetTarget(Transform newTarget) 
	{
		target_ = newTarget;
	}

	private bool MinDistanceFollow() 
	{
		if (Vector3.Distance(target_.position, transform.position) < minDistance)
			return true;
		else 
			return false;
	}
}
