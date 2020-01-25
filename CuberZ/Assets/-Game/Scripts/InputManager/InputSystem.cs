using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour, IInput
{
    private static InputSystem instance_;
    public static InputSystem instance { get { return instance_; } }
   
    [Header("Teclas Configuráveis")]
    public KeyCode moveCameraKey = KeyCode.Mouse1;
    public KeyCode executeActionInput = KeyCode.Mouse0;
    public KeyCode kubberAtk1Key = KeyCode.Alpha1;
    public KeyCode kubberAtk2Key = KeyCode.Alpha2;
    public KeyCode kubberAtk3Key = KeyCode.Alpha3;
    public KeyCode kubberAtk4Key = KeyCode.Alpha4;
    public string getAxisHorizontal = "Horizontal";
    public string getAxisVertical = "Vertical";
    public string getAxisMouseX = "Mouse X";
    public string getAxisMouseY = "Mouse Y";
    public KeyCode fixCameraOnMyKubberKey = KeyCode.Tab;
    public KeyCode rescueKubberKey = KeyCode.LeftControl;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode exitKey = KeyCode.Escape;
    public KeyCode captureKubberkey = KeyCode.Backspace;

    private void Awake()
    {
        instance_ = this;
    }

    #region Variáveis PlayerPrefs
    private const string moveCameraPlayerPref = "MOVE-CAMERA";
    private const string executeActionPlayerPref = "EXECUTE-ACTION-INPUT";
    private const string kubberAtk1PlayerPref = "KUBBER-ATTTACK-1";
    private const string kubberAtk2PlayerPref = "KUBBER-ATTTACK-2";
    private const string kubberAtk3PlayerPref = "KUBBER-ATTTACK-3";
    private const string kubberAtk4PlayerPref = "KUBBER-ATTTACK-4";
    private const string getAxisHorizontalPlayerPref = "GET-AXIS-HORIZONTAL";
    private const string getAxisVerticalPlayerPref = "GET-AXIS-VERTICAL";
    private const string getAxisMouseXPlayerPref = "GET-AXIS-MOUSE-X";
    private const string getAxisMouseYPlayerPref = "GET-AXIS-MOUSE-Y";    
    private const string fixCameraOnMyKubberPlayerPref = "FIX-CAMERA-ON-MY-KUBBER";
    private const string rescueKubberPlayerPref = "RESCUE-KUBBER";
    private const string runPlayerPref = "RUN-PLAYER";
    private const string exitPlayerPref = "EXIT-PLAYER";
    private const string captureKubberPref = "CAPTURE-KUBBER";
    #endregion

    #region Funções Singleton
    public bool MoveCameraInput() { return Input.GetKey(moveCameraKey); }
    public bool ExecuteActionInput() { return Input.GetKeyDown(executeActionInput); }
    public bool KubberAttack1Input() { return Input.GetKeyDown(kubberAtk1Key); }
    public bool KubberAttack2Input() { return Input.GetKeyDown(kubberAtk2Key); }
    public bool KubberAttack3Input() { return Input.GetKeyDown(kubberAtk3Key); }
    public bool KubberAttack4Input() { return Input.GetKeyDown(kubberAtk4Key); }
    public float GetAxisHorizontal() { return Input.GetAxis(getAxisHorizontal); }
    public float GetAxisVertical() { return Input.GetAxis(getAxisVertical); }
    public float GetAxisMouseX() { return Input.GetAxis(getAxisMouseX); }
    public float GetAxisMouseY() { return Input.GetAxis(getAxisMouseY); }
    public bool FixCameraOnMyKubberInput() { return Input.GetKeyDown(fixCameraOnMyKubberKey); }
    public bool RescueKubberInput() { return Input.GetKeyDown(rescueKubberKey); }
    public bool RunInput() { return Input.GetKey(runKey); }
    public bool ExitInput() { return Input.GetKeyDown(exitKey); }
    public bool CaptureKubberInput() { return Input.GetKeyDown(captureKubberkey); }
    #endregion

    #region Funções PlayerPrefs
    public void SaveAllKeyCodes()
    {
        PlayerPrefs.SetString(moveCameraPlayerPref, moveCameraKey.ToString());
        PlayerPrefs.SetString(executeActionPlayerPref, executeActionInput.ToString());
        PlayerPrefs.SetString(kubberAtk1PlayerPref, kubberAtk1Key.ToString());
        PlayerPrefs.SetString(kubberAtk2PlayerPref, kubberAtk2Key.ToString());
        PlayerPrefs.SetString(kubberAtk3PlayerPref, kubberAtk3Key.ToString());
        PlayerPrefs.SetString(kubberAtk4PlayerPref, kubberAtk4Key.ToString());
        PlayerPrefs.SetString(getAxisHorizontalPlayerPref, getAxisHorizontal);
        PlayerPrefs.SetString(getAxisVerticalPlayerPref, getAxisVertical);
        PlayerPrefs.SetString(getAxisMouseXPlayerPref, getAxisMouseX);
        PlayerPrefs.SetString(getAxisMouseYPlayerPref, getAxisMouseY);
        PlayerPrefs.SetString(fixCameraOnMyKubberPlayerPref, fixCameraOnMyKubberKey.ToString());
        PlayerPrefs.SetString(rescueKubberPlayerPref, rescueKubberKey.ToString());
        PlayerPrefs.SetString(runPlayerPref, runKey.ToString());
        PlayerPrefs.SetString(exitPlayerPref, exitKey.ToString());
        PlayerPrefs.SetString(captureKubberPref, captureKubberkey.ToString());

        Debug.Log("Todas as Keys Foram Salvas");
    }

    public void LoadAllKeyCodes()
    {
        moveCameraKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(moveCameraPlayerPref));
        executeActionInput = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(executeActionPlayerPref));
        kubberAtk1Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kubberAtk1PlayerPref));
        kubberAtk2Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kubberAtk2PlayerPref));
        kubberAtk3Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kubberAtk3PlayerPref));
        kubberAtk4Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kubberAtk4PlayerPref));
        fixCameraOnMyKubberKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(fixCameraOnMyKubberPlayerPref));
        rescueKubberKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(rescueKubberPlayerPref));
        runKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(runPlayerPref));
        exitKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(exitPlayerPref));
        getAxisHorizontal = PlayerPrefs.GetString(getAxisHorizontalPlayerPref);
        getAxisVertical = PlayerPrefs.GetString(getAxisVerticalPlayerPref);
        getAxisHorizontal = PlayerPrefs.GetString(getAxisMouseXPlayerPref);
        getAxisVertical = PlayerPrefs.GetString(getAxisMouseYPlayerPref);
        captureKubberkey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(captureKubberPref));
        
        Debug.Log("Todas as Keys Foram Carregadas");
    }
    #endregion
}
