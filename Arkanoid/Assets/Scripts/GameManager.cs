using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public int initialLives = 3;
    public int totalBricksPerLevel = 0;
    public int currentLevel = 1;
    public float difficultyMultiplier = 1.1f;
    
    [Header("References")]
    private Ball ball;
    private Paddle paddle;
    private UIManager uiManager;
    
    private int lives;
    private int score = 0;
    private int bricksDestroyed = 0;
    private bool gameOver = false;
    private bool levelWon = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    void Start()
    {
        ball = FindObjectOfType<Ball>();
        paddle = FindObjectOfType<Paddle>();
        uiManager = FindObjectOfType<UIManager>();
        
        lives = initialLives;
        totalBricksPerLevel = FindObjectsOfType<Brick>().Length;
        bricksDestroyed = 0;
        
        UpdateUI();
    }

    void Update()
    {
        // Verifica se a bola caiu
        if (ball != null && ball.IsLaunched && ball.transform.position.y < -10f)
        {
            HandleBallLost();
        }

        // Verifica se ganhou o nível
        if (totalBricksPerLevel > 0 && bricksDestroyed >= totalBricksPerLevel && !levelWon)
        {
            LevelWon();
        }

        // Restart com R
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        // Próximo nível com N
        if (levelWon && Input.GetKeyDown(KeyCode.N))
        {
            NextLevel();
        }
    }

    void HandleBallLost()
    {
        lives--;
        
        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            ball.Reset();
            if (uiManager != null)
            {
                uiManager.ShowMessage($"Vidas: {lives}");
            }
        }
        
        UpdateUI();
    }

    void GameOver()
    {
        gameOver = true;
        
        // Salvar dados para a cena de derrota
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("FinalLevel", currentLevel);
        PlayerPrefs.Save();
        
        // Carregar cena de derrota após pequeno delay
        Invoke("LoadDefeatScene", 1.5f);
    }
    
    void LoadDefeatScene()
    {
        SceneManager.LoadScene("DefeatScene");
    }

    void LevelWon()
    {
        levelWon = true;
        
        // Salvar dados para a cena de vitória 
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("FinalLevel", currentLevel);
        PlayerPrefs.Save();
        
        // Carregar cena de vitória após pequeno delay
        Invoke("LoadVictoryScene", 1.5f);
    }
    
    void LoadVictoryScene()
    {
        SceneManager.LoadScene("VictoryScene");
    }

    void NextLevel()
    {
        currentLevel++;
        
        // Aumenta a dificuldade
        Ball ballScript = ball.GetComponent<Ball>();
        if (ballScript != null)
        {
            ballScript.initialSpeed *= difficultyMultiplier;
            ballScript.maxSpeed *= difficultyMultiplier;
        }
        
        // Recarrega a cena ou avança para o próximo nível
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void RestartGame()
    {
        currentLevel = 1;
        score = 0;
        lives = initialLives;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    public void BrickDestroyed()
    {
        bricksDestroyed++;
    }

    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(score);
            uiManager.UpdateLives(lives);
            uiManager.UpdateLevel(currentLevel);
        }
    }

    public int GetScore() => score;
    public int GetLives() => lives;
    public int GetCurrentLevel() => currentLevel;
}
