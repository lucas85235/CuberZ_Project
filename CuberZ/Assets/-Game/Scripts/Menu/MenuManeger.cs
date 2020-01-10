using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManeger : MonoBehaviour
{
    // Menu Principal
    // Single Player
    // Multi Player
    // Opções
    // Sair

    // Single Player
    // Novo Jogo
    // Continuar
    // Voltar

    // Multi Player
    // Arena
    // Mundo Aberto
    // Voltar

    // Opções
    // Gráficos
    // Som
    // Legenda
    // Controles
    // Voltar

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

        button.SetSpaceBetweenButtons(spaceBetween, initialSpace);

        MainMenu(); 
    }

    private void MainMenu()
    {
        button.SetButtonPositon(3);
        button.SetButtonText("Solo");
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => LoadScene("SiglePlayer"));

        button.SetButtonPositon(1);
        button.SetButtonText("Multijogador");
        button.CreateButton(disableButtonPrefab_);

        button.SetButtonPositon(2);
        button.SetButtonText("Opções");
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(
            () => LoadScene("Options"));

        button.SetButtonPositon(4);
        button.SetButtonText("Sair");
        button.CreateButton(buttonPrefab_);
        button.GetCurrentButton().GetComponent<Button>().onClick.AddListener(QuitGame);
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
