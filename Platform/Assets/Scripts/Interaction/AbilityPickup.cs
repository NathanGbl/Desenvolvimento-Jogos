using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    [SerializeField] private AbilityType abilityToUnlock;
    [SerializeField] private int bonusScore = 100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.UnlockAbility(abilityToUnlock);
        }

        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.AddScore(bonusScore);
        }

        Destroy(gameObject);
    }
}
