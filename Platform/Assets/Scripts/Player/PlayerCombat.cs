using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private LightProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 0.25f;

    private PlayerController2D controller;
    private float shootTimer;

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.J) && shootTimer <= 0f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }

        float direction = controller != null && !controller.IsFacingRight ? -1f : 1f;
        LightProjectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.SetDirection(direction);
        shootTimer = shootCooldown;
    }
}
