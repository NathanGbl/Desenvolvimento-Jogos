using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Brick Settings")]
    public int pointValue = 10;
    public float powerUpDropChance = 0.3f; // 30% de chance de droparvou criar o GameManager, que é crucial para gerenciar o jogo todo.power-up
    
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        gameObject.tag = "Brick";
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            DestroyBrick();
        }
    }

    void DestroyBrick()
    {
        // Notifica o GameManager
        GameManager instance = GameManager.Instance;
        if (instance != null)
        {
            instance.AddScore(pointValue);
            instance.BrickDestroyed();
        }

        // Chance de dropar power-up
        if (Random.value < powerUpDropChance)
        {
            SpawnPowerUp();
        }
        
        // Destroi o bloco
        Destroy(gameObject);
    }

    void SpawnPowerUp()
    {
        PowerUp powerUpPrefab = Resources.Load<PowerUp>("Prefabs/PowerUp");
        if (powerUpPrefab != null)
        {
            PowerUp powerUp = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            powerUp.Initialize();
        }
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}
