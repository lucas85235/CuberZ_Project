using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCube : MonoBehaviour
{
    public Transform[] allcubes_;
    List<Vector3> cubesPositionList_ = new List<Vector3>();
    List<Quaternion> cubesRotationList_ = new List<Quaternion>();

    private void Start()
    {
        StartCoroutine(RememberCubesPositionAndRotation());
        OnEnable();
    }

    protected void OnEnable()
    {
        StartCoroutine(WaitToDissolve());
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

    private IEnumerator ResetCubesPositionAndRotation()
    {
        for (int i = 0; i < allcubes_.Length; i++)
        {
            allcubes_[i].transform.localPosition = cubesPositionList_[i];
            allcubes_[i].transform.localRotation = cubesRotationList_[i];
        }

        transform.gameObject.SetActive(false);

        yield break;

    }

    private IEnumerator WaitToDissolve()
    {
        for (int i = 0; i < allcubes_.Length; i++)
        {
            allcubes_[i].GetComponent<Collider>().enabled = true;

            float randomForceX_ = Random.Range(-2f, 2f);
            float randomForceY_ = Random.Range(1, 5f);
            float randomForceZ_ = Random.Range(-2f, 2f);

            allcubes_[i].GetComponent<Rigidbody>().AddForce
                (randomForceX_, randomForceY_, randomForceZ_, ForceMode.Impulse);

        }

        yield return new WaitForSeconds(2);
        transform.GetChild(0).GetComponent<Animator>().Play("DissolveCubo", -1, 0);
        yield return new WaitForSeconds(3);
        StartCoroutine(ResetCubesPositionAndRotation());
        yield break;
    }
}
