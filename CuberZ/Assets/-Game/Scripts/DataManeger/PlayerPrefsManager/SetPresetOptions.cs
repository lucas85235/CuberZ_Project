using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPresetOptions : MonoBehaviour
{
    private DataPresetOptions data;

    // Start is called before the first frame update
    private void Awake()
    {
        data = new DataPresetOptions();

        if (!data.ExistResolutionSave() || !data.ExistQualitySave() || !data.ExistVolumeSave() || !data.ExistFullScreenSave())
        {
            data.resolution = GetCurrentResolutionIndex();
            data.quality = QualitySettings.GetQualityLevel();
            data.fullScreen = Screen.fullScreen ? 1 : 0;
            data.volume = 1.0f;

            data.SavePreset();
        }

        data.LoadPreset();

        SetGraphicOptions();
        SetSoundOptions();

        Destroy(GetComponent<SetPresetOptions>());
    }

    // Update is called once per frame
    private void SetGraphicOptions()
    {
        QualitySettings.SetQualityLevel(data.quality);
        Screen.SetResolution(Screen.resolutions[data.resolution].width, 
            Screen.resolutions[data.resolution].height, data.fullScreen != 0 ? true : false);  
    }

    private void SetSoundOptions()
    {
        AudioListener.volume = data.volume;
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].height == Screen.currentResolution.height && 
                Screen.resolutions[i].width == Screen.currentResolution.width)
                    return i;
        }

        return Screen.resolutions.Length;
    }
}
