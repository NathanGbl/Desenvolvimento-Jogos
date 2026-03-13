using UnityEngine;

namespace SpaceShipGame
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 1.1f;

        private float timer;
        private int wave;
        private Sprite[] enemySprites;

        private void Start()
        {
            enemySprites = new Sprite[3];
            for (int i = 0; i < 3; i++)
            {
                Texture2D tex = Resources.Load<Texture2D>($"Ship0{i + 2}");
                if (tex != null)
                {
                    enemySprites[i] = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f),
                        100f);
                }
            }
        }

        private void Update()
        {
            if (SpaceShipGameManager.Instance == null || SpaceShipGameManager.Instance.Ended)
            {
                return;
            }

            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnEnemy();
                wave++;
                spawnInterval = Mathf.Max(0.35f, 1.1f - wave * 0.02f);
            }
        }

        private void SpawnEnemy()
        {
            float y = Random.Range(-4.5f, 4.5f);
            GameObject enemy = new GameObject("EnemyShip");
            enemy.tag = "Enemy";
            enemy.transform.position = new Vector3(9.2f, y, 0f);

            SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 6;

            Sprite chosen = enemySprites != null ? enemySprites[Random.Range(0, enemySprites.Length)] : null;
            if (chosen != null)
            {
                sr.sprite = chosen;
                sr.color = Color.white;
                float s = 1.2f / Mathf.Max(chosen.bounds.size.x, chosen.bounds.size.y);
                enemy.transform.localScale = Vector3.one * s;
            }
            else
            {
                sr.sprite = SpaceShipSpriteFactory.GetSquareSprite();
                sr.color = Color.Lerp(new Color(0.9f, 0.25f, 0.25f), new Color(1f, 0.8f, 0.35f), Random.value);
                enemy.transform.localScale = new Vector3(0.7f, 0.5f, 1f);
            }

            BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            EnemyShipController controller = enemy.AddComponent<EnemyShipController>();
            int points = Random.value < 0.2f ? 30 : (Random.value < 0.5f ? 20 : 10);
            float fireChance = Mathf.Lerp(0.08f, 0.28f, Mathf.Clamp01(wave / 80f));
            controller.Setup(points, fireChance);
        }
    }
}
