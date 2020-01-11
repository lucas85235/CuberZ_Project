using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsMenu : SubMenuAbstraction
{
    public Button applyButton;
    public Dropdown resolutionsBox, graphicsBox;

    private int graphicQuality, resolutionsIndex;
    private Resolution[] resolutionsSupport;

    private void Awake()
    {
        resolutionsSupport = Screen.resolutions;
        applyButton.onClick.AddListener(SavePreferencces);
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetResolutionsDropdown();
        SetQualityDropdown();

        if (PlayerPrefs.HasKey("RESOLUTION"))
        {
            SetResolution(PlayerPrefs.GetInt("RESOLUTION"));
            resolutionsBox.value = resolutionsIndex;
        }
        else
        {
            SetResolution(resolutionsSupport.Length);
            PlayerPrefs.SetInt("RESOLUTION", resolutionsIndex);
            resolutionsBox.value = resolutionsIndex;
        }

        if (PlayerPrefs.HasKey("QUALITY"))
        {
            graphicQuality = PlayerPrefs.GetInt("QUALITY");
            QualitySettings.SetQualityLevel(graphicQuality);
            graphicsBox.value = graphicQuality;
        }
        else
        {
            QualitySettings.SetQualityLevel((QualitySettings.names.Length));
            graphicQuality = (QualitySettings.names.Length);
            PlayerPrefs.SetInt("QUALITY", graphicQuality);
            graphicsBox.value = graphicQuality;
        }
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

    private void SetResolution(int index)
    {
        resolutionsIndex = index;
        Screen.SetResolution(resolutionsSupport[resolutionsIndex].width, 
            resolutionsSupport[resolutionsIndex].height, true);
    }

    public void SavePreferencces()
    {
        PlayerPrefs.SetInt("QUALITY", graphicsBox.value);
        PlayerPrefs.SetInt("RESOLUTION", resolutionsBox.value);

        SetResolution(resolutionsBox.value);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qualidadeGrafica"));

        resolutionsBox.captionText.text = "Resolução";
        graphicsBox.captionText.text = "Gráficos";
    }

    public Button GetReturnButton()
    {
        return returnButton;
    }
}
