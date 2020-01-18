using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterAbstraction
{
    public CharacterAbstraction moster;

    protected override void Start()
    {
        boby_ = GetComponent<Rigidbody>();
        animator_ = GetComponent<Animator>();
        camera_ = Camera.main.gameObject;
        cameraController_ = Camera.main.GetComponent<CameraController>();
        // collider_ = transform.Find("COLISOR").gameObject;

        boby_.freezeRotation = true;
        isDead = false;

        characterLife = maxLife;
        SetStartCharacter();
    }

    protected override void Update()
    {
        axisX = Input.GetAxis("Horizontal");
        axisY = Input.GetAxis("Vertical");

        Walk();
        AnimationSpeed();

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (moster != null)
                SwitchCharacterController(moster);
        }
    }

    private void SetStartCharacter()
    {
        CharacterAbstraction thisCharacter = GetComponent<CharacterAbstraction>();
        CharacterAbstraction[] allCharacters = FindObjectsOfType<CharacterAbstraction>();

        foreach (CharacterAbstraction currentCharacter in allCharacters)
        {
            Debug.Log("Loop");

            if (currentCharacter != thisCharacter)
                currentCharacter.enabled = false;
        }

        thisCharacter.enabled = true;
        SetCameraPropeties(transform.Find("CameraTarget"));
    }
}
