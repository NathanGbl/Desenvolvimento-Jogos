using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int scoreValue = 10;
    public EnemyType enemyType = EnemyType.Type1;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float shootChance = 0.001f; // Chance por frame

    private EnemyManager enemyManager;

    public enum EnemyType
    {
        Type1 = 10,  // 10 pontos
        Type2 = 20,  // 20 pontos
        Type3 = 30   // 30 pontos
    }

    void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        scoreValue = (int)enemyType;
    }

    void Update()
    {
        // Chance aleatória de atirar
        if (bulletPrefab != null && Random.value < shootChance * GameManager.Instance.speedMultiplier)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.isPlayerBullet = false;
            }
        }
    }

    public void Die()
    {
        // Adiciona pontuação
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.EnemyDestroyed();
        }

        // Notifica o EnemyManager
        if (enemyManager != null)
        {
            enemyManager.EnemyDestroyed(this);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Se o inimigo atingir a parte inferior da tela
        if (other.CompareTag("DeathZone"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.lives = 0;
                GameManager.Instance.LoseLife();
            }
        }
    }
}
