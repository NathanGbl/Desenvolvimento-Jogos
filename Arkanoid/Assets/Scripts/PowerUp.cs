using UnityEngine;

public enum PowerUpType
{
    ExpandPaddle,
    SpeedBall,
    ExtraLife
}

public class PowerUp : MonoBehaviour
{
    public ArkanoidGameManager manager;
    public PowerUpType type;

    public Color GetColor()
    {
        switch (type)
        {
            case PowerUpType.ExpandPaddle:
                return new Color(0.45f, 0.9f, 1f, 1f);
            case PowerUpType.SpeedBall:
                return new Color(1f, 0.85f, 0.3f, 1f);
            case PowerUpType.ExtraLife:
                return new Color(0.5f, 1f, 0.5f, 1f);
            default:
                return Color.white;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Paddle"))
        {
            return;
        }

        manager?.ApplyPowerUp(type);
        Destroy(gameObject);
    }
}
