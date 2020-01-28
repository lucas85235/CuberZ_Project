using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private RaycastHit hit;
    private IInput input_;

    [Header("Player to follow")]
    [SerializeField] private Transform target_;

    [Header("Camera Properties")]
    public float sensibility = 125.0f;
    public float cameraDistance = 16.0f;
    public float adjustCollisionForward = 0.1f;
    public float smooth = 4.0f;
    public LayerMask excludeLayer;

    private float distanceUp;
    private float minAngle = 0.9f;
    private float maxAngle = 1.1f;
    private Vector3 cameraPosition;

    // O target deve ser um pivo no meio do personagem e não nos pés

    protected virtual void Construt(IInput newInputInterface) 
    {
        input_ = newInputInterface;
    }

    private void Awake() 
    {
        Construt(Object.FindObjectOfType<InputSystem>());    
    }

    void LateUpdate()
    {
        if(input_.MoveCameraInput())
        {
            CameraRotate();
        }

        cameraPosition = target_.position - transform.forward * cameraDistance * distanceUp;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * smooth);

        distanceUp = Mathf.Clamp(distanceUp += input_.GetAxisVertical(), minAngle, maxAngle);

        if(Physics.Linecast(target_.position, transform.position, out hit, excludeLayer)) 
        {
            transform.position = hit.point + transform.forward * adjustCollisionForward;
        }
    }

    private void CameraRotate() 
    {
        transform.RotateAround(target_.position, transform.up, input_.GetAxisMouseX() * sensibility * Time.deltaTime);
        transform.RotateAround(target_.position, transform.right, input_.GetAxisMouseY() * sensibility * Time.deltaTime);

        Vector3 rotation = transform.eulerAngles;
        rotation.z = 0;

        transform.eulerAngles = rotation;        
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
