using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour, IInput
{

    static InputSystem instance_;
    public static InputSystem Instance_ { get { return instance_; } }

    [Header("Teclas Configuráveis")]
    public KeyCode moveCameraKey_;
    public KeyCode goToBattleKey_;
    public KeyCode kubberAtk1Key_;
    public KeyCode kubberAtk2Key_;
    public KeyCode kubberAtk3Key_;
    public KeyCode kubberAtk4Key_;
    public string getAxisHorizontal_;
    public string getAxisVertical_;
    public KeyCode fixCameraOnMyKubberKey_;
    public KeyCode rescueKubberKey_;
    public KeyCode runKey_;
    public KeyCode exitKey_;

    #region Variáveis PlayerPrefs
    private const string moveCameraPlayerPref_ = "moveCameraPlayerPref_";
    private const string goToBattlePlayerPref_ = "goToBattlePlayerPref_";
    private const string kubberAtk1PlayerPref_ = "kubberAtk1PlayerPref_";
    private const string kubberAtk2PlayerPref_ = "kubberAtk2PlayerPref_";
    private const string kubberAtk3PlayerPref_ = "kubberAtk3PlayerPref_";
    private const string kubberAtk4PlayerPref_ = "kubberAtk4PlayerPref_";
    private const string getAxisHorizontalPlayerPref_ = "getAxisHorizontalPlayerPref_";
    private const string getAxisVerticalPlayerPref_ = "getAxisVerticalPlayerPref_";
    private const string fixCameraOnMyKubberPlayerPref_ = "fixCameraOnMyKubberPlayerPref_";
    private const string rescueKubberPlayerPref_ = "rescueKubberPlayerPref_";
    private const string runPlayerPref_ = "runPlayerPref_";
    private const string exitPlayerPref_ = "exitPlayerPref_";

    #endregion

    #region Funções Singleton

    public bool MoveCameraInput() { return Input.GetKey(moveCameraKey_); }
    public bool GoToBattleInput() { return Input.GetKeyDown(goToBattleKey_); }
    public bool KubberAtk1Input() { return Input.GetKeyDown(kubberAtk1Key_); }
    public bool KubberAtk2Input() { return Input.GetKeyDown(kubberAtk2Key_); }
    public bool KubberAtk3Input() { return Input.GetKeyDown(kubberAtk3Key_); }
    public bool KubberAtk4Input() { return Input.GetKeyDown(kubberAtk4Key_); }
    public float GetAxisHorizontal() { return Input.GetAxis(getAxisHorizontal_); }
    public float GetAxisVertical() { return Input.GetAxis(getAxisVertical_); }
    public bool FixCameraOnMyKubberInput() { return Input.GetKeyDown(fixCameraOnMyKubberKey_); }
    public bool RescueKubberInput() { return Input.GetKeyDown(rescueKubberKey_); }
    public bool RunInput() { return Input.GetKey(runKey_); }
    public bool ExitInput() { return Input.GetKeyDown(exitKey_); }


    #endregion

    #region Funções PlayerPrefs

    public void SaveAllKeyCodes() //Salva todas as Keys
    {

        PlayerPrefs.SetString(moveCameraPlayerPref_, moveCameraKey_.ToString());
        PlayerPrefs.SetString(goToBattlePlayerPref_, goToBattleKey_.ToString());
        PlayerPrefs.SetString(kubberAtk1PlayerPref_, kubberAtk1Key_.ToString());
        PlayerPrefs.SetString(kubberAtk2PlayerPref_, kubberAtk2Key_.ToString());
        PlayerPrefs.SetString(kubberAtk3PlayerPref_, kubberAtk3Key_.ToString());
        PlayerPrefs.SetString(kubberAtk4PlayerPref_, kubberAtk4Key_.ToString());
        PlayerPrefs.SetString(getAxisHorizontalPlayerPref_, getAxisHorizontal_);
        PlayerPrefs.SetString(getAxisVerticalPlayerPref_, getAxisVertical_);
        PlayerPrefs.SetString(fixCameraOnMyKubberPlayerPref_, fixCameraOnMyKubberKey_.ToString());
        PlayerPrefs.SetString(rescueKubberPlayerPref_, rescueKubberKey_.ToString());
        PlayerPrefs.SetString(runPlayerPref_, runKey_.ToString());
        PlayerPrefs.SetString(exitPlayerPref_, exitKey_.ToString());
        Debug.Log("Todas as Keys Foram Salvas");
    }

    public void LoadAllKeyCodes() //Da load em todas as Keys
    {
        moveCameraKey_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(moveCameraPlayerPref_));
        goToBattleKey_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(goToBattlePlayerPref_));
        kubberAtk1Key_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kubberAtk1PlayerPref_));
        kubberAtk2Key_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kubberAtk2PlayerPref_));
        kubberAtk3Key_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kubberAtk3PlayerPref_));
        kubberAtk4Key_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kubberAtk4PlayerPref_));
        fixCameraOnMyKubberKey_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(fixCameraOnMyKubberPlayerPref_));
        rescueKubberKey_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(rescueKubberPlayerPref_));
        runKey_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(runPlayerPref_));
        exitKey_ = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(exitPlayerPref_));
        getAxisHorizontal_ = PlayerPrefs.GetString(getAxisHorizontalPlayerPref_);
        getAxisVertical_ = PlayerPrefs.GetString(getAxisVerticalPlayerPref_);
        Debug.Log("Todas as Keys Foram Carregadas");
    }



    #endregion
}
