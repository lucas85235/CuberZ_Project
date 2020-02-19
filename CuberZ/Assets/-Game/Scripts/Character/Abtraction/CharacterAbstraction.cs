using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]

public abstract class CharacterAbstraction : MonoBehaviour
{
    [Header("Basic Components")]
    protected Rigidbody body_;
    protected CameraController cameraController_;
    protected IInput input_;

    [HideInInspector] public float axisX;
    [HideInInspector] public float axisY;

    [Header("Adjust Camera Propeties")]
    public float cameraDistance = 14.0f;

    [Header("Walk Stats")]
    public float walkSpeed = 15.0f;
    public float runSpeed = 22.0f;
    public float smoothTime = 0.3f;
    protected float smooth_;



    public bool isEnabled { get; set; }

    protected virtual void Construt(IInput newInputInterface) 
    {
        input_ = newInputInterface;
    }

    protected virtual void Awake() 
    {
        Construt(Object.FindObjectOfType<InputSystem>());
    }

    #region Implementações padrão
    protected virtual void Movement() 
    {
        Vector2 input = new Vector2(axisX, axisY);

        if (input != Vector2.zero)
        {
            float targetrotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetrotation + Camera.main.transform.eulerAngles.y, 
                ref smooth_, 
                smoothTime);

            if (!input_.RunInput())
                transform.position += transform.forward * walkSpeed * Time.deltaTime;
            else
                transform.position += transform.forward * runSpeed * Time.deltaTime;
        }
    }

   

    protected virtual void SwitchCharacterController(CharacterAbstraction switchCharacter) 
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
        #endregion

        SetCameraPropeties(switchCharacter.transform.Find("CameraTarget"));
        StartCoroutine(WaitTime(switchCharacter));
    }

    protected virtual void SetCameraPropeties(Transform target) 
    {
        cameraController_.SetTarget(target);
        cameraController_.SetCameraDistance(cameraDistance);
    }
    #endregion

    protected IEnumerator WaitTime(CharacterAbstraction switchCharacter)
    {
        // chamar para nao trocar 2 vezes seguidas
        yield return new WaitForSeconds(0.2f);
        switchCharacter.isEnabled = true;
    }
}
