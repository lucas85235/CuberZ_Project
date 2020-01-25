using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]

public abstract class CharacterAbstraction : MonoBehaviour
{
    [Header("Basic Components")]
    protected Rigidbody boby_;
    protected Animator animator_;
    protected CameraController cameraController_;
    protected IInput input_;

    protected const string animationSpeed = "SPEED";

    protected float axisX;
    protected float axisY;

    [Header("Adjust Camera Propeties")]
    public float cameraDistance = 16.0f;

    [Header("Walk Stats")]
    public float characterSpeed = 15.0f;
    public float smoothTime = 0.3f;
    private float smooth_;

    public bool isEnabled { get; set; }

    protected virtual void Construt(IInput newInputInterface) 
    {
        input_ = newInputInterface;
    }

    protected virtual void Awake() 
    {
        Construt(Object.FindObjectOfType<InputSystem>());
    }

    protected virtual void Walk() 
    {
        Vector2 input = new Vector2(axisX, axisY);

        if (input != Vector2.zero)
        {
            float targetrotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetrotation + Camera.main.transform.eulerAngles.y, 
                ref smooth_, 
                smoothTime);
            transform.position += transform.forward * characterSpeed * Time.deltaTime;
        }
    }

    public virtual void AnimationSpeed() 
    {
        animator_.SetFloat(animationSpeed, axisX * axisX + axisY * axisY);
    }


    protected void SwitchCharacterController(CharacterAbstraction switchCharacter) 
    {
        CharacterAbstraction thisCharacter = GetComponent<CharacterAbstraction>();
        CharacterAbstraction[] allCharacters = FindObjectsOfType<CharacterAbstraction>();

        foreach(CharacterAbstraction currentCharacter in allCharacters)
        {
            if (currentCharacter != switchCharacter)
                currentCharacter.isEnabled = false;
        }

        #region stop character walk
        thisCharacter.axisX = 0;
        thisCharacter.axisY = 0;
        AnimationSpeed();
        #endregion

        SetCameraPropeties(switchCharacter.transform.Find("CameraTarget"));
        StartCoroutine(WaitTime(switchCharacter));
    }

    protected virtual void SetCameraPropeties(Transform target) 
    {
        cameraController_.SetTarget(target);
        cameraController_.SetCameraDistance(cameraDistance);
    }

    private IEnumerator WaitTime(CharacterAbstraction switchCharacter)
    {
        // chamar para nao trocar 2 vezes seguidas
        yield return new WaitForSeconds(0.2f);
        switchCharacter.isEnabled = true;
    }
}
