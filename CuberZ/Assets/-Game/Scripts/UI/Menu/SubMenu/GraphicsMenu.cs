using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsMenu : SubMenuAbstraction
{
    private Resolution[] resolutionsSupport_;
    private int graphicQuality_, resolutionsIndex_;

    [Header("Implemented Components")]
    public Dropdown resolutionsBox;
    public Dropdown graphicsBox;
    public Toggle fullScreenToggle;

    private void Awake()
    {
        data = new DataPresetOptions();
        data.LoadPreset();

        resolutionsSupport_ = Screen.resolutions;
        applyButton.onClick.AddListener(SavePreferences);
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetResolutionsDropdown();
        SetQualityDropdown();

        fullScreenToggle.isOn = data.fullScreen != 0 ? true : false;
        resolutionsBox.value = data.resolution;
        graphicsBox.value = data.quality;
    }

    private void SetQualityDropdown()
    {
        string[] qualityNames = QualitySettings.names;
        graphicsBox.options.Clear();
        for (int i = 0; i < qualityNames.Length; i++)
        {
            graphicsBox.options.Add(new Dropdown.OptionData() {
                text = qualityNames[i]
            });
        }
        graphicsBox.captionText.text = "Gráficos";
    }

    private void SetResolutionsDropdown()
    {
        Resolution[] suportedResolutions = Screen.resolutions;
        resolutionsBox.options.Clear();
        for (int i = 0; i < suportedResolutions.Length; i++)
        {
            resolutionsBox.options.Add(new Dropdown.OptionData() {
                text = suportedResolutions[i].width + "x" + suportedResolutions[i].height
            });
        }
        resolutionsBox.captionText.text = "Resolução";
    }

    private void SetResolution()
    {
        Screen.SetResolution(resolutionsSupport_[data.resolution].width, 
            resolutionsSupport_[data.resolution].height, fullScreenToggle.isOn);
    }

    private void SetQuality()
    {
        QualitySettings.SetQualityLevel(data.quality);
    }

    public void SavePreferences()
    {
        data.quality = graphicsBox.value;
        data.resolution = resolutionsBox.value;
        data.fullScreen = fullScreenToggle ? 1 : 0;
        data.SavePreset();

        SetResolution();
        SetQuality();
    }
}
