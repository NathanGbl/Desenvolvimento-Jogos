using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool isFinalLevel;
    [SerializeField] private bool requireAllCollectibles = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        bool canProceed = !requireAllCollectibles || (GameFlow.Instance != null && GameFlow.Instance.HasAllCollectibles());
        if (!canProceed)
        {
            return;
        }

        if (isFinalLevel)
        {
            if (GameFlow.Instance != null)
            {
                GameFlow.Instance.EndGame(true);
            }

            return;
        }

        if (!string.IsNullOrWhiteSpace(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
