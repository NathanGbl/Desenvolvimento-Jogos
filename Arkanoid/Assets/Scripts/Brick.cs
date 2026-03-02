using UnityEngine;

public class Brick : MonoBehaviour
{
    public ArkanoidGameManager manager;
    public int points = 100;

    public void Break()
    {
        manager?.NotifyBrickDestroyed(transform.position, points);
        Destroy(gameObject);
    }
}
