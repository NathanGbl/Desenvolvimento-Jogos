using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text collectiblesText;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private PlayerInteractor playerInteractor;

    private void Update()
    {
        if (GameFlow.Instance == null)
        {
            return;
        }

        if (scoreText != null)
        {
            scoreText.text = $"Pontos: {GameFlow.Instance.Score}";
        }

        if (livesText != null)
        {
            livesText.text = $"Vidas: {GameFlow.Instance.Lives}";
        }

        if (collectiblesText != null)
        {
            collectiblesText.text = $"Itens: {GameFlow.Instance.TotalCollectiblesCollected}/{GameFlow.Instance.TotalCollectiblesRequired}";
        }

        if (interactText != null)
        {
            interactText.text = playerInteractor != null ? playerInteractor.CurrentPrompt : string.Empty;
        }
    }
}
