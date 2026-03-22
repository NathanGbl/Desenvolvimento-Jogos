using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PilgrimRequestedSceneBuilder
{
    private const string ScenesFolder = "Assets/Scenes";
    private const string StartScene = "Pilgrim_Start";
    private const string GameplayScene = "Pilgrim_Chapter01";
    private const string SanctuaryScene = "Pilgrim_Sanctuary";

    [MenuItem("Tools/O Caminho do Peregrino/Gerar Cenas da Proposta")]
    public static void GenerateRequestedScenes()
    {
        EnsureScenesFolder();

        string startPath = BuildStartScene();
        string gameplayPath = BuildGameplayScene();
        string sanctuaryPath = BuildSanctuaryScene();

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(startPath, true),
            new EditorBuildSettingsScene(gameplayPath, true),
            new EditorBuildSettingsScene(sanctuaryPath, true),
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Cenas criadas",
            "As cenas Pilgrim_Start, Pilgrim_Chapter01 e Pilgrim_Sanctuary foram geradas e adicionadas no Build Settings.",
            "OK"
        );
    }

    private static void EnsureScenesFolder()
    {
        if (!AssetDatabase.IsValidFolder(ScenesFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
    }

    private static string BuildStartScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = StartScene;

        CreateCamera(new Color(0.02f, 0.02f, 0.06f));
        CreateEventSystem();

        GameObject canvas = CreateCanvas("StartCanvas");
        CreateText(canvas.transform, "Title", "O CAMINHO DO PEREGRINO", 60, new Vector2(0f, 170f), new Vector2(1200f, 120f), TextAlignmentOptions.Center, new Color(0.93f, 0.88f, 0.66f));
        CreateText(canvas.transform, "Lore", "Atravesse as terras corrompidas pelo pecado e peregrine rumo ao Santuario do Senhor.", 30, new Vector2(0f, 40f), new Vector2(1200f, 120f), TextAlignmentOptions.Center, Color.white);
        CreateText(canvas.transform, "Controls", "Setas/A-D: mover | Espaco: pular | Shift: dash | J: Santo Rosario", 24, new Vector2(0f, -50f), new Vector2(1200f, 80f), TextAlignmentOptions.Center, Color.white);
        CreateText(canvas.transform, "Start", "Pressione ESPACO para iniciar a peregrinacao", 34, new Vector2(0f, -170f), new Vector2(900f, 90f), TextAlignmentOptions.Center, new Color(1f, 0.95f, 0.4f));

        GameObject controller = new GameObject("StartSceneController");
        PilgrimStartSceneController startController = controller.AddComponent<PilgrimStartSceneController>();
        SetPrivateString(startController, "gameplaySceneName", GameplayScene);

        string path = Path.Combine(ScenesFolder, StartScene + ".unity").Replace("\\", "/");
        EditorSceneManager.SaveScene(scene, path);
        return path;
    }

    private static string BuildGameplayScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = GameplayScene;

        CreateCamera(new Color(0.04f, 0.04f, 0.09f));

        GameObject systems = new GameObject("Systems");
        systems.AddComponent<VirtueSystem>();

        GameObject world = new GameObject("World");

        CreateGround(world.transform, "Ground", new Vector2(0f, -4.3f), new Vector2(30f, 1.2f));
        CreateGround(world.transform, "Platform_A", new Vector2(-6f, -1.7f), new Vector2(6f, 0.8f));
        CreateGround(world.transform, "Platform_B", new Vector2(2.5f, -0.4f), new Vector2(5f, 0.8f));

        GameObject player = CreatePlayer(world.transform, new Vector2(-11f, -2.8f));

        CreateEnemy(world.transform, new Vector2(7f, -2.7f));
        CreateEnemy(world.transform, new Vector2(12f, -2.7f));

        CreateVirtuePickup(world.transform, "CharityPickup", new Vector2(-8.5f, -2.9f), RequiredVirtue.Charity, false, new Color(0.4f, 0.9f, 1f));
        CreateVirtuePickup(world.transform, "FortitudePickup", new Vector2(4.6f, 0.6f), RequiredVirtue.Fortitude, false, new Color(1f, 0.6f, 0.2f));
        CreateVirtuePickup(world.transform, "DoubleJumpPickup", new Vector2(0f, 2.1f), RequiredVirtue.Charity, true, new Color(0.8f, 0.9f, 0.3f));

        CreateNpcGate(world.transform, new Vector2(-2f, -2.6f));
        CreateBarrierGate(world.transform, new Vector2(9.8f, -2.8f), new Vector2(1f, 2.5f));

        CreateSanctuaryPortal(world.transform, new Vector2(14.8f, -2.5f));

        string path = Path.Combine(ScenesFolder, GameplayScene + ".unity").Replace("\\", "/");
        EditorSceneManager.SaveScene(scene, path);
        return path;
    }

    private static string BuildSanctuaryScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = SanctuaryScene;

        CreateCamera(new Color(0.12f, 0.1f, 0.03f));
        CreateEventSystem();

        GameObject canvas = CreateCanvas("SanctuaryCanvas");
        CreateText(canvas.transform, "Title", "SANTUARIO ALCANCADO - DEO GRATIAS", 62, new Vector2(0f, 130f), new Vector2(1200f, 120f), TextAlignmentOptions.Center, new Color(1f, 0.92f, 0.68f));
        CreateText(canvas.transform, "Body", "A jornada continua, mas a sua fe venceu esta travessia.", 30, new Vector2(0f, 0f), new Vector2(1200f, 120f), TextAlignmentOptions.Center, Color.white);
        CreateText(canvas.transform, "Restart", "Pressione R para reiniciar | ESC para sair", 24, new Vector2(0f, -140f), new Vector2(900f, 90f), TextAlignmentOptions.Center, Color.white);

        GameObject controller = new GameObject("SanctuaryController");
        PilgrimSanctuaryController sanctuaryController = controller.AddComponent<PilgrimSanctuaryController>();
        SetPrivateString(sanctuaryController, "startSceneName", StartScene);

        string path = Path.Combine(ScenesFolder, SanctuaryScene + ".unity").Replace("\\", "/");
        EditorSceneManager.SaveScene(scene, path);
        return path;
    }

    private static Camera CreateCamera(Color background)
    {
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";

        Camera cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.backgroundColor = background;

        camObj.AddComponent<AudioListener>();
        return cam;
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
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        return canvasObj;
    }

    private static TMP_Text CreateText(Transform parent, string name, string text, float size, Vector2 anchoredPos, Vector2 boxSize, TextAlignmentOptions alignment, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = boxSize;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = alignment;
        return tmp;
    }

    private static void CreateGround(Transform parent, string name, Vector2 pos, Vector2 scale)
    {
        GameObject ground = new GameObject(name);
        ground.name = name;
        ground.transform.SetParent(parent);
        ground.transform.position = new Vector3(pos.x, pos.y, 0f);
        ground.transform.localScale = new Vector3(scale.x, scale.y, 1f);

        BoxCollider2D collider = ground.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;

        Rigidbody2D rb = ground.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.15f, 0.15f, 0.2f);

        ground.layer = LayerMask.NameToLayer("Default");
    }

    private static GameObject CreatePlayer(Transform parent, Vector2 spawn)
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.SetParent(parent);
        player.transform.position = new Vector3(spawn.x, spawn.y, 0f);

        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.86f, 0.9f, 1f);

        BoxCollider2D bodyCollider = player.AddComponent<BoxCollider2D>();
        bodyCollider.size = new Vector2(0.8f, 1.4f);

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        PlayerMovement movement = player.AddComponent<PlayerMovement>();
        RosaryCombat combat = player.AddComponent<RosaryCombat>();

        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(player.transform);
        groundCheck.transform.localPosition = new Vector3(0f, -0.75f, 0f);

        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform);
        firePoint.transform.localPosition = new Vector3(0.55f, 0.15f, 0f);

        SetPrivateTransform(movement, "groundCheck", groundCheck.transform);
        SetPrivateLayerMask(movement, "groundMask", LayerMask.GetMask("Default"));
        SetPrivateTransform(combat, "firePoint", firePoint.transform);

        RosaryProjectile projectilePrefab = CreateRosaryProjectilePrefabLikeObject();
        SetPrivateProjectile(combat, "projectilePrefab", projectilePrefab);

        return player;
    }

    private static RosaryProjectile CreateRosaryProjectilePrefabLikeObject()
    {
        GameObject prefabObject = new GameObject("RuntimeRosaryProjectile");
        prefabObject.SetActive(false);

        SpriteRenderer sr = prefabObject.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 0.95f, 0.5f);

        CircleCollider2D col = prefabObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        RosaryProjectile projectile = prefabObject.AddComponent<RosaryProjectile>();
        return projectile;
    }

    private static void CreateEnemy(Transform parent, Vector2 pos)
    {
        GameObject enemy = new GameObject("SinEnemy");
        enemy.tag = "Enemy";
        enemy.transform.SetParent(parent);
        enemy.transform.position = new Vector3(pos.x, pos.y, 0f);

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.9f, 0.3f, 0.3f);

        BoxCollider2D col = enemy.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private static void CreateVirtuePickup(Transform parent, string name, Vector2 pos, RequiredVirtue virtue, bool unlockDoubleJump, Color color)
    {
        GameObject pickup = new GameObject(name);
        pickup.transform.SetParent(parent);
        pickup.transform.position = new Vector3(pos.x, pos.y, 0f);

        SpriteRenderer sr = pickup.AddComponent<SpriteRenderer>();
        sr.color = color;

        CircleCollider2D col = pickup.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        VirtuePickup virtuePickup = pickup.AddComponent<VirtuePickup>();
        SetPrivateEnum(virtuePickup, "virtueToUnlock", virtue);
        SetPrivateBool(virtuePickup, "unlockDoubleJump", unlockDoubleJump);
    }

    private static void CreateNpcGate(Transform parent, Vector2 pos)
    {
        GameObject npc = new GameObject("Npc_Purificacao");
        npc.transform.SetParent(parent);
        npc.transform.position = new Vector3(pos.x, pos.y, 0f);

        SpriteRenderer sr = npc.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.65f, 0.82f, 1f);

        CircleCollider2D col = npc.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        ObstacleGate gate = npc.AddComponent<ObstacleGate>();
        SetPrivateEnum(gate, "requiredVirtue", RequiredVirtue.Charity);
        SetPrivateBool(gate, "requiresInteractionKey", true);
        SetPrivateBool(gate, "destroySelfOnUnlock", true);
    }

    private static void CreateBarrierGate(Transform parent, Vector2 pos, Vector2 size)
    {
        GameObject barrier = new GameObject("Barreira_Fisica");
        barrier.tag = "Barreira_Fisica";
        barrier.transform.SetParent(parent);
        barrier.transform.position = new Vector3(pos.x, pos.y, 0f);

        SpriteRenderer sr = barrier.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.38f, 0.22f, 0.17f);

        BoxCollider2D col = barrier.AddComponent<BoxCollider2D>();
        col.size = size;
        col.isTrigger = false;

        Rigidbody2D rb = barrier.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        ObstacleGate gate = barrier.AddComponent<ObstacleGate>();
        SetPrivateEnum(gate, "requiredVirtue", RequiredVirtue.Fortitude);
        SetPrivateBool(gate, "requiresDashForFortitude", true);
        SetPrivateBool(gate, "destroySelfOnUnlock", true);
        SetPrivateBool(gate, "requiresInteractionKey", false);
    }

    private static void CreateSanctuaryPortal(Transform parent, Vector2 pos)
    {
        GameObject portal = new GameObject("SantuarioPortal");
        portal.transform.SetParent(parent);
        portal.transform.position = new Vector3(pos.x, pos.y, 0f);

        SpriteRenderer sr = portal.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 0.93f, 0.6f);

        BoxCollider2D col = portal.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        ScenePortalTransition transition = portal.AddComponent<ScenePortalTransition>();
        SetPrivateString(transition, "targetSceneName", SanctuaryScene);
    }

    private static void SetPrivateString(object target, string field, string value)
    {
        var info = target.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (info != null)
        {
            info.SetValue(target, value);
        }
    }

    private static void SetPrivateTransform(object target, string field, Transform value)
    {
        var info = target.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (info != null)
        {
            info.SetValue(target, value);
        }
    }

    private static void SetPrivateLayerMask(object target, string field, LayerMask value)
    {
        var info = target.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (info != null)
        {
            info.SetValue(target, value);
        }
    }

    private static void SetPrivateProjectile(object target, string field, RosaryProjectile value)
    {
        var info = target.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (info != null)
        {
            info.SetValue(target, value);
        }
    }

    private static void SetPrivateEnum(object target, string field, RequiredVirtue value)
    {
        var info = target.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (info != null)
        {
            info.SetValue(target, value);
        }
    }

    private static void SetPrivateBool(object target, string field, bool value)
    {
        var info = target.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (info != null)
        {
            info.SetValue(target, value);
        }
    }
}
