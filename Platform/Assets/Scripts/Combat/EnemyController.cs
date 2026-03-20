using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private int scoreOnDefeat = 50;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    private Transform currentTarget;

    private void Start()
    {
        currentTarget = pointB != null ? pointB : transform;
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (pointA == null || pointB == null)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(contactDamage);
        }
    }

    public void Defeat()
    {
        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.AddScore(scoreOnDefeat);
        }

        Destroy(gameObject);
    }
}
