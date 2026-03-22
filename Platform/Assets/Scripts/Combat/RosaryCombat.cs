using UnityEngine;

public class RosaryCombat : MonoBehaviour
{
    [Header("Rosario")]
    [SerializeField] private RosaryProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.25f;
    [SerializeField] private KeyCode fireKey = KeyCode.J;

    [Header("Escudo de Fe (futuro)")]
    [SerializeField] private bool faithShieldEnabled;
    [SerializeField] private float faithShieldCooldown = 6f;

    private PlayerMovement movement;
    private float fireTimer;
    private float shieldTimer;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (fireTimer > 0f)
        {
            fireTimer -= Time.deltaTime;
        }

        if (shieldTimer > 0f)
        {
            shieldTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(fireKey) && fireTimer <= 0f)
        {
            FireLightShot();
        }

        if (faithShieldEnabled && Input.GetKeyDown(KeyCode.K) && shieldTimer <= 0f)
        {
            ActivateFaithShieldPlaceholder();
            shieldTimer = faithShieldCooldown;
        }
    }

    private void FireLightShot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }

        float direction = movement != null ? movement.FacingSign() : 1f;
        RosaryProjectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.SetDirection(direction);
        fireTimer = fireCooldown;
    }

    private void ActivateFaithShieldPlaceholder()
    {
        // Placeholder para futura implementacao do Escudo de Fe.
    }
}
