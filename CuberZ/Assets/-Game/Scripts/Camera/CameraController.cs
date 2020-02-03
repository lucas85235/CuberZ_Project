using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // O target deve ser um pivo no meio do personagem

    private RaycastHit hit_;
    private IInput input_;

    [Header("Player to follow")]
    [SerializeField] private Transform target_; 

    [Header("Camera Properties")]
    public float sensibility = 125.0f;
    public float cameraDistance = 16.0f;
    public float captureDistance = 12.0f;
    public float adjustCollisionForward = 0.1f;
    public float smooth = 4.0f;
    public bool invertVerticalMouseInput;

    public LayerMask excludeLayer;    
    public CameraMode cameraStyle;

    private float distanceUp;
    private float minAngle = 0.95f;
    private float maxAngle = 1.05f;
    private Vector3 cameraPosition;

    #region Singleton
    public static CameraController instance { get { return instance_; } }
    private static CameraController instance_;
    #endregion
    
    protected virtual void Construt(IInput newInputInterface)
    {
        input_ = newInputInterface;
    }

    private void Awake()
    {
        instance_ = this;
        Construt(Object.FindObjectOfType<InputSystem>());

    }

    void LateUpdate()
    {
        if (cameraStyle == CameraMode.FollowPlayer) // Câmera Padrão
        {
            if (input_.MoveCameraInput())
            {
                CameraRotate();
            }

            cameraPosition = target_.position - transform.forward * cameraDistance * distanceUp;
            transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * smooth);

            distanceUp = Mathf.Clamp(distanceUp += input_.GetAxisVertical(), minAngle, maxAngle);
        }
        else
        {
            cameraPosition = target_.position - transform.forward * (cameraDistance/2);
            transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * smooth);


            if (Physics.Linecast(target_.position, transform.position, out hit))
            {
                transform.position = hit.point + transform.forward * adjustCollisionForward;
            }
        }

        if(Physics.Linecast(target_.position, transform.position, out hit_, excludeLayer)) 
        {
            transform.position = hit_.point + transform.forward * adjustCollisionForward;
        } 



    }

    private void CameraRotate()
    {
        transform.RotateAround(target_.position, transform.up, input_.GetAxisMouseX() * sensibility * Time.deltaTime);

        if (invertVerticalMouseInput)
            transform.RotateAround(target_.position, transform.right, -input_.GetAxisMouseY() * sensibility * Time.deltaTime);
        else 
            transform.RotateAround(target_.position, transform.right, input_.GetAxisMouseY() * sensibility * Time.deltaTime);

        Vector3 rotation = transform.eulerAngles;
        rotation.z = 0;

        transform.eulerAngles = rotation;
    }

    public void SetTarget(Transform newTarget)
    {
        target_ = newTarget;
    }

    public Transform GetTarget()
    {
        return target_;
    }

    public void SetCameraDistance(float newDistance)
    {
        cameraDistance = newDistance;
    }

    public enum CameraMode
    {
        FollowPlayer,
        Capturing
    }
}
