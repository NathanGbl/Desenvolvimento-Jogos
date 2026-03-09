using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemyType1Prefab;
    public GameObject enemyType2Prefab;
    public GameObject enemyType3Prefab;
    public GameObject bossPrefab;

    [Header("Grid Settings")]
    public int rows = 5;
    public int columns = 11;
    public float horizontalSpacing = 1f;
    public float verticalSpacing = 0.8f;
    public Vector2 gridStartPosition = new Vector2(-5f, 4f);

    [Header("Movement Settings")]
    public float baseSpeed = 2f;
    public float moveDistance = 0.5f;
    public float downDistance = 0.5f;
    public float moveInterval = 1f;

    [Header("Boss Settings")]
    public float bossSpawnInterval = 40f; // 30-50 segundos
    public float bossSpawnVariation = 10f;
    public Vector2 bossSpawnPosition = new Vector2(-10f, 4.5f);

    private List<Enemy> enemies = new List<Enemy>();
    private float moveTimer = 0f;
    private int moveDirection = 1; // 1 = direita, -1 = esquerda
    private int moveCount = 0;
    private float bossTimer = 0f;
    private float nextBossSpawnTime;

    void Start()
    {
        SpawnEnemyGrid();
        nextBossSpawnTime = Random.Range(bossSpawnInterval - bossSpawnVariation, 
                                         bossSpawnInterval + bossSpawnVariation);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetTotalEnemies(enemies.Count);
        }
    }

    void SpawnEnemyGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject enemyPrefab = GetEnemyPrefabForRow(row);
                
                if (enemyPrefab != null)
                {
                    Vector2 spawnPos = gridStartPosition + new Vector2(
                        col * horizontalSpacing,
                        -row * verticalSpacing
                    );

                    GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                    enemyObj.transform.parent = transform;

                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemies.Add(enemy);
                        
                        // Define o tipo baseado na linha
                        if (row == 0)
                            enemy.enemyType = Enemy.EnemyType.Type3;
                        else if (row <= 2)
                            enemy.enemyType = Enemy.EnemyType.Type2;
                        else
                            enemy.enemyType = Enemy.EnemyType.Type1;
                    }
                }
            }
        }
    }

    GameObject GetEnemyPrefabForRow(int row)
    {
        if (row == 0)
            return enemyType3Prefab ?? enemyType1Prefab;
        else if (row <= 2)
            return enemyType2Prefab ?? enemyType1Prefab;
        else
            return enemyType1Prefab;
    }

    void Update()
    {
        MoveEnemies();
        HandleBossSpawn();
    }

    void MoveEnemies()
    {
        if (enemies.Count == 0) return;

        moveTimer += Time.deltaTime;

        float adjustedInterval = moveInterval / GameManager.Instance.speedMultiplier;

        if (moveTimer >= adjustedInterval)
        {
            moveTimer = 0f;
            moveCount++;

            // Move todos os inimigos
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                {
                    Vector3 pos = enemy.transform.position;
                    pos.x += moveDistance * moveDirection;
                    enemy.transform.position = pos;
                }
            }

            // Após 10 movimentos, muda direção e desce
            if (moveCount >= 10)
            {
                moveCount = 0;
                moveDirection *= -1;

                // Move para baixo
                foreach (Enemy enemy in enemies)
                {
                    if (enemy != null)
                    {
                        Vector3 pos = enemy.transform.position;
                        pos.y -= downDistance;
                        enemy.transform.position = pos;
                    }
                }
            }
        }
    }

    void HandleBossSpawn()
    {
        bossTimer += Time.deltaTime;

        if (bossTimer >= nextBossSpawnTime && bossPrefab != null)
        {
            SpawnBoss();
            bossTimer = 0f;
            nextBossSpawnTime = Random.Range(bossSpawnInterval - bossSpawnVariation, 
                                             bossSpawnInterval + bossSpawnVariation);
        }
    }

    void SpawnBoss()
    {
        Instantiate(bossPrefab, bossSpawnPosition, Quaternion.identity);
    }

    public void EnemyDestroyed(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
}
