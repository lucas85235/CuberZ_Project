using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAMERA : MonoBehaviour {

    public float ANGLE_MIN = -80.0f;
    public LayerMask layer;
    public float ANGLE_MAX = 10.0f;

    public Transform alvo;
    RaycastHit hit = new RaycastHit ();
    public float mouseX = 0;
    public float mouseY = 0;
    private float currentX = 0.0f;
    private float currentY = 0.0f;

    public float distCam;
    public float ajusteCamera;

    bool PRESS () {
        return Input.GetMouseButton (1);
    }

    void Start () {
        alvo = GameObject.Find ("LOOK").transform;
    }

    void Update () {
        
        if (PRESS ()) {

            currentX += Input.GetAxis ("Horizontal") * mouseX + Input.GetAxis ("Mouse X") * mouseX;
            currentY -= Input.GetAxis ("Vertical") * mouseY + Input.GetAxis ("Mouse Y") * mouseY;

            currentY = Mathf.Clamp (currentY, ANGLE_MIN, ANGLE_MAX);
            Vector3 dir = new Vector3 (0, 0, distCam);
            Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
            transform.position = alvo.position + rotation * dir;
            transform.LookAt (alvo.position);

        }
        transform.position = alvo.position - transform.forward * distCam;

        if (Physics.Linecast (alvo.position, transform.position, out hit, layer)) {
            transform.position = hit.point + transform.forward * ajusteCamera;
        }

    }
}