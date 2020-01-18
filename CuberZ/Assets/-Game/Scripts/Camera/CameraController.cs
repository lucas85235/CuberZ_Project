using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private RaycastHit hit;

    [Header("Player to follow")]
    [SerializeField] private Transform target_;

    [Header("Camera Properties")]
    public float sensibility = 125.0f;
    public float cameraDistance = 16.0f;
    public float adjustCollisionForward = 0.1f;
    public float smooth = 4.0f;

    private float distanceUp;
    private float minAngle = 0.9f;
    private float maxAngle = 1.1f;
    private Vector3 cameraPosition;

    void Start()
    {
        // O target deve ser um pivo no meio do personagem e não nos pés
    }

    void LateUpdate()
    {
        if(Input.GetMouseButton(1))
        {
            transform.RotateAround(target_.position, transform.up, Input.GetAxis("Mouse X") * sensibility * Time.deltaTime);
            transform.RotateAround(target_.position, transform.right, Input.GetAxis("Mouse Y") * sensibility * Time.deltaTime);

            Vector3 rotation = transform.eulerAngles;
            rotation.z = 0;

            transform.eulerAngles = rotation;
        }

        cameraPosition = target_.position - transform.forward * cameraDistance * distanceUp;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * smooth);

        distanceUp = Mathf.Clamp(distanceUp += Input.GetAxis("Vertical"), minAngle, maxAngle);

        if(Physics.Linecast(target_.position, transform.position, out hit)) {
            transform.position = hit.point + transform.forward * adjustCollisionForward;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target_ = newTarget;
    }

    public void SetCameraDistance(float newDistance)
    {
        cameraDistance = newDistance;
    }
}
