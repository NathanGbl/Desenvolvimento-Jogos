using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceShipGame
{
    public class SpaceShipGameManager : MonoBehaviour
    {
        public static SpaceShipGameManager Instance { get; private set; }

        [SerializeField] private int startLives = 3;
        [SerializeField] private int victoryScore = 400;

        private int lives;
        private int score;
        private bool ended;

        private Text livesText;
        private Text scoreText;
        private Text slowText;

        private float slowEnergy = 1f;
        private const float SlowDrain = 0.35f;
        private const float SlowRecover = 0.22f;

        public bool Ended => ended;
        public float SlowEnergy => slowEnergy;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            lives = startLives;
            EnsureHud();
            UpdateHud();
        }

        private void Update()
        {
            if (ended)
            {
                Time.timeScale = 1f;
                return;
            }

            bool wantsSlow = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (wantsSlow && slowEnergy > 0f)
            {
                Time.timeScale = 0.45f;
                slowEnergy = Mathf.Max(0f, slowEnergy - SlowDrain * Time.unscaledDeltaTime);
            }
            else
            {
                Time.timeScale = 1f;
                slowEnergy = Mathf.Min(1f, slowEnergy + SlowRecover * Time.unscaledDeltaTime);
            }

            UpdateHud();

            if (score >= victoryScore)
            {
                EndGame("VictoryScene");
            }
        }

        public void AddScore(int value)
        {
            if (ended)
            {
                return;
            }

            score += value;
            UpdateHud();
        }

        public void DamagePlayer()
        {
            if (ended)
            {
                return;
            }

            lives--;
            UpdateHud();
            if (lives <= 0)
            {
                EndGame("DefeatScene");
            }
        }

        public void EndGame(string sceneName)
        {
            if (ended)
            {
                return;
            }

            ended = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        private void EnsureHud()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("HUDCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            scoreText = CreateHudText(canvas.transform, "Score", new Vector2(0.03f, 0.95f), TextAnchor.MiddleLeft);
            livesText = CreateHudText(canvas.transform, "Lives", new Vector2(0.97f, 0.95f), TextAnchor.MiddleRight);
            slowText = CreateHudText(canvas.transform, "Slow", new Vector2(0.5f, 0.95f), TextAnchor.MiddleCenter);
        }

        private Text CreateHudText(Transform parent, string name, Vector2 anchor, TextAnchor align)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent);
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(520f, 60f);

            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.color = Color.white;
            text.alignment = align;
            return text;
        }

        private void UpdateHud()
        {
            if (scoreText != null)
            {
                scoreText.text = $"PONTOS: {score}/{victoryScore}";
            }

            if (livesText != null)
            {
                livesText.text = $"VIDAS: {lives}";
            }

            if (slowText != null)
            {
                slowText.text = $"SLOW: {Mathf.RoundToInt(slowEnergy * 100f)}%";
            }
        }
    }
}
