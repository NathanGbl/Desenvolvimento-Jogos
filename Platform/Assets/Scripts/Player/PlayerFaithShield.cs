using UnityEngine;

public class PlayerFaithShield : MonoBehaviour
{
    [SerializeField] private KeyCode shieldKey = KeyCode.K;
    [SerializeField] private float activeDuration = 1.5f;
    [SerializeField] private float cooldown = 8f;
    [SerializeField] private GameObject shieldVisual;

    private float activeTimer;
    private float cooldownTimer;

    public bool IsActive => activeTimer > 0f;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (activeTimer > 0f)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0f && shieldVisual != null)
            {
                shieldVisual.SetActive(false);
            }
        }

        if (Input.GetKeyDown(shieldKey) && cooldownTimer <= 0f && activeTimer <= 0f)
        {
            ActivateShield();
        }
    }

    private void ActivateShield()
    {
        activeTimer = activeDuration;
        cooldownTimer = cooldown;

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }
    }
}
