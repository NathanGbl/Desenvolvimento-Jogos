using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArkanoidSceneBootstrap : MonoBehaviour
{
    public const string PresentationSceneName = "PresentationScene";
    public const string GameSceneName = "GameScene";
    public const string VictorySceneName = "VictoryScene";
    public const string DefeatSceneName = "DefeatScene";

    private static bool isBootstrapping;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BootstrapActiveScene()
    {
        if (isBootstrapping)
        {
            return;
        }

        isBootstrapping = true;

        var existing = FindFirstObjectByType<ArkanoidSceneBootstrap>();
        if (existing == null)
        {
            var bootstrapObject = new GameObject(nameof(ArkanoidSceneBootstrap));
            bootstrapObject.AddComponent<ArkanoidSceneBootstrap>();
        }

        isBootstrapping = false;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        BuildScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BuildScene(scene.name);
    }

    private void BuildScene(string sceneName)
    {
        if (sceneName == PresentationSceneName || sceneName == "SampleScene")
        {
            BuildPresentationScene();
            return;
        }

        if (sceneName == GameSceneName)
        {
            BuildGameScene();
            return;
        }

        if (sceneName == VictorySceneName)
        {
            BuildResultScene("VOCÊ VENCEU!", Color.green);
            return;
        }

        if (sceneName == DefeatSceneName)
        {
            BuildResultScene("GAME OVER", Color.red);
        }
    }

    private void BuildPresentationScene()
    {
        if (FindFirstObjectByType<SimpleMenuController>() != null)
        {
            return;
        }

        EnsureMainCamera();
        var canvas = CreateCanvas("PresentationCanvas");

        CreateText(canvas.transform, "ARKANOID", 84, new Vector2(0f, 140f), Color.white);
        CreateText(canvas.transform, "Use A/D ou Setas para mover a nave", 34, new Vector2(0f, 10f), Color.white);
        CreateText(canvas.transform, "Pressione ESPAÇO para iniciar", 40, new Vector2(0f, -110f), Color.yellow);

        var menuController = new GameObject("PresentationMenuController").AddComponent<SimpleMenuController>();
        menuController.targetScene = GameSceneName;
    }

    private void BuildResultScene(string title, Color color)
    {
        if (FindFirstObjectByType<SimpleMenuController>() != null)
        {
            return;
        }

        EnsureMainCamera();
        var canvas = CreateCanvas("ResultCanvas");

        CreateText(canvas.transform, title, 86, new Vector2(0f, 80f), color);
        CreateText(canvas.transform, "Pressione ESPAÇO para voltar ao menu", 36, new Vector2(0f, -70f), Color.white);

        var menuController = new GameObject("ResultMenuController").AddComponent<SimpleMenuController>();
        menuController.targetScene = PresentationSceneName;
    }

    private void BuildGameScene()
    {
        if (FindFirstObjectByType<ArkanoidGameManager>() != null)
        {
            return;
        }

        EnsureMainCamera();

        var gameManager = new GameObject("ArkanoidGameManager").AddComponent<ArkanoidGameManager>();
        gameManager.InitializeGame();
    }

    private static Canvas CreateCanvas(string name)
    {
        var existingCanvas = FindFirstObjectByType<Canvas>();
        if (existingCanvas != null)
        {
            return existingCanvas;
        }

        var canvasObject = new GameObject(name);
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    private static Text CreateText(Transform parent, string content, int fontSize, Vector2 anchoredPosition, Color color)
    {
        var textObject = new GameObject(content);
        textObject.transform.SetParent(parent, false);

        var text = textObject.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;

        var rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(1400f, 220f);

        return text;
    }

    private static void EnsureMainCamera()
    {
        var camera = Camera.main;
        if (camera == null)
        {
            var cameraObject = new GameObject("Main Camera");
            camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
        }

        camera.backgroundColor = new Color(0.07f, 0.08f, 0.15f, 1f);
        camera.clearFlags = CameraClearFlags.SolidColor;

        if (camera.GetComponent<AudioListener>() == null)
        {
            camera.gameObject.AddComponent<AudioListener>();
        }
    }
}

public class SimpleMenuController : MonoBehaviour
{
    public string targetScene;

    private void Update()
    {
        bool pressed = Keyboard.current != null &&
                       (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);

        if (!pressed)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
