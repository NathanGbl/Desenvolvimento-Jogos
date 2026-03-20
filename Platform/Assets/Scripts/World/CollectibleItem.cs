using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool requiredForVictory = true;

    public bool RequiredForVictory => requiredForVictory;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (GameFlow.Instance != null)
        {
            if (requiredForVictory)
            {
                GameFlow.Instance.AddCollectible(scoreValue);
            }
            else
            {
                GameFlow.Instance.AddScore(scoreValue);
            }
        }

        Destroy(gameObject);
    }
}
