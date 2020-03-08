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
    [HideInInspector] public bool isSwimMode = false;

    [Header("Adjust Camera Propeties")]
    public float cameraDistance = 14.0f;

    [Header("Walk Stats")]
    public float walkSpeed = 15.0f;
    public float runSpeed = 22.0f;
    public float smoothTime = 0.3f;
    protected float smooth_;

    [Header("Jump Config")]
    public float initialJumpImpulse = 10.0f;
    public float jumpForce = 0.03f;
    public float jumpTime = 0.12f;
    public float dowmSpeed = 15;

    protected bool startJumpWait_ = false;
    protected bool canJump_ = true;
    protected bool isJump = false;
    protected bool startJumpTime = false;
    protected float countJumpTime = 0;

    [Header("Stamina Stats")]
    public float staminaRegen = 0.03f;
    public float regenStartTime = 1.0f;

    [SerializeField] protected float characterStamina;
    protected float maxStamina = 100f;
    protected float countRegenStartTime = 0;
    protected bool startRegenProcess = false;
    protected bool inRunInput = false;
    protected bool endedStamina = false;

    [SerializeField]
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

    protected virtual bool ExistGround()
    {
        return Physics.Raycast(transform.position, (-1 * transform.up), 0.25f);
    }

    public virtual void SwitchCharacterController(CharacterAbstraction switchCharacter) 
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

    protected IEnumerator EndedRegenTime()
    {
        endedStamina = true;
        yield return new WaitUntil(() => inRunInput == false && characterStamina > 5f);
        endedStamina = false;
    }

    public abstract void IncrementStamina(float increment);

    public abstract void DecrementStamina(float decrement);

    public bool HaveStamina()
    {
        return characterStamina > 0;
    }

    public void RegenStamina()
    {
        if (characterStamina < maxStamina)
        {
            if (!startRegenProcess)
            {
                startRegenProcess = true;
                countRegenStartTime = 0;
            }
            else countRegenStartTime += Time.deltaTime;

            if (countRegenStartTime >= regenStartTime)
                IncrementStamina(staminaRegen);

        }
    }

    /*protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            canJump_ = false;  
            Debug.Log("Can Jump: " + false);
        }    
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall") 
        {
            canJump_ = true;
            Debug.Log("Can Jump: " + false);
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && isJump)
        {
            transform.position += new Vector3(0, -0.1f, 0);
            body_.AddForce(Vector3.down * dowmSpeed);
            Debug.Log("Down Jump!");
        }
    }*/
}
