using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        if (GameFlow.Instance == null)
        {
            return;
        }

        bool won = GameFlow.Instance.LastResult == GameResult.Victory;
        titleText.text = won ? "Vitória" : "Derrota";
        summaryText.text = $"Pontos: {GameFlow.Instance.Score}\nItens: {GameFlow.Instance.TotalCollectiblesCollected}/{GameFlow.Instance.TotalCollectiblesRequired}";
    }

    public void RestartRun()
    {
        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.StartNewRun();
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
