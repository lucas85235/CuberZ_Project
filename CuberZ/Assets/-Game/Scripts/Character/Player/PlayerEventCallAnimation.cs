using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventCallAnimation : MonoBehaviour
{


    private PlayerAnimation playerAnim_;

    public static PlayerEventCallAnimation instance { get { return instance_; } }
    private static PlayerEventCallAnimation instance_;

    private void Awake()
    {
        instance_ = this;
        playerAnim_ = FindObjectOfType<PlayerAnimation>();
    }


    public void GoToWalkAnimator()
    {
        FindObjectOfType<CaptureSystem>().throwbool = false;
        playerAnim_.ResetAll();
    }




}
