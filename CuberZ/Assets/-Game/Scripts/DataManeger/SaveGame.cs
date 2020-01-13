using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame : MonoBehaviour
{
    public Button returnMenu;
    public GameObject saveText;

    // Start is called before the first frame update
    void Start()
    {
        returnMenu.onClick.AddListener(
            ()=> MenuManeger.LoadScene("MainMenu"));

        saveText.SetActive(false);

        if (!PlayerPrefs.HasKey("SaveGame"))
        {
            PlayerPrefs.SetInt("SaveGame", 1);

            saveText.SetActive(true);
            Destroy(saveText, 3.0f);
        }
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
