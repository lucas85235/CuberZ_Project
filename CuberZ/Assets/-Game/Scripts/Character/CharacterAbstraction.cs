using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbstraction : MonoBehaviour
{
    [Header("Basic Components")]
    protected Rigidbody boby_;
    protected Animator animator_;
    protected GameObject collider_;
    protected GameObject camera_;
    protected CameraController cameraController_;

    protected float axisX;
    protected float axisY;

    [Header("Adjust Camera Propeties")]
    public float cameraDistance = 16.0f;

    [Header("Walk Stats")]
    public float characterSpeed = 15.0f;
    public float smoothTime = 0.3f;
    private float smooth_;

    [Header("Life Stats")]
    protected float characterLife;
    protected float maxLife = 100;
    protected bool isDead = false;

    protected virtual void Start() 
    {
        boby_ = GetComponent<Rigidbody>();
        animator_ = GetComponent<Animator>();
        camera_ = Camera.main.gameObject;
        cameraController_ = Camera.main.GetComponent<CameraController>();

        // mudar
        // collider_ = transform.Find("COLISOR").gameObject; AttackCollider

        boby_.freezeRotation = true;

        isDead = false;
        characterLife = maxLife;
    }

    protected virtual void Update() 
    {
        axisX = Input.GetAxis("Horizontal");
        axisY = Input.GetAxis("Vertical");

        Walk();
        AnimationSpeed();
    }

    protected virtual void Walk() 
    {
        Vector2 input = new Vector2(axisX, axisY);

        if (input != Vector2.zero)
        {
            float targetrotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetrotation + camera_.transform.eulerAngles.y, 
                ref smooth_, 
                smoothTime);
            transform.position += transform.forward * characterSpeed * Time.deltaTime;
        }
    }

    protected virtual void AnimationSpeed() 
    {
        animator_.SetFloat("SPEED", axisX * axisX + axisY * axisY);
    }

    protected void IncrementLife(float increment) 
    {
        characterLife += increment;

        if (characterLife > maxLife) {
            characterLife = maxLife;
        }
    }

    protected void DecrementLife(float decrement) 
    {
        characterLife -= decrement;

        if (characterLife <= 0) {
            isDead = true;
            Debug.Log("Life < 0, You Are Dead!");
        }
    }

    protected void SwitchCharacterController(CharacterAbstraction switchCharacter) 
    {
        CharacterAbstraction thisCharacter = GetComponent<CharacterAbstraction>();
        CharacterAbstraction[] allCharacters = FindObjectsOfType<CharacterAbstraction>();

        foreach(CharacterAbstraction currentCharacter in allCharacters)
        {
            if (currentCharacter != thisCharacter)
                currentCharacter.enabled = false;
            if (currentCharacter == switchCharacter) { }
                currentCharacter.enabled = true;
        }

        #region stop walk
        thisCharacter.axisX = 0;
        thisCharacter.axisY = 0;
        AnimationSpeed();
        #endregion

        SetCameraPropeties(switchCharacter.transform.Find("CameraTarget"));
        thisCharacter.enabled = false;
    }

    protected virtual void SetCameraPropeties(Transform target) 
    {
        cameraController_.SetTarget(target);
        cameraController_.SetCameraDistance(cameraDistance);
    }
}
