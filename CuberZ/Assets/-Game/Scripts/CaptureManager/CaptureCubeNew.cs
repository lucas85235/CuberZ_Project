using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCubeNew : MonoBehaviour
{
    public GameObject fakeCube;
    [HideInInspector] public bool alreadyInUse { get; set; }
    [HideInInspector] public bool canShakeCube { get; set; }
    private CaptureSystemNew captureSystemNew_;
    private Collider myCollider_;
    private CameraController cameraController_;
    private Rigidbody rigidybody_;


    //NEGOCIO DA QUANTIDADE DE CUBOS AO CUBO QUEBRAR ETC

    private void Start()
    {
        myCollider_ = GetComponent<Collider>();
        captureSystemNew_ = FindObjectOfType<CaptureSystemNew>();
        rigidybody_ = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        transform.GetComponent<Collider>().isTrigger = true;
        transform.GetChild(0).transform.localPosition = Vector3.zero;
        transform.GetChild(1).transform.localPosition = Vector3.zero;

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider currentCollider)
    {
        DestroysCubeIfCollidesWithTags(currentCollider, "Ground", "Wall");
        CanCapture(currentCollider, "Monster");
    }

    private void OnCollisionEnter(Collision currentCollider)
    {
        DetectIfCanShake(currentCollider.collider, "Ground");
    }


    private void DestroysCubeIfCollidesWithTags(Collider currentCollider, string tag1 = "Ground", string tag2 = "Wall")
    {
        if ((currentCollider.name == tag1 || currentCollider.name == tag2) && !alreadyInUse && !captureSystemNew_.tryToCatchKubber)
        {
            transform.parent = null;
            captureSystemNew_.ResetCaptureSystem(true);
            GetComponent<Rigidbody>().useGravity = false;
            ChangeMeForFakeCube();
        }

        else if(currentCollider.name == tag1 && !alreadyInUse && captureSystemNew_.tryToCatchKubber){

            captureSystemNew_.AcessShakeCubeCoroutine();
        }

        else if (currentCollider.tag == tag1 || currentCollider.tag == tag2 && alreadyInUse)
        {
            cameraController_.SetCameraMode(CameraProperties.CameraMode.Capturing);
            GetComponent<Rigidbody>().useGravity = false;
            captureSystemNew_.previewTarget = cameraController_.GetTarget();
            cameraController_.SetTarget(transform);
            canShakeCube = true;
        }
    }

    private void CanCapture(Collider currentCollider, string tag1 = "Monster")
    {
        if (currentCollider.tag == tag1)
        {
            if (currentCollider.GetComponent<Rigidbody>()) currentCollider.GetComponent<Rigidbody>().useGravity = false;
            captureSystemNew_.startCapturing = true;
            captureSystemNew_.currentMonsterColider_ = currentCollider;
            myCollider_.enabled = false;
        }
    }

    private void DetectIfCanShake(Collider currentCollider, string tag1 = "Ground")
    {
        if ((currentCollider.name == tag1 && !alreadyInUse && captureSystemNew_.tryToCatchKubber))
        {
            captureSystemNew_.AcessShakeCubeCoroutine();
        }
    }

    private void ChangeMeForFakeCube()
    {
        rigidybody_.velocity = Vector3.zero;
        rigidybody_.useGravity = true;
        GameObject t = Pooling.InstantiatePooling(fakeCube, transform.position, transform.rotation);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.gameObject.SetActive(false);
        Destroy(transform.gameObject);
    }

    private void SetToDefaultCaptureSystem()
    {
        captureSystemNew_.ResetCaptureSystem(false);
    }
}
