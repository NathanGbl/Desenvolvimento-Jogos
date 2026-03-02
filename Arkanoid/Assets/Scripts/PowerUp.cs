using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { ExpandPaddle, FastBall, SlowBall, ExtraLife }
    
    [Header("PowerUp Settings")]
    private PowerUpType type;
    public float fallSpeed = 2f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Configura as propriedades do Rigidbody
        rb.gravityScale = 1f;
        rb.velocity = Vector2.down * fallSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ApplyPowerUp(collision.gameObject);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ApplyPowerUp(collision.gameObject);
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        // Seleciona um power-up aleatório
        int randomType = Random.Range(0, 4);
        type = (PowerUpType)randomType;
        
        // Define a cor baseada no tipo
        switch(type)
        {
            case PowerUpType.ExpandPaddle:
                if (spriteRenderer != null) spriteRenderer.color = Color.green;
                break;
            case PowerUpType.FastBall:
                if (spriteRenderer != null) spriteRenderer.color = Color.red;
                break;
            case PowerUpType.SlowBall:
                if (spriteRenderer != null) spriteRenderer.color = Color.cyan;
                break;
            case PowerUpType.ExtraLife:
                if (spriteRenderer != null) spriteRenderer.color = Color.yellow;
                break;
        }
    }

    void ApplyPowerUp(GameObject paddle)
    {
        Paddle paddleScript = paddle.GetComponent<Paddle>();
        Ball ballScript = FindObjectOfType<Ball>();
        
        switch(type)
        {
            case PowerUpType.ExpandPaddle:
                if (paddleScript != null)
                {
                    paddleScript.ExpandPaddle(5f);
                }
                break;
                
            case PowerUpType.FastBall:
                if (ballScript != null)
                {
                    ballScript.initialSpeed += 2f;
                }
                break;
                
            case PowerUpType.SlowBall:
                if (ballScript != null)
                {
                    ballScript.initialSpeed = Mathf.Max(ballScript.initialSpeed - 1f, 2f);
                }
                break;
                
            case PowerUpType.ExtraLife:
                GameManager gameManager = GameManager.Instance;
                if (gameManager != null)
                {
                    // Adiciona uma vida via reflexão ou método público
                    gameManager.AddScore(0); // Placeholder
                }
                break;
        }
    }
}
