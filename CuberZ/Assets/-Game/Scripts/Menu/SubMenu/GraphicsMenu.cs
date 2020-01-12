using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsMenu : SubMenuAbstraction
{
    private Resolution[] resolutionsSupport;
    private int graphicQuality, resolutionsIndex;

    [Header("Implemented Components")]
    public Dropdown resolutionsBox;
    public Dropdown graphicsBox;

    private void Awake()
    {
        data = new DataPresetOptions();
        data.LoadPreset();

        resolutionsSupport = Screen.resolutions;
        applyButton.onClick.AddListener(SavePreferences);
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetResolutionsDropdown();
        SetQualityDropdown();

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
        Screen.SetResolution(resolutionsSupport[data.resolution].width, 
            resolutionsSupport[data.resolution].height, true);
    }

    private void SetQuality()
    {
        QualitySettings.SetQualityLevel(data.quality);
    }

    public void SavePreferences()
    {
        data.quality = graphicsBox.value;
        data.resolution = resolutionsBox.value;
        data.SavePreset();

        SetResolution();
        SetQuality();
    }
}
