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

    private float distanceUp_;
    private float minAngle_ = 0.95f;
    private float maxAngle_ = 1.05f;
    private Vector3 cameraPosition_;

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
        if (cameraStyle == CameraMode.FollowPlayer)
        {
            cameraPosition_ = target_.position - transform.forward * cameraDistance * distanceUp_;
            distanceUp_ = Mathf.Clamp(distanceUp_ += input_.GetAxisVertical(), minAngle_, maxAngle_);
            transform.position = Vector3.Lerp(transform.position, cameraPosition_, Time.deltaTime * smooth);
        }
        else 
        {
            cameraPosition_ = target_.position - transform.forward * (cameraDistance / 2);
            transform.position = Vector3.Lerp(transform.position, cameraPosition_, Time.deltaTime * smooth);
        } 

        if (input_.MoveCameraInput()) 
            CameraRotate();

        if (Physics.Linecast(target_.position, transform.position, out hit_, excludeLayer)) 
            transform.position = hit_.point + transform.forward * adjustCollisionForward;
    }

    public void SetDistanceUpMinAngleMaxAngle(float distanceUp, float minAngle, float maxAngle)
    {
        distanceUp_ = distanceUp;
        minAngle_ = minAngle;
        maxAngle_ = maxAngle;
    }

    public float GetCameraDistance()
    {
        return cameraDistance;
    }

    public float GetDistanceUp()
    {
        return distanceUp_;
    }

    public float GetMinAngle()
    {
        return minAngle_;
    }
    public float GetMaxAngle()
    {
        return maxAngle_;
    }

    public float GetSmooth()
    {
        return smooth;
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
