using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealCamera : MonoBehaviour
{
    [Header("Camera Properties")]
    public float distanceAway;
    public float distanceUp;
    public float smooth = 4.0f;
    public float rotateAround = 70.0f;

    [Header("Follow Target")]
    public Transform target;

    [Header("Laye(s) to include")]
    public LayerMask camOcclusion;

    [Header("Map Coordinate Script")]
    // public worldVectorMap mwv;
    public RaycastHit hit;
    public float cameraHeight = 55.0f;
    public float cameraPan = 0;
    public float camRotateSpeed = 180f;
    Vector3 camPosition;
    Vector3 camMask;
    Vector3 followMask;

    private float HorizontalAxis;
    private float VerticalAxis;

    // Start is called before the first frame update
    void Start()
    {
        rotateAround = target.eulerAngles.y - 45f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LateUpdate()
    {
        HorizontalAxis = Input.GetAxis("Horizontal");
        VerticalAxis = Input.GetAxis("Vertical");

        Vector3 targetOffset = new Vector3(target.position.x, (target.position.y + 2f), target.position.z);

        //Quaternion rotation = Quaternion.Euler(cameraHeight, rotateAround, cameraPan);
        //Vector3 vectorMask = Vector3.one;
        //Vector3 rotateVector = rotation * vectorMask;

        //this determines where both the camera and it's mask will be.
        //the camMask is for forcing the camera to push away from walls.
        camPosition = targetOffset + Vector3.up * distanceUp * distanceAway;
        camMask = targetOffset + Vector3.up * distanceUp * distanceAway;

        DetectWall(ref targetOffset);
        SmoothCameraPosition();

        transform.LookAt(target);

        #region wrap the cam orbit rotation
        if (rotateAround > 360)
        {
            rotateAround = 0f;
        }
        else if (rotateAround < 0f)
        {
            rotateAround = (rotateAround + 360f);
        }
        #endregion

        rotateAround += HorizontalAxis * camRotateSpeed * Time.deltaTime;
        distanceUp = Mathf.Clamp(distanceUp += VerticalAxis, -0.79f, 2.3f);
        //distanceAway = Mathf.Clamp(distanceAway += VerticalAxis, minDistance, maxDistance);
    }

    void SmoothCameraPosition()
    {
        smooth = 4f;
        transform.position = Vector3.Lerp(transform.position, camPosition, Time.deltaTime * smooth);
    }

    void DetectWall(ref Vector3 targetFollow)
    {
        #region prevent wall clipping

        RaycastHit wallHit = new RaycastHit();

        //linecast from your player (targetFollow) to your cameras mask (camMask) to find collisions.
        if (Physics.Linecast(targetFollow, camMask, out wallHit, camOcclusion))
        {
            //the smooth is increased so you detect geometry collisions faster.
            smooth = 10f;

            //the x and z coordinates are pushed away from the wall by hit.normal.
            //the y coordinate stays the same.
            camPosition = new Vector3(
                wallHit.point.x + wallHit.normal.x * 0.5f, 
                camPosition.y, 
                wallHit.point.z + wallHit.normal.z * 0.5f);
        }

        #endregion
    }
}
