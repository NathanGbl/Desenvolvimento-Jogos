using UnityEngine;

public class LightProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 14f;
    [SerializeField] private float lifeTime = 3f;

    private float direction = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * (direction * speed * Time.deltaTime), Space.World);
    }

    public void SetDirection(float newDirection)
    {
        direction = Mathf.Sign(newDirection);

        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * direction;
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Defeat();
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Barreira_Fisica"))
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
