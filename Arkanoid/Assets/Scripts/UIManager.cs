using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Text livesText;
    public Text levelText;
    public Text messageText;
    public Canvas pauseMenuCanvas;
    
    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Pontos: {score}";
        }
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = $"Vidas: {lives}";
        }
    }

    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Nível: {level}";
        }
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            Invoke("HideMessage", 2f);
        }
    }

    void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    public void ShowGameOver(int finalScore)
    {
        if (messageText != null)
        {
            messageText.text = $"GAME OVER\nPontos Finais: {finalScore}\nPressione R para reiniciar";
            messageText.gameObject.SetActive(true);
        }
    }

    public void ShowLevelWon(int level)
    {
        if (messageText != null)
        {
            messageText.text = $"NÍVEL {level} COMPLETO!\nPressione N para próximo nível";
            messageText.gameObject.SetActive(true);
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.gameObject.SetActive(isPaused);
        }
        
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public bool IsPaused => isPaused;
}
