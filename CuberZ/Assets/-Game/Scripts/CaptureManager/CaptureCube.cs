using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCube : MonoBehaviour
{
    public GameObject fakeCube_;
    public Transform[] allcubes_;
    Rigidbody rb_;
    [HideInInspector]
    public float gravityImpact_;
    [HideInInspector]
    public Vector3 target_;
    [HideInInspector]
    public float speed_;
    [HideInInspector]
    bool moviment_ = true;
    [HideInInspector]
    public float impulseY_;
    bool break_;
    bool capture_;
    bool moveCapture_;
    Vector3 point_;
    Collider col2;

    private void Awake()
    {
        moviment_ = true;
        rb_ = GetComponent<Rigidbody>();
        transform.GetChild(0).localScale = Vector3.one;
        rb_.useGravity = false;
        rb_.velocity = Vector3.zero;
    }

    private void OnEnable()
    {
        rb_ = GetComponent<Rigidbody>();
        transform.GetChild(0).localScale = Vector3.one;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        rb_.useGravity = false;
        break_ = false;
        rb_.velocity = Vector3.zero;
        moviment_ = true;

    }

    private void OnDisable()
    {
        rb_.useGravity = false;
        rb_.velocity = Vector3.zero;

    }

    private void Update()
    {
        Abduction(col2);
        StopDistanceControl();
    }

    protected void PhysicsControl()
    {
        if (rb_.useGravity && moviment_ && !capture_)
        {
            rb_.velocity = (target_ - transform.position).normalized * speed_ + new Vector3(0, impulseY_, 0);
            transform.Rotate(-900 * Time.deltaTime, 0, 0, Space.Self);
        }
    }


    private void FixedUpdate()
    {
        PhysicsControl();
    }

    protected void StopDistanceControl()
    {
        if (Vector3.Distance(transform.position, target_) <= 1.5f && moviment_ && !capture_)
        {
            moviment_ = false;
            rb_.useGravity = false;
        }

    }

    protected void Abduction(Collider coll)
    {
        if (capture_)
        {
            coll.GetComponent<Rigidbody>().useGravity = false;

            coll.transform.localScale = Vector3.Lerp(coll.transform.localScale, Vector3.zero, 8f * Time.deltaTime);

            coll.transform.position = Vector3.Lerp(coll.transform.position, transform.position, 5f * Time.deltaTime);

            if (coll.transform.localScale == Vector3.zero)
            {

                coll.transform.SetParent(transform);
                capture_ = false;
                coll.transform.gameObject.SetActive(false);
                transform.GetChild(1).GetComponent<Animator>().Play("DissolveCubo", -1, 0);

            }
        }

    }

    private void OnTriggerStay(Collider col)
    {
        col2 = col;

        if (col.gameObject.name == "Ground" || col.gameObject.name == "Wall" && !break_)
        {

            rb_.velocity = Vector3.zero;
            rb_.useGravity = true;
            moviment_ = false;
            GameObject t = Pooling.InstantiatePooling(fakeCube_, transform.GetChild(1).transform.position,
             transform.GetChild(1).transform.rotation);
            transform.gameObject.SetActive(false);
            break_ = true;
        }

        else if (col.gameObject.tag == "Monster" && !capture_)
        {
            moviment_ = false;
            rb_.velocity = Vector3.zero;
            rb_.AddForce(-(col.transform.position - transform.position) * 10 + new Vector3(0, 3, 0), ForceMode.Impulse);
            rb_.AddTorque(Vector3.forward * 5, ForceMode.Impulse);
            StartCoroutine(StopCube(col));

        }
    }

    IEnumerator StopCube(Collider col)
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1f);
        rb_.useGravity = false;
        rb_.velocity = Vector3.zero;
        rb_.freezeRotation = true;
        transform.forward = point_ - transform.position;
        Debug.Log("Toca Animação");
        moveCapture_ = false;
        capture_ = true;
        col2 = col;

        yield break;
    }

}
