using UnityEngine;
using System.Collections;
 
public class RealCamera : MonoBehaviour
{   
    [Header("Player to follow")]
    private Transform target;                    
    
    [Header("Layer(s) to include")]
    private LayerMask CamOcclusion;                             // the layers that will be affected by collision
    
    [Header("Camera Properties")]
    public float DistanceAway;                                  // how far the camera is from the player.
    public float DistanceUp;                                    // how high the camera is above the player
            
    [SerializeField] private float smooth = 4.0f;               // how smooth the camera moves into place
    [SerializeField] private float rotateAround = 70f;          // the angle at which you will rotate the camera (on an axis)
    // [SerializeField] private float cameraHeight = 55f;
    // [SerializeField] private float cameraForward = 0f;
    [SerializeField] private float cameraRotateSpeed = 180f;
    [SerializeField] private Vector3 cameraPosition;
    [SerializeField] private Vector3 cameraMask;
    
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    // public float minDistance = 1;               
    // public float maxDistance = 2; 
    // DistanceAway = Mathf.Clamp(DistanceAway += VerticalAxis, minDistance, maxDistance);

    private float HorizontalAxis;
    private float VerticalAxis;

    void Start () 
    {
        // position the camera behind the target
        rotateAround = target.eulerAngles.y - 45f;
    }

    void Update()
    {
 
    }
 
    void LateUpdate () 
    {
        HorizontalAxis = Input.GetAxis("Horizontal");
        VerticalAxis = Input.GetAxis("Vertical");

        if (Input.GetMouseButton(1))
        {
            Vector3 targetOffset = new Vector3(target.position.x, target.position.y, target.position.z);

            currentX += HorizontalAxis * 10 + Input.GetAxis("Mouse X") * 10;
            currentY -= VerticalAxis * 10 + Input.GetAxis("Mouse Y") * 10;

            currentY = Mathf.Clamp(currentY, angleMin, angleMax);

            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            
            cameraPosition = targetOffset + rotation * DistanceAway;
            cameraMask = targetOffset + rotation * DistanceAway;
            // transform.position = target_.position + rotation * zoon;
            // cameraPosition = targetOffset + Vector3.up * DistanceUp /*- rotateVector*/ * DistanceAway;

            DetectWall(ref targetOffset);
            SmoothCameraPosition();

            transform.LookAt(target);
        }
        else 
        {
            // Offset of the targets transform (Since the pivot point is usually at the feet)
            Vector3 targetOffset = new Vector3(target.position.x, target.position.y, target.position.z);

            /* Quaternion rotation = Quaternion.Euler(cameraHeight, rotateAround, cameraForward);
            Vector3 vectorMask = Vector3.one;
            Vector3 rotateVector = rotation * vectorMask; */

            // this determines where both the camera and it's mask will be.
            // the camMask is for forcing the camera to push away from walls.
            cameraPosition = targetOffset + Vector3.up * DistanceUp /*- rotateVector*/ * DistanceAway;
            cameraMask = targetOffset + Vector3.up * DistanceUp /*- rotateVector*/ * DistanceAway; // cameraMask = cameraPosition;
    
            DetectWall(ref targetOffset);
            SmoothCameraPosition();
    
            // transform.LookAt(target);            
        }

        #region wrap the camera orbit rotation
        if(rotateAround > 360)
        {
            rotateAround = 0f;
        }
        else if(rotateAround < 0f)
        {
            rotateAround = (rotateAround + 360f);
        }
        #endregion

        rotateAround += HorizontalAxis * cameraRotateSpeed * Time.deltaTime;
        DistanceUp = Mathf.Clamp(DistanceUp += VerticalAxis, -0.79f, 2.3f);
    }

    void SmoothCameraPosition()
    {
        smooth = 4f;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * smooth);
    }

    void DetectWall(ref Vector3 targetFollow)
    {
        #region prevent wall clipping

        RaycastHit wallHit = new RaycastHit();

        // linecast from your player (targetFollow) to your cameras mask (camMask) to find collisions.
        if(Physics.Linecast(targetFollow, cameraMask, out wallHit, CamOcclusion))
        {
            // the smooth is increased so you detect geometry collisions faster.
            smooth = 10f;

            // the x and z coordinates are pushed away from the wall by hit.normal.
            // the y coordinate stays the same.
            cameraPosition = new Vector3(
                wallHit.point.x + wallHit.normal.x * 0.5f, 
                cameraPosition.y, 
                wallHit.point.z + wallHit.normal.z * 0.5f);
        }

        #endregion
    }
}
