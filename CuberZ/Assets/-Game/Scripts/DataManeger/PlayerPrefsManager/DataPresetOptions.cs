using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPresetOptions : MonoBehaviour
{
    public const string resolutions_ = "RESOLUTION";
    public const string quality_ = "QUALITY";
    public const string fullScreen_ = "FULLSCREEN";
    public const string volume_ = "VOLUME";

    public int resolution, quality, fullScreen;
    public float volume;

    public void LoadPreset()
    {
        resolution = PlayerPrefs.GetInt(resolutions_);
        quality = PlayerPrefs.GetInt(quality_);
        fullScreen = PlayerPrefs.GetInt(fullScreen_);

        volume = PlayerPrefs.GetFloat(volume_);
    }

    public void SavePreset()
    {
        PlayerPrefs.SetInt(resolutions_, resolution);
        PlayerPrefs.SetInt(quality_, quality);
        PlayerPrefs.SetInt(fullScreen_, fullScreen);

        PlayerPrefs.SetFloat(volume_, volume);
    }

    public bool ExistResolutionSave() { return PlayerPrefs.HasKey(resolutions_); }
    public bool ExistQualitySave() { return PlayerPrefs.HasKey(quality_); }
    public bool ExistFullScreenSave() { return PlayerPrefs.HasKey(fullScreen_); }
    public bool ExistVolumeSave() { return PlayerPrefs.HasKey(volume_); }
}
