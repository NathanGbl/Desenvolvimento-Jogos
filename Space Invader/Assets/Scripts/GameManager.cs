using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int lives = 3;
    public int score = 0;
    public int level = 1;

    [Header("UI References")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

    [Header("Game Settings")]
    public float speedMultiplier = 1.0f;
    public float speedIncreasePerEnemy = 0.02f;

    private int totalEnemies;
    private int enemiesDestroyed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        EnsureRuntimeHUD();
        UpdateUI();
    }

    void EnsureRuntimeHUD()
    {
        if (livesText != null && scoreText != null && levelText != null)
        {
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("HUDCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        if (livesText == null)
        {
            livesText = CreateHudText("LivesText", canvas.transform, new Vector2(20f, -20f), TextAlignmentOptions.Left);
        }

        if (scoreText == null)
        {
            scoreText = CreateHudText("ScoreText", canvas.transform, new Vector2(20f, -55f), TextAlignmentOptions.Left);
        }

        if (levelText == null)
        {
            levelText = CreateHudText("LevelText", canvas.transform, new Vector2(-20f, -20f), TextAlignmentOptions.Right, true);
        }
    }

    TextMeshProUGUI CreateHudText(string objectName, Transform parent, Vector2 anchoredPos, TextAlignmentOptions alignment, bool anchorRight = false)
    {
        GameObject textObj = new GameObject(objectName);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(320f, 30f);
        rect.anchorMin = anchorRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
        rect.anchorMax = rect.anchorMin;
        rect.pivot = anchorRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPos;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 28;
        text.color = Color.white;
        text.alignment = alignment;
        text.text = string.Empty;
        return text;
    }

    public void SetTotalEnemies(int count)
    {
        totalEnemies = count;
        enemiesDestroyed = 0;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    public void LoseLife()
    {
        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void EnemyDestroyed()
    {
        enemiesDestroyed++;
        
        // Aumenta velocidade conforme inimigos são destruídos
        speedMultiplier += speedIncreasePerEnemy;

        if (enemiesDestroyed >= totalEnemies)
        {
            Victory();
        }
    }

    void UpdateUI()
    {
        if (livesText != null)
            livesText.text = "Vidas: " + lives;
        
        if (scoreText != null)
            scoreText.text = "Pontos: " + score;
        
        if (levelText != null)
            levelText.text = "Nivel: " + level;
    }

    void GameOver()
    {
        SceneManager.LoadScene("DefeatScene");
    }

    void Victory()
    {
        SceneManager.LoadScene("VictoryScene");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
