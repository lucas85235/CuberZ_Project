using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterAbstraction
{
    public MonsterBase moster;

    private void Start()
    {
        #region Get Components
        boby_ = GetComponent<Rigidbody>();
        cameraController_ = Camera.main.GetComponent<CameraController>();
        #endregion

        boby_.constraints = RigidbodyConstraints.FreezeAll;

        SetInitialCharacter();
    }

    private void Update()
    {
        if (isEnabled)
        {
            axisX = input_.GetAxisHorizontal();
            axisY = input_.GetAxisVertical();

            Movement();
            PlayerAnimation.instance.SpeedBlendTree(PlayerVelocity());
            
          //  animation_.AnimationSpeed(axisX, axisY);

            #region Get Inputs
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (moster != null)
                {
                    SwitchCharacterController(moster);
                    StartCoroutine(moster.StopFollow());
                }
            }
            #endregion
        }
    }
    

    private void SetInitialCharacter()
    {
        CharacterAbstraction thisCharacter = GetComponent<CharacterAbstraction>();
        CharacterAbstraction[] allCharacters = FindObjectsOfType<CharacterAbstraction>();

        foreach (CharacterAbstraction currentCharacter in allCharacters)
        {
            if (currentCharacter != thisCharacter)
                currentCharacter.isEnabled = false;
        }

        thisCharacter.isEnabled = true;
        SetCameraPropeties(transform.Find("CameraTarget"));
    }
}
