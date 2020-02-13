using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMonsterByClick : MonoBehaviour
{
    [Header("Mask")]
    public LayerMask mask;

    [Header("Zoom Config")]
    [Range(0, 50)] public float cameraDistance;
    [Range(0, 20)] public float smooth;
    [Range(0, 20)] public float distanceUp;
    [Range(0, 2f)] public float minAngle;
    [Range(0, 2f)] public float maxAngle;

    private float defaultCameraDistance_;
    private float defaultSmooth_;
    private float defaultDistanceUp_;
    private float defaultMinAngle_;
    private float defaultMaxAngle_;

    private int countTimes; // Para quando o código estiver completo
    private ICameraProperties camera_;
    private IInput input_;
    private MonsterDataBase dataBase_;
    private Transform previousCamTarget_;
    private Transform placeOfTransform_;
    private bool findMonster_;
    private bool findSomeone_;

    private void Construt(IInput newInputInterface, ICameraProperties newCamera)
    {
        input_ = newInputInterface;
        camera_ = newCamera;
    }

    private void Awake()
    {
        Construt (Object.FindObjectOfType<InputSystem>(), 
            Camera.main.GetComponent<ICameraProperties>());
    }

    private void Start()
    {
        dataBase_ = MonsterDataBase.instance;
    }

    private void Update()
    {
        if (input_.ExecuteActionInput() && !findMonster_)
        {
            RayCastMonster();
            Counting();
        }

        if (input_.ExitInput() && findMonster_)
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
                StartCoroutine(FindInDatabase(hit.transform));
            }
        }
    }

    private void CameraZoom()
    {
        #region SaveCameraInfo
        previousCamTarget_ = camera_.GetTarget();
        defaultCameraDistance_ = camera_.GetCameraDistance();
        defaultDistanceUp_ = camera_.GetDistanceUp();
        defaultMaxAngle_ = camera_.GetMaxAngle();
        defaultMinAngle_ = camera_.GetMinAngle();
        defaultSmooth_ = camera_.GetSmooth();
        #endregion

        #region SetNewInfo
        camera_.SetTarget(placeOfTransform_);
        camera_.SetDistanceUp(distanceUp);
        camera_.SetMinAngle(minAngle);
        camera_.SetMaxAngle(maxAngle);
        camera_.SetSmooth(smooth);
        camera_.SetCameraDistance(cameraDistance);
        #endregion

        camera_.SetCameraMode<CameraController.CameraMode>(CameraController.CameraMode.Capturing);
    }

    private void LoadCameraDefault()
    {
        camera_.SetTarget(previousCamTarget_);
        camera_.SetCameraDistance(defaultCameraDistance_);
        camera_.SetSmooth(defaultSmooth_);
        camera_.SetDistanceUp(defaultDistanceUp_);
        camera_.SetMaxAngle(defaultMinAngle_);
        camera_.SetMinAngle(defaultMaxAngle_);
        camera_.SetCameraMode<CameraController.CameraMode>(CameraController.CameraMode.FollowPlayer);
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

    private IEnumerator FindInDatabase(Transform MonsterHitted)
    {
        for (int i = 0; i < dataBase_.monster.Length; i++)
        {

            if (MonsterHitted == dataBase_.monster[i].monster.transform && dataBase_.monster[i].beenSeen)
            {
                Debug.Log("O Monster " + MonsterHitted.transform.name + " ja existe, e é o Nº" + i + 1 + " no seu DataBase!");
                findSomeone_ = true;
            }

            if (MonsterHitted == dataBase_.monster[i].monster.transform && !dataBase_.monster[i].beenSeen)
            {
                dataBase_.monster[i].beenSeen = true;
                Debug.Log("O Monster " + MonsterHitted.transform.name + " nunca foi visto antes e agora foi adicionado!");
                findSomeone_ = true;
            }

            if(i == dataBase_.monster.Length-1 && !findSomeone_)
                Debug.Log("O Monster " + MonsterHitted.transform.name + " não existe no DataBase!");
        }

        findMonster_ = true;
        placeOfTransform_ = MonsterHitted.transform;
        CameraZoom();

        yield break;
    }
}
