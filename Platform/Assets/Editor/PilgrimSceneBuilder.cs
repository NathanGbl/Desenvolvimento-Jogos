using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public static class PilgrimSceneBuilder
{
    private const string ScenesFolder = "Assets/Scenes";
    private const string MainMenuSceneName = "MainMenu";
    private const string Level1SceneName = "Level_1";
    private const string Level2SceneName = "Level_2";
    private const string EndSceneName = "EndScene";

    [MenuItem("Tools/O Caminho do Peregrino/Gerar Cenas Base")]
    public static void GenerateAllScenes()
    {
        EnsureFolders();

        string mainMenuPath = BuildMainMenuScene();
        string level1Path = BuildLevelScene(Level1SceneName, false, Level2SceneName);
        string level2Path = BuildLevelScene(Level2SceneName, true, string.Empty);
        string endScenePath = BuildEndScene();

        AddScenesToBuildSettings(new[] { mainMenuPath, level1Path, level2Path, endScenePath });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Cenas geradas",
            "As cenas base foram criadas com sucesso. Agora você pode focar em sprites, animações, áudio e level art.",
            "OK"
        );
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(ScenesFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
    }

    private static string BuildMainMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = MainMenuSceneName;

        CreateEventSystem();
        CreateCamera();
        CreateGlobalSystems();

        GameObject canvas = CreateCanvas("MainMenuCanvas");

        TMP_Text title = CreateText(canvas.transform, "TitleText", "O Caminho do Peregrino", 58, new Vector2(0f, 210f), new Vector2(1000f, 100f));
        title.alignment = TextAlignmentOptions.Center;

        TMP_Text instructions = CreateText(canvas.transform, "InstructionsText", "", 28, new Vector2(0f, 10f), new Vector2(1100f, 420f));
        instructions.alignment = TextAlignmentOptions.Top;

        GameObject startButton = CreateButton(canvas.transform, "StartButton", "Iniciar Jornada", new Vector2(0f, -210f), new Vector2(280f, 80f));

        GameObject quitButton = CreateButton(canvas.transform, "QuitButton", "Sair", new Vector2(0f, -310f), new Vector2(280f, 80f));

        MainMenuController menuController = CreateScriptHost<MainMenuController>("MainMenuController");
        menuController.GetType().GetField("firstLevelSceneName", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(menuController, Level1SceneName);

        StartScreenInstructions instructionScript = instructions.gameObject.AddComponent<StartScreenInstructions>();
        instructionScript.GetType().GetField("instructionsText", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(instructionScript, instructions);

        Button startButtonComponent = startButton.GetComponent<Button>();
        startButtonComponent.onClick.AddListener(menuController.StartGame);

        Button quitButtonComponent = quitButton.GetComponent<Button>();
        quitButtonComponent.onClick.AddListener(menuController.QuitGame);

        string path = Path.Combine(ScenesFolder, MainMenuSceneName + ".unity").Replace("\\", "/");
        EditorSceneManager.SaveScene(scene, path);
        return path;
    }

    private static string BuildLevelScene(string sceneName, bool isFinalLevel, string nextSceneName)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = sceneName;

        CreateEventSystem();
        Camera mainCamera = CreateCamera();
        CreateGlobalSystems();

        GameObject worldRoot = new GameObject("World");

        CreateParallaxLayer(worldRoot.transform, "Parallax_Back", new Color(0.05f, 0.05f, 0.1f), -20f, 0.15f);
        CreateParallaxLayer(worldRoot.transform, "Parallax_Mid", new Color(0.1f, 0.1f, 0.18f), -15f, 0.3f);

        GameObject ground = CreateGround(worldRoot.transform, new Vector2(0f, -4f), new Vector2(45f, 1.2f));
        GameObject platform1 = CreateGround(worldRoot.transform, new Vector2(-6f, -1.4f), new Vector2(6f, 0.8f));
        GameObject platform2 = CreateGround(worldRoot.transform, new Vector2(4f, 0.1f), new Vector2(5f, 0.8f));
        GameObject platform3 = CreateGround(worldRoot.transform, new Vector2(13f, 1.3f), new Vector2(6f, 0.8f));

        CreateBarrier(worldRoot.transform, new Vector2(10.5f, -2.8f), new Vector2(1.2f, 2.8f));

        GameObject bridge = CreateGround(worldRoot.transform, new Vector2(-14f, -0.3f), new Vector2(4f, 0.7f));
        bridge.name = "PonteSecreta";
        bridge.SetActive(false);

        GameObject secretWall = CreateGround(worldRoot.transform, new Vector2(-14f, -2.1f), new Vector2(1f, 3f));
        secretWall.name = "ParedeOculta";

        SecretPassageActivator passageActivator = CreateScriptHost<SecretPassageActivator>("SecretPassageActivator");
        SetPrivateField(passageActivator, "targetToActivate", bridge);
        SetPrivateField(passageActivator, "targetToDisable", secretWall);

        GameObject npc = CreateNPC(worldRoot.transform, new Vector2(-12.5f, -2.5f));
        CharityNpc charityNpc = npc.AddComponent<CharityNpc>();
        SetPrivateField(charityNpc, "passageActivator", passageActivator);

        CreateAbilityPickup(worldRoot.transform, new Vector2(-8f, -2.8f), AbilityType.Charity);
        CreateAbilityPickup(worldRoot.transform, new Vector2(1f, 2f), AbilityType.DoubleJump);
        CreateAbilityPickup(worldRoot.transform, new Vector2(16f, 2.4f), AbilityType.Fortitude);

        GameObject collectiblesRoot = new GameObject("Collectibles");
        collectiblesRoot.transform.SetParent(worldRoot.transform);

        CreateCollectible(collectiblesRoot.transform, new Vector2(-3f, -2.2f));
        CreateCollectible(collectiblesRoot.transform, new Vector2(1f, -0.6f));
        CreateCollectible(collectiblesRoot.transform, new Vector2(5.5f, 0.8f));
        CreateCollectible(collectiblesRoot.transform, new Vector2(12.8f, 2.2f));
        CreateCollectible(collectiblesRoot.transform, new Vector2(18f, 2.7f));

        GameObject enemiesRoot = new GameObject("Enemies");
        enemiesRoot.transform.SetParent(worldRoot.transform);
        CreateEnemy(enemiesRoot.transform, new Vector2(-2f, -2.7f), new Vector2(-4f, -2.7f), new Vector2(2f, -2.7f));
        CreateEnemy(enemiesRoot.transform, new Vector2(8f, -2.7f), new Vector2(6f, -2.7f), new Vector2(11f, -2.7f));

        GameObject player = CreatePlayer(worldRoot.transform, new Vector2(-17f, -2.5f));
        AttachCameraFollow(mainCamera, player.transform);

        CreateLevelExit(worldRoot.transform, new Vector2(23f, -2.2f), isFinalLevel, nextSceneName);

        LevelCollectibleRegistrar registrar = CreateScriptHost<LevelCollectibleRegistrar>("LevelCollectibleRegistrar");

        GameObject hudCanvas = CreateHUD(player.GetComponent<PlayerInteractor>());
        hudCanvas.name = "HUDCanvas";

        mainCamera.orthographicSize = 6f;

        string path = Path.Combine(ScenesFolder, sceneName + ".unity").Replace("\\", "/");
        EditorSceneManager.SaveScene(scene, path);
        return path;
    }

    private static string BuildEndScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = EndSceneName;

        CreateEventSystem();
        CreateCamera();
        CreateGlobalSystems();

        GameObject canvas = CreateCanvas("EndCanvas");

        TMP_Text title = CreateText(canvas.transform, "ResultTitle", "Resultado", 64, new Vector2(0f, 180f), new Vector2(900f, 120f));
        title.alignment = TextAlignmentOptions.Center;

        TMP_Text summary = CreateText(canvas.transform, "SummaryText", "", 32, new Vector2(0f, 10f), new Vector2(1000f, 280f));
        summary.alignment = TextAlignmentOptions.Center;

        GameObject restartButton = CreateButton(canvas.transform, "RestartButton", "Reiniciar", new Vector2(0f, -210f), new Vector2(320f, 90f));

        EndSceneController endController = CreateScriptHost<EndSceneController>("EndSceneController");
        SetPrivateField(endController, "titleText", title);
        SetPrivateField(endController, "summaryText", summary);
        SetPrivateField(endController, "mainMenuSceneName", MainMenuSceneName);

        restartButton.GetComponent<Button>().onClick.AddListener(endController.RestartRun);

        string path = Path.Combine(ScenesFolder, EndSceneName + ".unity").Replace("\\", "/");
        EditorSceneManager.SaveScene(scene, path);
        return path;
    }

    private static void AddScenesToBuildSettings(string[] scenePaths)
    {
        List<EditorBuildSettingsScene> result = new List<EditorBuildSettingsScene>();
        for (int i = 0; i < scenePaths.Length; i++)
        {
            result.Add(new EditorBuildSettingsScene(scenePaths[i], true));
        }

        EditorBuildSettings.scenes = result.ToArray();
    }

    private static void CreateGlobalSystems()
    {
        GameObject systems = new GameObject("GlobalSystems");
        systems.AddComponent<GameFlow>();
        systems.AddComponent<PlayerProgress>();
    }

    private static Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera cam = cameraObject.AddComponent<Camera>();
        cam.orthographic = true;
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.03f);
        cameraObject.AddComponent<UniversalAdditionalCameraData>();
        cameraObject.AddComponent<AudioListener>();
        return cam;
    }

    private static void AttachCameraFollow(Camera cam, Transform target)
    {
        CameraFollow2D follow = cam.gameObject.AddComponent<CameraFollow2D>();
        SetPrivateField(follow, "target", target);
    }

    private static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    private static GameObject CreateCanvas(string name)
    {
        GameObject canvasObject = new GameObject(name);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvasObject;
    }

    private static TMP_Text CreateText(Transform parent, string name, string text, float fontSize, Vector2 anchoredPos, Vector2 size)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        TMP_Text tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.16f, 0.16f, 0.24f, 0.95f);

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.16f, 0.16f, 0.24f, 0.95f);
        colors.highlightedColor = new Color(0.25f, 0.25f, 0.35f, 1f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.18f, 1f);
        button.colors = colors;

        TMP_Text labelText = CreateText(buttonObj.transform, "Label", label, 30, Vector2.zero, size);
        labelText.alignment = TextAlignmentOptions.Center;

        return buttonObj;
    }

    private static GameObject CreateGround(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject platform = new GameObject("Plataforma");
        platform.transform.SetParent(parent);
        platform.transform.position = position;

        SpriteRenderer sr = platform.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.2f, 0.2f, 0.25f);

        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = size;

        platform.layer = LayerMask.NameToLayer("Default");
        return platform;
    }

    private static void CreateBarrier(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject barrier = new GameObject("Barreira_Fisica");
        barrier.transform.SetParent(parent);
        barrier.transform.position = position;
        barrier.tag = "Barreira_Fisica";

        SpriteRenderer sr = barrier.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.35f, 0.12f, 0.12f);

        BoxCollider2D collider = barrier.AddComponent<BoxCollider2D>();
        collider.size = size;

        barrier.AddComponent<PhysicalBarrier>();
    }

    private static void CreateParallaxLayer(Transform parent, string name, Color color, float y, float multiplier)
    {
        GameObject layer = new GameObject(name);
        layer.transform.SetParent(parent);
        layer.transform.position = new Vector3(0f, y, 10f);

        SpriteRenderer renderer = layer.AddComponent<SpriteRenderer>();
        renderer.color = color;
        renderer.sortingOrder = -10;

        ParallaxLayer parallax = layer.AddComponent<ParallaxLayer>();
        SetPrivateField(parallax, "parallaxMultiplier", multiplier);
        SetPrivateField(parallax, "lockY", true);
    }

    private static GameObject CreatePlayer(Transform parent, Vector2 position)
    {
        GameObject player = new GameObject("Player");
        player.transform.SetParent(parent);
        player.transform.position = position;
        player.tag = "Player";

        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.85f, 0.85f, 0.92f);

        CapsuleCollider2D capsule = player.AddComponent<CapsuleCollider2D>();
        capsule.size = new Vector2(0.8f, 1.8f);

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        PlayerController2D controller = player.AddComponent<PlayerController2D>();
        PlayerCombat combat = player.AddComponent<PlayerCombat>();
        player.AddComponent<PlayerHealth>();
        player.AddComponent<PlayerInteractor>();
        player.AddComponent<PlayerFaithShield>();

        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(player.transform);
        groundCheck.transform.localPosition = new Vector3(0f, -1f, 0f);
        SetPrivateField(controller, "groundCheck", groundCheck.transform);
        SetPrivateField(controller, "groundLayer", LayerMask.GetMask("Default"));

        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform);
        firePoint.transform.localPosition = new Vector3(0.6f, 0.1f, 0f);
        SetPrivateField(combat, "firePoint", firePoint.transform);

        LightProjectile projectilePrefab = CreateProjectilePrefab();
        SetPrivateField(combat, "projectilePrefab", projectilePrefab);

        return player;
    }

    private static LightProjectile CreateProjectilePrefab()
    {
        const string prefabPath = "Assets/Prefabs/LightProjectile.prefab";

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        LightProjectile existing = AssetDatabase.LoadAssetAtPath<LightProjectile>(prefabPath);
        if (existing != null)
        {
            return existing;
        }

        GameObject go = new GameObject("LightProjectile");
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.color = new Color(1f, 0.95f, 0.5f);

        CircleCollider2D trigger = go.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 0.2f;

        LightProjectile projectile = go.AddComponent<LightProjectile>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);

        return prefab.GetComponent<LightProjectile>();
    }

    private static GameObject CreateEnemy(Transform parent, Vector2 spawnPos, Vector2 pointA, Vector2 pointB)
    {
        GameObject enemy = new GameObject("Enemy");
        enemy.transform.SetParent(parent);
        enemy.transform.position = spawnPos;

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.64f, 0.2f, 0.2f);

        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.9f, 0.9f);

        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        EnemyController controller = enemy.AddComponent<EnemyController>();

        Transform a = new GameObject("PatrolPointA").transform;
        a.SetParent(enemy.transform.parent);
        a.position = pointA;

        Transform b = new GameObject("PatrolPointB").transform;
        b.SetParent(enemy.transform.parent);
        b.position = pointB;

        SetPrivateField(controller, "pointA", a);
        SetPrivateField(controller, "pointB", b);

        return enemy;
    }

    private static GameObject CreateCollectible(Transform parent, Vector2 position)
    {
        GameObject collectible = new GameObject("Collectible");
        collectible.transform.SetParent(parent);
        collectible.transform.position = position;

        SpriteRenderer sr = collectible.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.95f, 0.8f, 0.2f);

        CircleCollider2D trigger = collectible.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 0.35f;

        collectible.AddComponent<CollectibleItem>();
        return collectible;
    }

    private static GameObject CreateNPC(Transform parent, Vector2 position)
    {
        GameObject npc = new GameObject("NPC_Charity");
        npc.transform.SetParent(parent);
        npc.transform.position = position;

        SpriteRenderer sr = npc.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.55f, 0.72f, 0.95f);

        BoxCollider2D trigger = npc.AddComponent<BoxCollider2D>();
        trigger.size = new Vector2(0.9f, 1.6f);
        trigger.isTrigger = true;

        return npc;
    }

    private static void CreateAbilityPickup(Transform parent, Vector2 position, AbilityType abilityType)
    {
        GameObject pickup = new GameObject("Ability_" + abilityType);
        pickup.transform.SetParent(parent);
        pickup.transform.position = position;

        SpriteRenderer sr = pickup.AddComponent<SpriteRenderer>();
        sr.color = abilityType switch
        {
            AbilityType.Charity => new Color(0.3f, 0.8f, 1f),
            AbilityType.DoubleJump => new Color(0.6f, 1f, 0.6f),
            AbilityType.Fortitude => new Color(1f, 0.5f, 0.2f),
            _ => Color.white
        };

        CircleCollider2D trigger = pickup.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;

        AbilityPickup ap = pickup.AddComponent<AbilityPickup>();
        SetPrivateField(ap, "abilityToUnlock", abilityType);
    }

    private static void CreateLevelExit(Transform parent, Vector2 position, bool isFinalLevel, string nextSceneName)
    {
        GameObject exit = new GameObject("LevelExit");
        exit.transform.SetParent(parent);
        exit.transform.position = position;

        SpriteRenderer sr = exit.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.85f, 0.85f, 1f);

        BoxCollider2D trigger = exit.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(1.2f, 2.5f);

        LevelExit levelExit = exit.AddComponent<LevelExit>();
        SetPrivateField(levelExit, "isFinalLevel", isFinalLevel);
        SetPrivateField(levelExit, "nextSceneName", nextSceneName);
        SetPrivateField(levelExit, "requireAllCollectibles", true);
    }

    private static GameObject CreateHUD(PlayerInteractor playerInteractor)
    {
        GameObject canvas = CreateCanvas("HUD");

        TMP_Text scoreText = CreateHudText(canvas.transform, "ScoreText", new Vector2(140f, -40f), TextAlignmentOptions.Left, "Pontos: 0");
        TMP_Text livesText = CreateHudText(canvas.transform, "LivesText", new Vector2(140f, -80f), TextAlignmentOptions.Left, "Vidas: 3");
        TMP_Text itemsText = CreateHudText(canvas.transform, "ItemsText", new Vector2(180f, -120f), TextAlignmentOptions.Left, "Itens: 0/0");
        TMP_Text interactText = CreateHudText(canvas.transform, "InteractText", new Vector2(0f, 40f), TextAlignmentOptions.Center, "");

        HUDController hud = CreateScriptHost<HUDController>("HUDController");
        SetPrivateField(hud, "scoreText", scoreText);
        SetPrivateField(hud, "livesText", livesText);
        SetPrivateField(hud, "collectiblesText", itemsText);
        SetPrivateField(hud, "interactText", interactText);
        SetPrivateField(hud, "playerInteractor", playerInteractor);

        return canvas;
    }

    private static TMP_Text CreateHudText(Transform parent, string name, Vector2 anchoredPos, TextAlignmentOptions alignment, string text)
    {
        TMP_Text tmp = CreateText(parent, name, text, 28, anchoredPos, new Vector2(420f, 50f));
        RectTransform rect = tmp.GetComponent<RectTransform>();
        if (alignment == TextAlignmentOptions.Center)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
        }
        else
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
        }

        tmp.alignment = alignment;
        return tmp;
    }

    private static T CreateScriptHost<T>(string objectName) where T : Component
    {
        GameObject host = new GameObject(objectName);
        return host.AddComponent<T>();
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        if (instance == null)
        {
            return;
        }

        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(instance, value);
    }
}
