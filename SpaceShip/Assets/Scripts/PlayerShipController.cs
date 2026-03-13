using UnityEngine;

namespace SpaceShipGame
{
    public class PlayerShipController : MonoBehaviour
    {
        [SerializeField] private float speed = 7.5f;
        [SerializeField] private float fireCooldown = 0.18f;

        private float nextFireTime;

        private void Update()
        {
            if (SpaceShipGameManager.Instance == null || SpaceShipGameManager.Instance.Ended)
            {
                return;
            }

            float horizontal = SpaceShipInput.HorizontalRaw();
            float vertical = SpaceShipInput.VerticalRaw();

            Vector3 delta = new Vector3(horizontal, vertical, 0f) * (speed * Time.deltaTime);
            transform.position += delta;

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -8.2f, -1.2f);
            pos.y = Mathf.Clamp(pos.y, -4.5f, 4.5f);
            transform.position = pos;

            bool wantsFire = SpaceShipInput.FireHeld() || SpaceShipInput.FirePressedThisFrame();
            if (wantsFire && Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireCooldown;
            }
        }

        private void Fire()
        {
            GameObject bullet = new GameObject("PlayerBullet");
            bullet.tag = "PlayerBullet";
            bullet.transform.position = transform.position + Vector3.right * 0.7f;
            bullet.transform.localScale = new Vector3(70f, 22f, 1f);

            SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
            sr.sprite = SpaceShipSpriteFactory.GetSquareSprite();
            sr.color = new Color(0.2f, 1f, 1f);
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
