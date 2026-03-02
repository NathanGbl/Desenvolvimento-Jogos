using UnityEngine;

public class DeathZoneTrigger : MonoBehaviour
{
    public ArkanoidGameManager manager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball"))
        {
            return;
        }

        manager?.BallLost();
    }
}
