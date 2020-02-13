using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, ICameraProperties
{
    // O target deve ser um pivo no meio do personagem

    private RaycastHit hit_;
    private IInput input_;

    [Header("Player to follow")]
    [SerializeField] private Transform target_;

    [Header("Camera Properties")]
    public float sensibility = 125.0f;
    [SerializeField] private float cameraDistance = 16.0f;
    public float adjustCollisionForward = 0.1f;
    [SerializeField] private float smooth = 4.0f;
    public bool invertVerticalMouseInput;

    public LayerMask excludeLayers;
    [SerializeField] private CameraMode cameraStyle_;

    private float distanceUp_;
    private float minAngle_ = 0.95f;
    private float maxAngle_ = 1.05f;
    private Vector3 cameraPosition_;

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
        if (cameraStyle_ == CameraMode.FollowPlayer)
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

        if (Physics.Linecast(target_.position, transform.position, out hit_, excludeLayers)) 
            transform.position = hit_.point + transform.forward * adjustCollisionForward;
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

    public bool IsFollowMode() { return cameraStyle_ == CameraMode.FollowPlayer; }
    public bool IsCaptureMode() { return cameraStyle_ == CameraMode.Capturing; }

    public float GetCameraDistance() { return cameraDistance; }
    public float GetDistanceUp() { return distanceUp_; }
    public float GetMinAngle() { return minAngle_; }
    public float GetMaxAngle() { return maxAngle_; }
    public float GetSmooth() { return smooth; }
    public Transform GetTarget() { return target_; }
    public System.Enum GetFollowMode() { return CameraMode.FollowPlayer; }
    public System.Enum GetCapturingMode() { return CameraMode.Capturing; }

    public void SetTarget(Transform newTarget) { target_ = newTarget; }
    public void SetDistanceUp(float distanceUp) { distanceUp_ = distanceUp; }
    public void SetMinAngle(float minAngle) { minAngle_ = minAngle; }
    public void SetMaxAngle(float maxAngle) { maxAngle_ = maxAngle; }
    public void SetSmooth(float newSmooth) { smooth = newSmooth; }
    public void SetCameraDistance(float newDistance) { cameraDistance = newDistance; }
    public void SetCameraMode<T>(T mode) { throw new System.NotImplementedException(); }
    public void SetCameraMode<CameraMode>(CameraController.CameraMode mode) { cameraStyle_ = mode; }

    public enum CameraMode
    {
        FollowPlayer,
        Capturing
    }
}
