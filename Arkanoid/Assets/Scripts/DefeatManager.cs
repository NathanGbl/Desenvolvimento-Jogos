using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DefeatManager : MonoBehaviour
{
    [Header("Defeat UI")]
    public Text titleText;
    public Text scoreText;
    public Text levelText;
    public Button menuButton;
    public Button retryButton;

    private int finalScore;
    private int finalLevel;

    void Start()
    {
        // Recuperar dados do GameManager
        finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        finalLevel = PlayerPrefs.GetInt("FinalLevel", 1);

        // Configurar textos
        if (titleText != null)
        {
            titleText.text = "GAME OVER";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Pontuação: {finalScore}";
        }

        if (levelText != null)
        {
            levelText.text = $"Nível Alcançado: {finalLevel}";
        }

        // Configurar botões
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryGame);
        }
    }

    void Update()
    {
        // Teclas de atalho
        if (Input.GetKeyDown(KeyCode.M))
        {
            GoToMenu();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RetryGame();
        }
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    void RetryGame()
    {
        PlayerPrefs.DeleteKey("FinalScore");
        PlayerPrefs.DeleteKey("FinalLevel");
        SceneManager.LoadScene("GameScene");
    }
}
