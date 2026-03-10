using UnityEngine;

namespace SpaceShipGame
{
    public class PlayerShipController : MonoBehaviour
    {
        [SerializeField] private float speed = 7.5f;
        [SerializeField] private float fireCooldown = 0.18f;

        private float lastFireTime;

        private void Update()
        {
            if (SpaceShipGameManager.Instance == null || SpaceShipGameManager.Instance.Ended)
            {
                return;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 delta = new Vector3(horizontal, vertical, 0f) * (speed * Time.deltaTime);
            transform.position += delta;

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -4.6f, 4.6f);
            pos.y = Mathf.Clamp(pos.y, -4.4f, 1.4f);
            transform.position = pos;

            if (Input.GetKey(KeyCode.Space) && Time.time - lastFireTime > fireCooldown)
            {
                lastFireTime = Time.time;
                Fire();
            }
        }

        private void Fire()
        {
            GameObject bullet = new GameObject("PlayerBullet");
            bullet.tag = "PlayerBullet";
            bullet.transform.position = transform.position + Vector3.up * 0.45f;
            bullet.transform.localScale = new Vector3(0.12f, 0.35f, 1f);

            SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
            sr.sprite = SpaceShipSpriteFactory.GetSquareSprite();
            sr.color = new Color(0.7f, 1f, 1f);
            sr.sortingOrder = 8;

            BoxCollider2D collider = bullet.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            ProjectileController projectile = bullet.AddComponent<ProjectileController>();
            projectile.owner = ProjectileOwner.Player;
            projectile.speed = 12f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
            {
                Destroy(other.gameObject);
                SpaceShipGameManager.Instance?.DamagePlayer();
            }
        }
    }
}
