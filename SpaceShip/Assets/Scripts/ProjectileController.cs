using UnityEngine;

namespace SpaceShipGame
{
    public enum ProjectileOwner
    {
        Player,
        Enemy
    }

    public class ProjectileController : MonoBehaviour
    {
        public ProjectileOwner owner;
        public float speed = 10f;

        private void Update()
        {
            float direction = owner == ProjectileOwner.Player ? 1f : -1f;
            transform.position += Vector3.up * (direction * speed * Time.deltaTime);

            if (transform.position.y > 6f || transform.position.y < -6f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (owner == ProjectileOwner.Player)
            {
                if (other.CompareTag("Enemy"))
                {
                    EnemyShipController enemy = other.GetComponent<EnemyShipController>();
                    if (enemy != null)
                    {
                        enemy.TakeHit();
                    }

                    Destroy(gameObject);
                }
            }
            else
            {
                if (other.CompareTag("Player"))
                {
                    SpaceShipGameManager.Instance?.DamagePlayer();
                    Destroy(gameObject);
                }
            }
        }
    }
}
