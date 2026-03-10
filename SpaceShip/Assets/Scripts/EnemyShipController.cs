using UnityEngine;

namespace SpaceShipGame
{
    public class EnemyShipController : MonoBehaviour
    {
        private int scoreValue;
        private float fireChancePerSecond;
        private bool dead;

        public void Setup(int points, float fireChance)
        {
            scoreValue = points;
            fireChancePerSecond = fireChance;
        }

        private void Update()
        {
            if (SpaceShipGameManager.Instance == null || SpaceShipGameManager.Instance.Ended)
            {
                return;
            }

            transform.position += Vector3.down * (1.2f * Time.deltaTime);
            transform.position += Vector3.right * (Mathf.Sin(Time.time * 3f + transform.position.y) * 0.9f * Time.deltaTime);

            if (Random.value < fireChancePerSecond * Time.deltaTime)
            {
                Fire();
            }

            if (transform.position.y < -5.3f)
            {
                SpaceShipGameManager.Instance.EndGame("DefeatScene");
            }
        }

        private void Fire()
        {
            GameObject bullet = new GameObject("EnemyBullet");
            bullet.tag = "EnemyBullet";
            bullet.transform.position = transform.position + Vector3.down * 0.35f;
            bullet.transform.localScale = new Vector3(0.1f, 0.28f, 1f);

            SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
            sr.sprite = SpaceShipSpriteFactory.GetSquareSprite();
            sr.color = new Color(1f, 0.5f, 0.45f);
            sr.sortingOrder = 7;

            BoxCollider2D collider = bullet.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            ProjectileController projectile = bullet.AddComponent<ProjectileController>();
            projectile.owner = ProjectileOwner.Enemy;
            projectile.speed = 7f;
        }

        public void TakeHit()
        {
            if (dead)
            {
                return;
            }

            dead = true;
            SpaceShipGameManager.Instance?.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
