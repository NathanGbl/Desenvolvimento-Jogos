using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private string level1SceneName = "Level_1";

    private float invulnerabilityTimer;
    private PlayerFaithShield faithShield;

    private void Awake()
    {
        faithShield = GetComponent<PlayerFaithShield>();
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int amount)
    {
        if (invulnerabilityTimer > 0f)
        {
            return;
        }

        if (faithShield != null && faithShield.IsActive)
        {
            return;
        }

        invulnerabilityTimer = invulnerabilityDuration;
        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.DamagePlayer(amount);
        }
    }

    public void RestartRunFromBeginning()
    {
        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.StartNewRun();
        }

        SceneManager.LoadScene(level1SceneName);
    }
}
