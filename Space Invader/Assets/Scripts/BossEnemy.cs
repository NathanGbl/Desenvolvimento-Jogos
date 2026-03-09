using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Boss Settings")]
    public int scoreValue = 50;
    public float speed = 5f;
    public float destroyX = 10f;

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

        // Move da esquerda para a direita
        rb2d.linearVelocity = new Vector2(speed, 0);

        gameObject.tag = "Enemy";
    }

    void Update()
    {
        // Destroi ao sair da tela
        if (transform.position.x > destroyX)
        {
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        // Adiciona pontuação especial
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            Die();
        }
    }
}
