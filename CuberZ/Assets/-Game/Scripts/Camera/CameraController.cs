using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float angleMin = -80.0f;
    [SerializeField] private float angleMax = -1.0f;


    private RaycastHit hit_ = new RaycastHit();
    private LayerMask layer_;
    private Transform target_;

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    public float mouseX = 10.0f;
    public float mouseY = 10.0f;
    public float zoonInPosition = 8.0f;
    public float cameraPosition = -18.0f;

    void Start()
    {
        target_ = GameObject.Find("LOOK").transform;
        layer_ = LayerMask.GetMask("Default");
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Horizontal") * mouseX + Input.GetAxis("Mouse X") * mouseX;
            currentY -= Input.GetAxis("Vertical") * mouseY + Input.GetAxis("Mouse Y") * mouseY;

            currentY = Mathf.Clamp(currentY, angleMin, angleMax);
            Vector3 dir = new Vector3(0, 0, zoonInPosition);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.position = target_.position + rotation * dir;
            transform.LookAt(target_.position);

        }

        transform.position = target_.position - transform.forward * zoonInPosition;

        if (Physics.Linecast(target_.position, transform.position, out hit_, layer_))
        {
            transform.position = hit_.point + transform.forward * cameraPosition;
        }
    }
}
