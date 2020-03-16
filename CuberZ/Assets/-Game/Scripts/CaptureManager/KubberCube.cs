using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KubberCube : MonoBehaviour
{
    [Header("Suck Options")]
    public bool suck;
    public state rotationMode;
    public float rotationSpeed;
    public float lerp;

    [Header("Explode Options")]
    public bool explode;
    [Range(1, 30)] public float forceX = 5;
    [Range(1, 30)] public float forceZ = 5;
    [Range(1, 30)] public float forceY = 5;
   


    [Header("Others")]
    public Transform[] allcubes_;
    List<Vector3> cubesPositionList_ = new List<Vector3>();
    List<Quaternion> cubesRotationList_ = new List<Quaternion>();
    private Transform fatherOffCubes_;

    public enum state
    {
        NoRotation,
        RotateX,
        RotateY,
        RotateZ,
    }


    private void Start()
    {
        StartCoroutine(RememberCubesPositionAndRotation());
        fatherOffCubes_ = transform.GetChild(0);
    }

    private void Update()
    {
        if (suck)
        {
            RotateFatherOffCubes();
            ResetCubesPositionAndRotation();
        }
        if (explode) ExplodeKubberCube();
    }



    private IEnumerator RememberCubesPositionAndRotation()
    {
        for (int i = 0; i < allcubes_.Length; i++)
        {
            cubesPositionList_.Add(allcubes_[i].transform.localPosition);
            cubesRotationList_.Add(allcubes_[i].transform.localRotation);
        }

        yield break;
    }

    private void ResetCubesPositionAndRotation()
    {
        for (int i = 0; i < allcubes_.Length; i++)
        {
            allcubes_[i].SetParent(transform.GetChild(0));
            allcubes_[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            allcubes_[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            allcubes_[i].GetComponent<Rigidbody>().useGravity = false;
            allcubes_[i].GetComponent<Rigidbody>().isKinematic = true;
            allcubes_[i].transform.localPosition = Vector3.Lerp(allcubes_[i].transform.localPosition, cubesPositionList_[i], lerp * Time.deltaTime);
            allcubes_[i].transform.localRotation = Quaternion.Lerp(allcubes_[i].transform.localRotation, cubesRotationList_[i], lerp  * 2 * Time.deltaTime);
        }
    }

    private void RotateFatherOffCubes()
    {
        switch (rotationMode)
        {
            case state.RotateY:

                fatherOffCubes_.Rotate(0, rotationSpeed * Time.deltaTime, 0);

                break;

            case state.RotateX:

                fatherOffCubes_.Rotate(rotationSpeed * Time.deltaTime, 0, 0);

                break;

            case state.RotateZ:

                fatherOffCubes_.Rotate(0, 0, rotationSpeed * Time.deltaTime);

                break;

                 default:

                break;

            
        }
    }

    private void ExplodeKubberCube()
    {
        for (int i = 0; i < allcubes_.Length; i++)
        {
            allcubes_[i].GetComponent<Collider>().enabled = true;
            allcubes_[i].SetParent(null);
            allcubes_[i].GetComponent<Rigidbody>().useGravity = true;
            allcubes_[i].GetComponent<Rigidbody>().isKinematic = false;

            float randomForceX_ = Random.Range(-forceX, forceX);
            float randomForceY_ = Random.Range(1, forceY);
            float randomForceZ_ = Random.Range(-forceZ, forceZ);

            allcubes_[i].GetComponent<Rigidbody>().AddForce(randomForceX_, randomForceY_, randomForceZ_, ForceMode.Impulse);
        }

        explode = false;

    }
}
