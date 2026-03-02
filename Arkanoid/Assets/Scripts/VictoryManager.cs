using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [Header("Victory UI")]
    public Text titleText;
    public Text scoreText;
    public Text levelText;
    public Button menuButton;
    public Button restartButton;

    private int finalScore;
    private int finalLevel;

    void Start()
    {
        // Recuperar dados do GameManager se persistir
        finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        finalLevel = PlayerPrefs.GetInt("FinalLevel", 1);

        // Configurar textos
        if (titleText != null)
        {
            titleText.text = "VITÓRIA!";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Pontuação Final: {finalScore}";
        }

        if (levelText != null)
        {
            levelText.text = $"Níveis Completados: {finalLevel}";
        }

        // Configurar botões
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
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
            RestartGame();
        }
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    void RestartGame()
    {
        PlayerPrefs.DeleteKey("FinalScore");
        PlayerPrefs.DeleteKey("FinalLevel");
        SceneManager.LoadScene("GameScene");
    }
}
