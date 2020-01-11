using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenu : SubMenuAbstraction
{
    public Slider volumeSlider;
    public Button applyButton;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("VOLUME"))
        {
            PlayerPrefs.SetFloat("VOLUME", 1);
        }

        volumeSlider.value = PlayerPrefs.GetFloat("VOLUME");
        AudioListener.volume = volumeSlider.value;

        applyButton.onClick.AddListener(
            () => SavePreferencces());
    }

    public void SavePreferencces()
    {
        PlayerPrefs.SetFloat("VOLUME", volumeSlider.value);
        AudioListener.volume = volumeSlider.value;
    }
}
