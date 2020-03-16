using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : CameraProperties
{
    // O target deve ser um pivo no meio do personagem
    [Min(0)]
    [SerializeField] private float minAngleY = 0f;
    [SerializeField] private float maxAngleY = 60f;

    private RaycastHit hit_;
    private IInput input_;

    [Header("Player to follow")]
    [SerializeField] private Transform target_;
    [SerializeField] private Transform enemyTarget_;

    [Header("Camera Properties")]
    [SerializeField] private float cameraDistanceMin_ = 5.0f;
    [SerializeField] private float cameraDistanceMax_ = 20.0f;
    [SerializeField] private float cameraDistance_ = 16.0f;
    [SerializeField] private float zoomSpeed_ = 25.0f;
    [SerializeField] private float smooth_ = 4.0f;
    
    public float sensibility = 125.0f;
    public float adjustCollisionForward = 0.1f;
    public bool invertVerticalMouseInput;

    public LayerMask excludeLayers;

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
        Construt(Object.FindObjectOfType<DesktopInputImpl>());
    }

    void FixedUpdate()
    {
        if (cameraStyle_ == CameraMode.FollowPlayer)
        {
            if (target_) cameraPosition_ = target_.position - transform.forward * cameraDistance_ * distanceUp_;
            distanceUp_ = Mathf.Clamp(distanceUp_ += input_.GetAxisVertical(), minAngle_, maxAngle_);
            transform.position = Vector3.Lerp(transform.position, cameraPosition_, Time.deltaTime * smooth_);
        }
        else if (cameraStyle_ == CameraMode.Capturing)
        {
            if (target_) cameraPosition_ = target_.position - transform.forward * (cameraDistance_ / 2);
            transform.position = Vector3.Lerp(transform.position, cameraPosition_, Time.deltaTime * smooth_);
        }

        if (cameraStyle_ == CameraMode.TargetEnemy)
        {
            if(target_) cameraPosition_ = target_.position - transform.forward * cameraDistance_ * distanceUp_;
            distanceUp_ = Mathf.Clamp(distanceUp_ += input_.GetAxisVertical(), minAngle_, maxAngle_);
            transform.position = Vector3.Lerp(transform.position, cameraPosition_, Time.deltaTime * smooth_);

            this.transform.LookAt(enemyTarget_);
        }
        else if (input_.MoveCamera())
            CameraRotate();

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            CameraZoom();

        if (target_)
        {
            if (Physics.Linecast(target_.position, transform.position, out hit_, excludeLayers))
                transform.position = hit_.point + transform.forward * adjustCollisionForward;
        }
    }

    private void CameraRotate()
    {
        if (target_) transform.RotateAround(target_.position, transform.up, input_.GetAxisMouseX() * sensibility * Time.deltaTime);

        float angleY = 0;

        if (invertVerticalMouseInput)
        {
            angleY = -input_.GetAxisMouseY() * sensibility * Time.deltaTime;
        }
        else angleY = input_.GetAxisMouseY() * sensibility * Time.deltaTime;

        if (transform.eulerAngles.x + angleY > minAngleY && transform.eulerAngles.x + angleY < maxAngleY)
        {
            if(target_) transform.RotateAround(target_.position, transform.right, angleY);
        }

        Vector3 rotation = transform.eulerAngles;
        rotation.z = 0;

        transform.eulerAngles = rotation;
    }

    private void CameraZoom()
    {
        cameraDistance_ -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed_;
        cameraDistance_ = Mathf.Clamp(cameraDistance_, cameraDistanceMin_, cameraDistanceMax_);
    }

    public override bool IsFollowMode() { return cameraStyle_ == CameraMode.FollowPlayer; }
    public override bool IsCaptureMode() { return cameraStyle_ == CameraMode.Capturing; }

    public override float GetCameraDistance() { return cameraDistance_; }
    public override float GetDistanceUp() { return distanceUp_; }
    public override float GetMinAngle() { return minAngle_; }
    public override float GetMaxAngle() { return maxAngle_; }
    public override float GetSmooth() { return smooth_; }
    public override Transform GetTarget() { return target_; }
    public override Transform GetEnemyTarget() { return enemyTarget_; }
    public override System.Enum GetFollowMode() { return CameraMode.FollowPlayer; }
    public override System.Enum GetCapturingMode() { return CameraMode.Capturing; }

    public override void SetTarget(Transform newTarget) { target_ = newTarget; }
    public override void SetEnemyTarget(Transform newTarget) { enemyTarget_ = newTarget; }
    public override void SetDistanceUp(float distanceUp) { distanceUp_ = distanceUp; }
    public override void SetMinAngle(float minAngle) { minAngle_ = minAngle; }
    public override void SetMaxAngle(float maxAngle) { maxAngle_ = maxAngle; }
    public override void SetSmooth(float newSmooth) { smooth_ = newSmooth; }
    public override void SetCameraDistance(float newDistance) { cameraDistance_ = newDistance; }
}
