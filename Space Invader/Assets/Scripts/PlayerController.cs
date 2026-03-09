using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float leftBound = -8f;
    public float rightBound = 8f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;

    private float nextFireTime = 0f;
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

        // Se não há firePoint definido, usa a própria posição
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        Vector2 velocity = new Vector2(horizontalInput * moveSpeed, 0);
        rb2d.linearVelocity = velocity;

        // Limita a posição do jogador
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        transform.position = pos;
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            TakeDamage();
        }
        else if (other.CompareTag("Enemy"))
        {
            // Se um inimigo tocar o jogador, game over imediato
            if (GameManager.Instance != null)
            {
                GameManager.Instance.lives = 0;
                GameManager.Instance.LoseLife();
            }
        }
    }

    void TakeDamage()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoseLife();
        }
    }
}
