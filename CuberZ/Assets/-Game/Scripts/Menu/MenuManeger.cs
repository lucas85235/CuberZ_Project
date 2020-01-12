using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManeger : MonoBehaviour
{
    private ButtonManager button;

    [SerializeField] private GameObject graphicMenu, soundMenu;

    [Header("Set Buttons")]
    [SerializeField] private GameObject buttonPrefab_;
    [SerializeField] private GameObject disableButtonPrefab_;
    public float initialSpace = 0.05f;
    public float spaceBetween = 0.1f;


    // Start is called before the first frame update
    private void Start()
    {
        button = new ButtonManager();

        button.SetParentPosition(this.transform);
        button.SetSpaceBetweenButtons(spaceBetween, initialSpace);

        MainMenu(); 
    }

    private void MainMenu()
    {
        ClearMenu();

        button.SetButton("Solo", 3);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => SinglePlayerMenu());

        button.SetButton("Multijogador", 1);
        button.CreateButton(disableButtonPrefab_);

        button.SetButton("Opções", 2);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => OptionsMenu());

        button.SetButton("Sair", 4);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(QuitGame);
    }

    private void SinglePlayerMenu()
    {
        ClearMenu();

        button.SetButton("Novo Jogo", 3);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => LoadScene("SinglePlayer"));

        button.SetButton("Continuar", 1);
        if (!PlayerPrefs.HasKey("SaveGame"))
        {
            button.CreateButton(disableButtonPrefab_);
        }
        else
        {
            button.CreateButton(buttonPrefab_);
            button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
                () => LoadScene("SinglePlayer"));
        }           

        button.SetButton("Voltar", 2);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => MainMenu());
    }

    private void OptionsMenu()
    {
        ClearMenu();

        button.SetButton("Gráficos", 3);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => DrawSubMenu(graphicMenu));

        button.SetButton("Som", 1);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => DrawSubMenu(soundMenu));

        button.SetButton("Legenda", 2);
        button.CreateButton(disableButtonPrefab_);

        button.SetButton("Controles", 4);
        button.CreateButton(disableButtonPrefab_);

        button.SetButton("Volar", 6);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            ()=> MainMenu());
    }

    private void DrawSubMenu(GameObject menuObject)
    {
        ClearMenu();

        GameObject referenceObject = Instantiate(menuObject, this.transform.position,
            Quaternion.identity, this.transform);

        referenceObject.GetComponent<SubMenuAbstraction>().GetReturnButton().onClick.AddListener(
            () => OptionsMenu());
    }

    private void ClearMenu()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public static void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        SaveGame.DeleteSave();
        Application.Quit();
    }
}
