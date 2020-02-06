using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMonsterByClick : MonoBehaviour
{
    [Header("Mask")]
    public LayerMask mask;

    [Header("Zoom Config")]
    [Range(0, 50)]
    public float cameraDistance;
    [Range(0, 20)]
    public float smooth;
    [Range(0, 20)]
    public float distanceUp;
    [Range(0, 2f)]
    public float minAngle;
    [Range(0, 2f)]
    public float maxAngle;

    private float defaultCameraDistance_;
    private float defaultSmooth_;
    private float defaultDistanceUp_;
    private float defaultMinAngle_;
    private float defaultMaxAngle_;

    private int countTimes; // Para quando o código estiver completo
    CameraController camControl_;
    private Transform previousCamTarget_;
    private Transform placeOfTransform_;
    private bool findMonster_;

    private void Awake()
    {
        camControl_ = CameraController.instance;
    }

    private void Start()
    {
        camControl_ = CameraController.instance;
    }


    private void Update()
    {
        if (InputSystem.instance.ExecuteActionInput())
        {
            RayCastMonster();
            Counting();

        }

        if (InputSystem.instance.ExitInput() && findMonster_)
        {
            LoadCameraDefault();
        }
    }


    private void RayCastMonster()
    {
        RaycastHit hit;
        Ray mousePosition_ = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mousePosition_, out hit, Mathf.Infinity, mask))
        {
            if (hit.transform.tag == "Monster")
            {
              
                Debug.Log("O Monster " + hit.transform.name + " Foi encontrado!");
                findMonster_ = true;
                placeOfTransform_ = hit.transform;
                CameraZoom();
            }
        }
    }

    private void CameraZoom()
    {

        #region SaveCameraInfo
        previousCamTarget_ = camControl_.GetTarget();
        defaultCameraDistance_ = camControl_.GetCameraDistance();
        defaultDistanceUp_ = camControl_.GetDistanceUp();
        defaultMaxAngle_ = camControl_.GetMaxAngle();
        defaultMinAngle_ = camControl_.GetMinAngle();
        defaultSmooth_ = camControl_.GetSmooth();
        #endregion

        #region SetNewInfo
        camControl_.SetTarget(placeOfTransform_);
        camControl_.cameraDistance = cameraDistance;
        camControl_.smooth = smooth;
        camControl_.SetDistanceUpMinAngleMaxAngle(distanceUp, minAngle, maxAngle);
        #endregion

        camControl_.cameraStyle = CameraController.CameraMode.Capturing;

    }

    private void LoadCameraDefault()
    {
        camControl_.SetTarget(previousCamTarget_);
        camControl_.cameraDistance = defaultCameraDistance_;
        camControl_.smooth = defaultSmooth_;
        camControl_.SetDistanceUpMinAngleMaxAngle(defaultDistanceUp_, defaultMinAngle_, defaultMaxAngle_);
        camControl_.cameraStyle = CameraController.CameraMode.FollowPlayer;
        findMonster_ = false;
    }


    private void Counting()
    {
        countTimes++;

        switch (countTimes)
        {
            case 1:
                Debug.Log("Primeira Vez Clicada");
                break;

            case 2:
                Debug.Log("Segunda Vez Clicada");
                break;

            default:
                countTimes = 1;
                Debug.Log("Primeira Vez Clicada");
                break;

        }
    }
}
