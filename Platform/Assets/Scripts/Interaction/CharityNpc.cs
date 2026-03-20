using UnityEngine;

public class CharityNpc : MonoBehaviour, IInteractable
{
    [SerializeField] private string promptText = "Pressione E para purificar";
    [SerializeField] private SecretPassageActivator passageActivator;
    [SerializeField] private int scoreOnPurify = 150;

    private bool purified;

    public string PromptText => promptText;

    public void Interact(GameObject interactor)
    {
        if (purified)
        {
            return;
        }

        if (PlayerProgress.Instance == null || !PlayerProgress.Instance.HasAbility(AbilityType.Charity))
        {
            return;
        }

        purified = true;

        if (passageActivator != null)
        {
            passageActivator.ActivatePassage();
        }

        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.AddScore(scoreOnPurify);
        }

        gameObject.SetActive(false);
    }
}
