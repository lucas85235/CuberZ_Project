using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenu : SubMenuAbstraction
{
    private DataPresetOptions data;

    public Slider volumeSlider;
    public Button applyButton;

    // Start is called before the first frame update
    void Start()
    {
        data = new DataPresetOptions();
        data.LoadPreset();

        volumeSlider.value = data.volume;
        AudioListener.volume = data.volume;

        applyButton.onClick.AddListener(
            () => SavePreferencces());
    }

    public void SavePreferencces()
    {
        data.volume = volumeSlider.value;
        AudioListener.volume = data.volume;
        data.SavePreset();
    }
}
