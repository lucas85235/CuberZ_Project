using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefault : MonoBehaviour
{
    private DesktopInputImpl inputSystem;
    private DataPresetOptions data;

    private const string defaultOptionsPreset_ = "DefaultOptionsPresetPlayerPrefs";
    private const string defaultInputs_ = "DefaultInputsPlayerPrefs";

    #region Variáveis padrões de Input
    public const KeyCode moveCameraKey = KeyCode.Mouse1;
    public const KeyCode executeActionInput = KeyCode.Mouse0;
    public const KeyCode kubberAtk1Key = KeyCode.Alpha1;
    public const KeyCode kubberAtk2Key = KeyCode.Alpha2;
    public const KeyCode kubberAtk3Key = KeyCode.Alpha3;
    public const KeyCode kubberAtk4Key = KeyCode.Alpha4;
    public const string getAxisHorizontal = "Horizontal";
    public const string getAxisVertical = "Vertical";
    public const string getAxisMouseX = "Mouse X";
    public const string getAxisMouseY = "Mouse Y";
    public const KeyCode fixCameraOnMyKubberKey = KeyCode.Tab;
    public const KeyCode rescueKubberKey = KeyCode.LeftControl;
    public const KeyCode runKey = KeyCode.LeftShift;
    public const KeyCode exitKey = KeyCode.Escape;
    public const KeyCode captureKubberkey = KeyCode.Backspace;
    #endregion

    #region Funções Unity
    private void Awake()
    {
        inputSystem = new DesktopInputImpl();
        data = new DataPresetOptions();

        if (!PlayerPrefs.HasKey(defaultInputs_))
        {
            SetDefaultInputs();
            PlayerPrefs.SetInt(defaultInputs_, 1);
        }

        if (!PlayerPrefs.HasKey(defaultOptionsPreset_))
        {
            SetDefaultOptionsPreset();
            PlayerPrefs.SetInt(defaultOptionsPreset_, 1);
        }

        IsNewGraphicOptions();
    }
    #endregion

    #region Funções de atribuição de valores
    private void SetDefaultInputs()
    {
        inputSystem.moveCameraKey = moveCameraKey;
        inputSystem.executeActionInput = executeActionInput;
        inputSystem.kubberAtk1Key = kubberAtk1Key;
        inputSystem.kubberAtk2Key = kubberAtk2Key;
        inputSystem.kubberAtk3Key = kubberAtk3Key;
        inputSystem.kubberAtk4Key = kubberAtk4Key;
        inputSystem.getAxisHorizontal = getAxisHorizontal;
        inputSystem.getAxisVertical = getAxisVertical;
        inputSystem.getAxisMouseX = getAxisMouseX;
        inputSystem.getAxisMouseY = getAxisMouseY;
        inputSystem.fixCameraOnMyKubberKey = fixCameraOnMyKubberKey;
        inputSystem.rescueKubberKey = rescueKubberKey;
        inputSystem.runKey = runKey;
        inputSystem.exitKey = exitKey;
        inputSystem.captureKubberkey = captureKubberkey;

        inputSystem.SaveAllKeyCodes();
    }

    private void SetDefaultOptionsPreset()
    {
        data.resolution = CurrentResolutionIndex();
        data.quality = QualitySettings.GetQualityLevel();
        data.fullScreen = Screen.fullScreen ? 1 : 0;
        data.volume = 1f;

        data.SavePreset();
    }

    private void IsNewGraphicOptions()
    {
        data.LoadPreset();

        if (data.resolution != CurrentResolutionIndex()) data.resolution = CurrentResolutionIndex();
        if (data.quality != QualitySettings.GetQualityLevel()) data.quality = QualitySettings.GetQualityLevel();
        if (data.fullScreen != (Screen.fullScreen ? 1 : 0)) data.fullScreen = Screen.fullScreen ? 1 : 0;

        data.SavePreset();
    }
    #endregion

    private int CurrentResolutionIndex()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].height == Screen.currentResolution.height && Screen.resolutions[i].width == Screen.currentResolution.width)
                return i;
        }

        return Screen.resolutions.Length;
    }
}
