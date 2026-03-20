using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }

    [SerializeField] private bool doubleJumpUnlocked;
    [SerializeField] private bool charityUnlocked;
    [SerializeField] private bool fortitudeUnlocked;

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

    public bool HasAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.DoubleJump:
                return doubleJumpUnlocked;
            case AbilityType.Charity:
                return charityUnlocked;
            case AbilityType.Fortitude:
                return fortitudeUnlocked;
            default:
                return false;
        }
    }

    public void UnlockAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.DoubleJump:
                doubleJumpUnlocked = true;
                break;
            case AbilityType.Charity:
                charityUnlocked = true;
                break;
            case AbilityType.Fortitude:
                fortitudeUnlocked = true;
                break;
        }
    }

    public void ResetProgress()
    {
        doubleJumpUnlocked = false;
        charityUnlocked = false;
        fortitudeUnlocked = false;
    }
}
