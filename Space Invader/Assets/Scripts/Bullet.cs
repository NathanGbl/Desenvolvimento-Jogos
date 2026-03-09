using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public bool isPlayerBullet = true;
    public float destroyBoundary = 10f;

    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        
        if (rb2d == null)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.gravityScale = 0;

        // Define a direção baseado em quem disparou
        float direction = isPlayerBullet ? 1f : -1f;
        rb2d.linearVelocity = new Vector2(0, speed * direction);

        // Tag apropriada
        gameObject.tag = isPlayerBullet ? "PlayerBullet" : "EnemyBullet";
    }

    void Update()
    {
        // Destroi o projétil se sair da tela
        if (Mathf.Abs(transform.position.y) > destroyBoundary)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet)
        {
            // Projétil do jogador
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                }
                Destroy(gameObject);
            }
        }
        else
        {
            // Projétil inimigo
            if (other.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }

        // Destroi ao atingir paredes/limites
        if (other.CompareTag("Wall") || other.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }
    }
}
