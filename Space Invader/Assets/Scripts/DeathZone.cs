using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Se um inimigo atingir a zona de morte (parte inferior), game over
            if (GameManager.Instance != null)
            {
                GameManager.Instance.lives = 0;
                GameManager.Instance.LoseLife();
            }
        }
    }
}
