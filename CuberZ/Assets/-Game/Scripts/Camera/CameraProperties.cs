using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraProperties : MonoBehaviour
{
    [SerializeField] 
    protected CameraMode cameraStyle_;

    public abstract bool IsFollowMode();
    public abstract bool IsCaptureMode();

    public abstract float GetCameraDistance();
    public abstract float GetDistanceUp();
    public abstract float GetMinAngle();
    public abstract float GetMaxAngle();
    public abstract float GetSmooth();
    public abstract Transform GetTarget();
    public abstract Transform GetEnemyTarget();
    public abstract System.Enum GetFollowMode();
    public abstract System.Enum GetCapturingMode();

    public abstract void SetTarget(Transform newTarget);
    public abstract void SetEnemyTarget(Transform newTarget);
    public abstract void SetDistanceUp(float distanceUp);
    public abstract void SetMinAngle(float minAngle);
    public abstract void SetMaxAngle(float maxAngle);
    public abstract void SetSmooth(float newSmooth);
    public abstract void SetCameraDistance(float newDistance);
    public virtual void SetCameraMode(CameraMode mode) { cameraStyle_ = mode; }

    public enum CameraMode
    {
        FollowPlayer,
        Capturing,
        TargetEnemy
    }
}
