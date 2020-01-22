using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCube : MonoBehaviour
{
    Rigidbody rb_;
    public float gravityImpact_;

    private void Awake()
    {
        rb_ = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        rb_ = GetComponent<Rigidbody>();
        rb_.useGravity = false;
        rb_.velocity = Vector3.zero;

    }

    private void OnDisable()
    {
        rb_.useGravity = false;
        rb_.velocity = Vector3.zero;
    }

    protected void PhysicsControl()
    {
        if(rb_.useGravity)
        rb_.AddForce(new Vector3(0, -gravityImpact_, 0));

    }

    private void FixedUpdate()
    {
        PhysicsControl();
    }


}
