using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu UI")]
    public Text titleText;
    public Button playButton;
    public Button quitButton;
    public Text instructionsText;

    void Start()
    {
        // Configurar textos
        if (titleText != null)
        {
            titleText.text = "ARKANOID";
        }

        if (instructionsText != null)
        {
            instructionsText.text = "Use as setas ou A/D para mover\nEspaço para lançar a bola\nESC para pausar";
        }

        // Configurar botões
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    void Update()
    {
        // Também pode pressionar Espaço para começar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
