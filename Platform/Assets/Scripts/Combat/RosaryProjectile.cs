using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RosaryProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 14f;
    [SerializeField] private float maxLifeTime = 3f;

    private float direction = 1f;

    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * (direction * speed * Time.deltaTime), Space.World);
    }

    public void SetDirection(float newDirection)
    {
        direction = Mathf.Sign(newDirection);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
