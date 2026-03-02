using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Settings")]
    public float initialSpeed = 5f;
    public float maxSpeed = 15f;
    public float speedIncrease = 0.5f;
    
    private Rigidbody2D rb;
    private Vector2 direction;
    private bool isLaunched = false;
    private Paddle paddle;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        paddle = FindObjectOfType<Paddle>();
        currentSpeed = initialSpeed;
        
        // Coloca a bola acima da nave no início
        if (paddle != null)
        {
            transform.position = new Vector3(paddle.transform.position.x, paddle.transform.position.y + 1f, 0);
        }
    }

    void Update()
    {
        // Se a bola ainda não foi lançada, segue a nave
        if (!isLaunched && paddle != null)
        {
            transform.position = new Vector3(paddle.transform.position.x, paddle.transform.position.y + 1f, 0);
        }

        // Pressiona espaço para lançar a bola
        if (Input.GetKeyDown(KeyCode.Space) && !isLaunched)
        {
            Launch();
        }

        // Limita a velocidade máxima
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void Launch()
    {
        isLaunched = true;
        
        // Lança em um ângulo aleatório (não reto para cima)
        float angle = Random.Range(-45f, 45f);
        direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
        
        rb.velocity = direction * currentSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Colisão com bloco
        if (collision.gameObject.CompareTag("Brick"))
        {
            HandleBrickCollision(collision);
        }
        
        // Colisão com a nave
        if (collision.gameObject.CompareTag("Paddle"))
        {
            HandlePaddleCollision(collision);
        }
        
        // Colisão com paredes
        if (collision.gameObject.CompareTag("Wall"))
        {
            HandleWallCollision(collision);
        }
    }

    void HandleBrickCollision(Collision2D collision)
    {
        // Aumenta a velocidade levemente
        currentSpeed = Mathf.Min(currentSpeed + speedIncrease, maxSpeed);
        
        // Calcula a nova direção de rebote usando a normal do contato
        Vector2 normal = collision.contacts[0].normal;
        Vector2 newVelocity = Vector2.Reflect(rb.velocity.normalized, normal);
        rb.velocity = newVelocity * currentSpeed;
        
        // Destroi o bloco (o próprio Brick notificará o GameManager)
        Destroy(collision.gameObject);
    }

    void HandlePaddleCollision(Collision2D collision)
    {
        if (!isLaunched) return;

        // Calcula o ponto de colisão na nave
        float paddleWidth = collision.gameObject.GetComponent<BoxCollider2D>().size.x;
        float paddleCenter = collision.transform.position.x;
        float hitPos = collision.relativeVelocity.x;

        // Gera ângulo de rebote dependendo de onde bateu na nave
        float hitFactor = (collision.transform.position.x - transform.position.x) / (paddleWidth / 2f);
        hitFactor = Mathf.Clamp(hitFactor, -1f, 1f);

        float angle = hitFactor * 60f; // Até 60 graus para cada lado
        direction = Quaternion.Euler(0, 0, angle) * Vector2.up;

        rb.velocity = direction * currentSpeed;
    }

    void HandleWallCollision(Collision2D collision)
    {
        // Rebote simples nas paredes usando a normal do contato
        Vector2 normal = collision.contacts[0].normal;
        Vector2 newVelocity = Vector2.Reflect(rb.velocity.normalized, normal);
        rb.velocity = newVelocity * currentSpeed;
    }

    public void Reset()
    {
        isLaunched = false;
        currentSpeed = initialSpeed;
        rb.velocity = Vector2.zero;
        
        if (paddle != null)
        {
            transform.position = new Vector3(paddle.transform.position.x, paddle.transform.position.y + 1f, 0);
        }
    }

    public bool IsLaunched => isLaunched;
}
