using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArkanoidGameManager : MonoBehaviour
{
    private const int InitialLives = 3;
    private const int MaxLevels = 3;
    private const float PlayWidth = 8.4f;
    private const float PlayHeightTop = 4.7f;
    private const float PlayHeightBottom = -5.0f;

    private int currentLives;
    private int currentScore;
    private int currentLevel;
    private int remainingBricks;

    private PaddleController paddle;
    private BallController ball;

    private Text hudText;
    private Text infoText;

    public void InitializeGame()
    {
        currentLives = InitialLives;
        currentScore = 0;
        currentLevel = 1;

        BuildWorldBounds();
        BuildHud();
        SpawnPaddleAndBall();
        StartLevel(currentLevel);
    }

    private void BuildWorldBounds()
    {
        CreateWall("LeftWall", new Vector2(-PlayWidth - 0.3f, 0f), new Vector2(0.6f, 11f));
        CreateWall("RightWall", new Vector2(PlayWidth + 0.3f, 0f), new Vector2(0.6f, 11f));
        CreateWall("TopWall", new Vector2(0f, PlayHeightTop + 0.3f), new Vector2(17.8f, 0.6f));

        var deathZone = new GameObject("DeathZone");
        deathZone.tag = "DeathZone";
        deathZone.transform.position = new Vector2(0f, PlayHeightBottom - 0.4f);

        var trigger = deathZone.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(20f, 1f);

        var deathScript = deathZone.AddComponent<DeathZoneTrigger>();
        deathScript.manager = this;
    }

    private static void CreateWall(string name, Vector2 position, Vector2 size)
    {
        var wall = new GameObject(name);
        wall.tag = "Boundary";
        wall.transform.position = position;

        var collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;
    }

    private void BuildHud()
    {
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var canvasObject = new GameObject("GameCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        hudText = CreateHudText(canvas.transform, "HudText", new Vector2(0f, -30f), 34);
        infoText = CreateHudText(canvas.transform, "InfoText", new Vector2(0f, -78f), 28);

        UpdateHud();
    }

    private static Text CreateHudText(Transform parent, string objectName, Vector2 anchoredPosition, int fontSize)
    {
        var textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        var text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.UpperCenter;
        text.color = Color.white;

        var rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(1400f, 60f);

        return text;
    }

    private void SpawnPaddleAndBall()
    {
        var paddleObject = CreateRectObject("Paddle", new Color(0.25f, 0.65f, 1f, 1f), new Vector2(2.2f, 0.35f));
        paddleObject.tag = "Paddle";
        paddleObject.transform.position = new Vector2(0f, -4.2f);

        var paddleBody = paddleObject.AddComponent<Rigidbody2D>();
        paddleBody.bodyType = RigidbodyType2D.Kinematic;

        var paddleController = paddleObject.AddComponent<PaddleController>();
        paddleController.moveSpeed = 11f;
        paddleController.minX = -PlayWidth + 1.2f;
        paddleController.maxX = PlayWidth - 1.2f;
        paddle = paddleController;

        var ballObject = CreateCircleObject("Ball", new Color(1f, 0.96f, 0.5f, 1f), 0.28f);
        ballObject.tag = "Ball";

        var ballCollider = ballObject.GetComponent<CircleCollider2D>();
        ballCollider.sharedMaterial = CreateBouncyMaterial();

        var ballBody = ballObject.AddComponent<Rigidbody2D>();
        ballBody.gravityScale = 0f;
        ballBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        ball = ballObject.AddComponent<BallController>();
        ball.manager = this;
        ball.paddle = paddle;

        ResetBallOnPaddle();
    }

    public void StartLevel(int level)
    {
        DestroyExistingBricksAndPowerUps();

        SpawnBricks(level);
        ball.SetSpeed(6f + level * 1.02f);
        ResetBallOnPaddle();

        infoText.text = $"Nível {level}: Pressione ESPAÇO para lançar a bola";
        UpdateHud();
    }

    private void DestroyExistingBricksAndPowerUps()
    {
        foreach (var brick in FindObjectsByType<Brick>(FindObjectsSortMode.None))
        {
            Destroy(brick.gameObject);
        }

        foreach (var powerUp in FindObjectsByType<PowerUp>(FindObjectsSortMode.None))
        {
            Destroy(powerUp.gameObject);
        }
    }

    private void SpawnBricks(int level)
    {
        int rows = 3 + level;
        int cols = 10;
        remainingBricks = rows * cols;

        float startX = -7.0f;
        float startY = 3.6f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var brickObject = CreateRectObject($"Brick_{row}_{col}", ColorForRow(row), new Vector2(1.3f, 0.45f));
                brickObject.tag = "Brick";
                brickObject.transform.position = new Vector2(startX + col * 1.55f, startY - row * 0.62f);

                var brick = brickObject.AddComponent<Brick>();
                brick.manager = this;
                brick.points = 100 + row * 20;
            }
        }
    }

    private static Color ColorForRow(int row)
    {
        Color[] palette =
        {
            new Color(1f, 0.35f, 0.35f, 1f),
            new Color(1f, 0.62f, 0.2f, 1f),
            new Color(1f, 0.88f, 0.3f, 1f),
            new Color(0.45f, 0.9f, 0.45f, 1f),
            new Color(0.4f, 0.7f, 1f, 1f),
            new Color(0.8f, 0.55f, 1f, 1f)
        };

        return palette[row % palette.Length];
    }

    public void NotifyBrickDestroyed(Vector3 position, int points)
    {
        currentScore += points;
        remainingBricks--;
        UpdateHud();

        TrySpawnPowerUp(position);

        if (remainingBricks > 0)
        {
            return;
        }

        currentLevel++;
        if (currentLevel > MaxLevels)
        {
            SceneManager.LoadScene(ArkanoidSceneBootstrap.VictorySceneName);
            return;
        }

        StartLevel(currentLevel);
    }

    private void TrySpawnPowerUp(Vector3 position)
    {
        if (Random.value > 0.35f)
        {
            return;
        }

        var powerUpObject = CreateRectObject("PowerUp", new Color(0.95f, 0.95f, 1f, 1f), new Vector2(0.75f, 0.35f));
        powerUpObject.tag = "PowerUp";
        powerUpObject.transform.position = position;

        var collider = powerUpObject.GetComponent<BoxCollider2D>();
        collider.isTrigger = true;

        var body = powerUpObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.linearVelocity = Vector2.down * 2.6f;

        var powerUp = powerUpObject.AddComponent<PowerUp>();
        powerUp.manager = this;
        powerUp.type = (PowerUpType)Random.Range(0, 3);

        var sprite = powerUpObject.GetComponent<SpriteRenderer>();
        sprite.color = powerUp.GetColor();
    }

    public void BallLost()
    {
        currentLives--;
        UpdateHud();

        if (currentLives <= 0)
        {
            SceneManager.LoadScene(ArkanoidSceneBootstrap.DefeatSceneName);
            return;
        }

        infoText.text = "Você perdeu uma vida! Pressione ESPAÇO para continuar";
        ResetBallOnPaddle();
    }

    public void ResetBallOnPaddle()
    {
        ball.AttachToPaddle();
    }

    public void OnBallLaunched()
    {
        infoText.text = string.Empty;
    }

    public void ApplyPowerUp(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.ExpandPaddle:
                StartCoroutine(ApplyPaddleExpansion());
                infoText.text = "Power-up: Nave expandida";
                break;
            case PowerUpType.SpeedBall:
                ball.BoostSpeed(1.25f, 6f);
                infoText.text = "Power-up: Bola acelerada";
                break;
            case PowerUpType.ExtraLife:
                currentLives++;
                infoText.text = "Power-up: Vida extra";
                UpdateHud();
                break;
        }
    }

    private IEnumerator ApplyPaddleExpansion()
    {
        paddle.SetWidthMultiplier(1.6f);
        yield return new WaitForSeconds(8f);
        if (paddle != null)
        {
            paddle.SetWidthMultiplier(1f);
        }
    }

    private void UpdateHud()
    {
        if (hudText != null)
        {
            hudText.text = $"Pontuação: {currentScore}  |  Vidas: {currentLives}  |  Nível: {currentLevel}/{MaxLevels}";
        }
    }

    private static PhysicsMaterial2D CreateBouncyMaterial()
    {
        var material = new PhysicsMaterial2D("BallBouncy")
        {
            friction = 0f,
            bounciness = 1f
        };

        return material;
    }

    private static GameObject CreateRectObject(string name, Color color, Vector2 size)
    {
        var gameObject = new GameObject(name);

        var sprite = gameObject.AddComponent<SpriteRenderer>();
        sprite.sprite = RuntimeSprites.WhiteSquare;
        sprite.color = color;

        gameObject.transform.localScale = new Vector3(size.x, size.y, 1f);
        gameObject.AddComponent<BoxCollider2D>();

        return gameObject;
    }

    private static GameObject CreateCircleObject(string name, Color color, float diameter)
    {
        var gameObject = new GameObject(name);

        var sprite = gameObject.AddComponent<SpriteRenderer>();
        sprite.sprite = RuntimeSprites.WhiteCircle;
        sprite.color = color;

        gameObject.transform.localScale = new Vector3(diameter, diameter, 1f);
        gameObject.AddComponent<CircleCollider2D>();

        return gameObject;
    }
}

public static class RuntimeSprites
{
    private static Sprite whiteSquare;
    private static Sprite whiteCircle;

    public static Sprite WhiteSquare
    {
        get
        {
            if (whiteSquare == null)
            {
                whiteSquare = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            }

            return whiteSquare;
        }
    }

    public static Sprite WhiteCircle
    {
        get
        {
            if (whiteCircle == null)
            {
                whiteCircle = CreateCircleSprite(64);
            }

            return whiteCircle;
        }
    }

    private static Sprite CreateCircleSprite(int size)
    {
        var texture = new Texture2D(size, size, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Vector2 center = new Vector2((size - 1) / 2f, (size - 1) / 2f);
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
