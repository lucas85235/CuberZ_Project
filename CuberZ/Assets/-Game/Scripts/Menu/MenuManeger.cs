using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManeger : MonoBehaviour
{
    private ButtonManager button;

    [SerializeField] private GameObject buttonPrefab_;
    [SerializeField] private GameObject disableButtonPrefab_;

    [Header("Space Between Buttons")]
    public float initialSpace = 20;
    public float spaceBetween = 40;

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
            () => LoadScene("SiglePlayer"));

        button.SetButton("Continuar", 1);
        button.CreateButton(disableButtonPrefab_); // Verificar se existe um save

        button.SetButton("Voltar", 2);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => MainMenu());
    }

    private void OptionsMenu()
    {
        ClearMenu();

        button.SetButton("Gráficos", 3);
        button.CreateButton(disableButtonPrefab_);

        button.SetButton("Som", 1);
        button.CreateButton(disableButtonPrefab_);

        button.SetButton("Legenda", 2);
        button.CreateButton(disableButtonPrefab_);

        button.SetButton("Controles", 4);
        button.CreateButton(disableButtonPrefab_);

        button.SetButton("Volar", 6);
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            ()=> MainMenu());
    }

    void ClearMenu()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
