using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCube : MonoBehaviour { 
   public  Transform[] allcubes_;

    private void Start()
    {
        BreakEverything();
    }
     


    private void OnEnable()
    {
        BreakEverything();
    }

    protected void BreakEverything()
    {
        for (int i = 0; i < allcubes_.Length; i++)
        {
            allcubes_[i].GetComponent<Collider>().enabled = true;
            float cubeTempX_ = allcubes_[i].transform.localScale.x;
            float cubeTempY_ = allcubes_[i].transform.localScale.y;
            float cubeTempZ_ = allcubes_[i].transform.localScale.z;

            float randomForceX_ = Random.Range(-2f, 2f);
            float randomForceY_ = Random.Range(1, 5f);
            float randomForceZ_ = Random.Range(-2f, 2f);

            allcubes_[i].GetComponent<Rigidbody>().AddForce
                (randomForceX_, randomForceY_, randomForceZ_, ForceMode.Impulse);



        }

        StartCoroutine(WaitToDissolve());

    }

    IEnumerator WaitToDissolve()
    {
        yield return new WaitForSeconds(2);
        transform.GetChild(0).GetComponent<Animator>().Play("DissolveCubo", -1, 0);
        yield break;
    }

}

