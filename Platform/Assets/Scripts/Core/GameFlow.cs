using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance { get; private set; }

    [Header("Configuração")]
    [SerializeField] private int initialLives = 3;
    [SerializeField] private string endSceneName = "EndScene";

    private readonly HashSet<string> registeredScenes = new HashSet<string>();

    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int TotalCollectiblesRequired { get; private set; }
    public int TotalCollectiblesCollected { get; private set; }
    public GameResult LastResult { get; private set; } = GameResult.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartNewRun();
    }

    public void StartNewRun()
    {
        Score = 0;
        Lives = initialLives;
        TotalCollectiblesRequired = 0;
        TotalCollectiblesCollected = 0;
        LastResult = GameResult.None;
        registeredScenes.Clear();

        PlayerProgress progress = PlayerProgress.Instance;
        if (progress != null)
        {
            progress.ResetProgress();
        }
    }

    public void RegisterSceneCollectibles(string sceneName, int requiredCount)
    {
        if (registeredScenes.Contains(sceneName))
        {
            return;
        }

        registeredScenes.Add(sceneName);
        TotalCollectiblesRequired += Mathf.Max(0, requiredCount);
    }

    public void AddScore(int amount)
    {
        Score += Mathf.Max(0, amount);
    }

    public void AddCollectible(int scoreValue)
    {
        TotalCollectiblesCollected++;
        AddScore(scoreValue);
    }

    public void DamagePlayer(int amount)
    {
        Lives -= Mathf.Max(1, amount);
        if (Lives <= 0)
        {
            Lives = 0;
            EndGame(false);
        }
    }

    public bool HasAllCollectibles()
    {
        return TotalCollectiblesRequired > 0 && TotalCollectiblesCollected >= TotalCollectiblesRequired;
    }

    public void EndGame(bool victory)
    {
        LastResult = victory ? GameResult.Victory : GameResult.Defeat;
        SceneManager.LoadScene(endSceneName);
    }
}
