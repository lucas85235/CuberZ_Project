using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICameraProperties
{
    bool IsFollowMode();
    bool IsCaptureMode();

    float GetCameraDistance();
    float GetDistanceUp();
    float GetMinAngle();
    float GetMaxAngle();
    float GetSmooth();
    Transform GetTarget();
    System.Enum GetFollowMode();
    System.Enum GetCapturingMode();

    void SetTarget(Transform newTarget);
    void SetDistanceUp(float distanceUp);
    void SetMinAngle(float minAngle);
    void SetMaxAngle(float maxAngle);
    void SetSmooth(float newSmooth);
    void SetCameraDistance(float newDistance);
    void SetCameraMode<T>(T mode);
}
