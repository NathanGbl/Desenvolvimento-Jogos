using System.Collections.Generic;
using OCaminhoDoPeregrino.Player;
using OCaminhoDoPeregrino.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OCaminhoDoPeregrino.Core
{
    public class DebugSceneBootstrapper : MonoBehaviour
    {
        private static DebugSceneBootstrapper instance;
        private GameObject runtimeRoot;
        private string lastComposedScene = string.Empty;
        private bool hasInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if (instance != null)
                return;

            GameObject bootstrapObject = new GameObject("DebugSceneBootstrapper");
            instance = bootstrapObject.AddComponent<DebugSceneBootstrapper>();
            DontDestroyOnLoad(bootstrapObject);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            if (hasInitialized)
                return;

            hasInitialized = true;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += HandleSceneLoaded;
            ComposeCurrentScene();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                SceneManager.sceneLoaded -= HandleSceneLoaded;
                instance = null;
            }
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) => ComposeCurrentScene();

        private void ComposeCurrentScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == lastComposedScene)
                return;

            Compose(activeScene);
        }

        private void Compose(Scene scene)
        {
            if (!IsSupportedScene(scene.name))
                return;

            lastComposedScene = scene.name;
            Debug.Log($"[DebugSceneBootstrapper] Composing scene: {scene.name}");
            EnsureManagers();
            EnsureCamera();
            EnsureRuntimeRoot();

            switch (scene.name)
            {
                case "Level_01":
                case "SampleScene":
                    BuildLevelOne();
                    break;
                case "Level_02":
                    BuildLevelTwo();
                    break;
                case "Victory":
                    BuildVictoryScene();
                    break;
                case "Defeat":
                    BuildDefeatScene();
                    break;
            }
        }

        private static bool IsSupportedScene(string sceneName)
        {
            return sceneName == "Level_01" || sceneName == "Level_02" || sceneName == "Victory" || sceneName == "Defeat" || sceneName == "SampleScene";
        }

        private void EnsureRuntimeRoot()
        {
            if (runtimeRoot == null)
            {
                GameObject root = GameObject.Find("__DebugRuntimeRoot");
                if (root == null)
                {
                    runtimeRoot = new GameObject("__DebugRuntimeRoot");
                    DontDestroyOnLoad(runtimeRoot);
                }
                else
                {
                    runtimeRoot = root;
                }
            }
            else
            {
                int childCount = runtimeRoot.transform.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                    Destroy(runtimeRoot.transform.GetChild(i).gameObject);
            }
        }

        private void EnsureManagers()
        {
            if (FindObjectOfType<VirtueManager>() == null)
                new GameObject("VirtueManager").AddComponent<VirtueManager>();

            if (FindObjectOfType<GameFlowManager>() == null)
                new GameObject("GameFlowManager").AddComponent<GameFlowManager>();

            if (FindObjectOfType<StageObjectiveManager>() == null)
                new GameObject("StageObjectiveManager").AddComponent<StageObjectiveManager>();
        }

        private void EnsureCamera()
        {
            CleanupExtraAudioListeners();

            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
                camera.orthographic = true;
                camera.orthographicSize = 6f;
                camera.transform.position = new Vector3(0f, 0f, -10f);
            }
            else if (camera.GetComponent<AudioListener>() == null)
            {
                camera.gameObject.AddComponent<AudioListener>();
            }

            CameraFollow2D follow = camera.GetComponent<CameraFollow2D>();
            if (follow == null)
                follow = camera.gameObject.AddComponent<CameraFollow2D>();

            follow.SetTarget(GetOrCreatePlayer().transform);
            camera.backgroundColor = new Color(0.05f, 0.06f, 0.09f, 1f);
        }

        private void CleanupExtraAudioListeners()
        {
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            for (int i = listeners.Length - 1; i >= 1; i--)
                Destroy(listeners[i]);
        }

        private GameObject GetOrCreatePlayer()
        {
            PlayerMovement existingPlayer = FindObjectOfType<PlayerMovement>();
            if (existingPlayer != null)
                return existingPlayer.gameObject;

            GameObject player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = new Vector3(-6f, 0f, 0f);
            player.layer = LayerMask.NameToLayer("Default");

            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3.2f;
            rb.freezeRotation = true;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = DebugGeometryFactory.CreateSquare(new Color(0.86f, 0.80f, 0.62f, 1f));
            renderer.sortingOrder = 10;
            player.transform.localScale = new Vector3(0.8f, 1.5f, 1f);

            Transform groundCheck = new GameObject("GroundCheck").transform;
            groundCheck.SetParent(player.transform);
            groundCheck.localPosition = new Vector3(0f, -0.8f, 0f);

            PlayerMovement movement = player.AddComponent<PlayerMovement>();
            ConfigurePlayerMovement(movement, groundCheck);

            if (runtimeRoot != null)
                player.transform.SetParent(runtimeRoot.transform, true);

            return player;
        }

        private static void ConfigurePlayerMovement(PlayerMovement movement, Transform groundCheck)
        {
            var groundField = typeof(PlayerMovement).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var layoutField = typeof(PlayerMovement).GetField("groundLayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (groundField != null)
                groundField.SetValue(movement, groundCheck);
            
            if (layoutField != null)
            {
                int groundLayerIndex = LayerMask.NameToLayer("Ground");
                int groundLayerMaskValue = (groundLayerIndex >= 0) ? (1 << groundLayerIndex) : 0;
                LayerMask groundLayerMask = new LayerMask();
                groundLayerMask.value = groundLayerMaskValue;
                layoutField.SetValue(movement, groundLayerMask);
            }
        }

        private void BuildLevelOne()
        {
            Debug.Log("[DebugSceneBootstrapper] Building Level One");
            CreateBackdrop(new Color(0.07f, 0.07f, 0.10f, 1f));
            CreateGround(new Vector3(0f, -2.8f, 0f), new Vector2(24f, 1.6f), new Color(0.2f, 0.19f, 0.18f, 1f));
            CreateGround(new Vector3(4f, -0.6f, 0f), new Vector2(4f, 0.5f), new Color(0.25f, 0.24f, 0.22f, 1f));
            CreateGround(new Vector3(10f, 0.7f, 0f), new Vector2(3f, 0.45f), new Color(0.25f, 0.24f, 0.22f, 1f));
            CreateGround(new Vector3(16f, -0.2f, 0f), new Vector2(4f, 0.45f), new Color(0.25f, 0.24f, 0.22f, 1f));

            CreateCheckpoint(new Vector3(-2.2f, -1.7f, 0f));
            CreateVirtueAltar(new Vector3(1.2f, -1.6f, 0f), "caridade", false, true, new Color(0.95f, 0.8f, 0.2f, 1f));
            CreateSinHazard(new Vector3(6.5f, -2.2f, 0f), new Vector2(1.2f, 1.2f));
            CreateSinHazard(new Vector3(8.3f, -2.2f, 0f), new Vector2(1.2f, 1.2f));
            CreatePortal(new Vector3(19.2f, -1.2f, 0f), new Vector2(1.6f, 3.2f), "caridade", "Level_02", new Color(0.2f, 0.9f, 1f, 1f));
            CreateBottomBoundary(new Vector3(0f, -5f, 0f));
            Debug.Log("[DebugSceneBootstrapper] Level One built successfully");
        }

        private void BuildLevelTwo()
        {
            CreateBackdrop(new Color(0.05f, 0.05f, 0.08f, 1f));
            CreateGround(new Vector3(0f, -2.8f, 0f), new Vector2(26f, 1.6f), new Color(0.18f, 0.17f, 0.16f, 1f));
            CreateGround(new Vector3(3.5f, -0.5f, 0f), new Vector2(3.5f, 0.45f), new Color(0.24f, 0.23f, 0.21f, 1f));
            CreateGround(new Vector3(8f, 1.0f, 0f), new Vector2(3f, 0.45f), new Color(0.24f, 0.23f, 0.21f, 1f));
            CreateGround(new Vector3(13f, -0.2f, 0f), new Vector2(3.2f, 0.45f), new Color(0.24f, 0.23f, 0.21f, 1f));
            CreateGround(new Vector3(18f, 0.9f, 0f), new Vector2(3f, 0.45f), new Color(0.24f, 0.23f, 0.21f, 1f));

            CreateCheckpoint(new Vector3(-2.2f, -1.7f, 0f));
            CreateVirtueAltar(new Vector3(2.1f, -1.6f, 0f), "paciência", false, true, new Color(0.95f, 0.8f, 0.2f, 1f));
            CreateSinHazard(new Vector3(5.5f, -2.2f, 0f), new Vector2(1.2f, 1.2f));
            CreateSinHazard(new Vector3(7.4f, -2.2f, 0f), new Vector2(1.2f, 1.2f));
            CreateSinHazard(new Vector3(11.7f, -2.2f, 0f), new Vector2(1.2f, 1.2f));
            CreatePortal(new Vector3(21.6f, -1.2f, 0f), new Vector2(1.6f, 3.2f), "paciência", "Victory", new Color(0.15f, 1f, 0.55f, 1f));
            CreateBottomBoundary(new Vector3(0f, -5f, 0f));
        }

        private void BuildVictoryScene()
        {
            CreateBackdrop(new Color(0.04f, 0.08f, 0.05f, 1f));
            CreateGround(new Vector3(0f, -2.8f, 0f), new Vector2(18f, 1.6f), new Color(0.16f, 0.18f, 0.15f, 1f));
            CreateCircleShape(new Vector3(0f, 1.0f, 0f), new Vector3(2.6f, 2.6f, 1f), new Color(0.75f, 0.9f, 0.4f, 1f));
            CreatePortal(new Vector3(0f, -1.0f, 0f), new Vector2(2f, 3f), string.Empty, "Level_01", new Color(0.9f, 0.95f, 0.5f, 1f));
            CreateBottomBoundary(new Vector3(0f, -5f, 0f));
        }

        private void BuildDefeatScene()
        {
            CreateBackdrop(new Color(0.10f, 0.04f, 0.04f, 1f));
            CreateGround(new Vector3(0f, -2.8f, 0f), new Vector2(18f, 1.6f), new Color(0.18f, 0.14f, 0.14f, 1f));
            CreateDiamondShape(new Vector3(0f, 0.8f, 0f), new Vector3(3f, 3f, 1f), new Color(0.9f, 0.25f, 0.25f, 1f));
            CreatePortal(new Vector3(0f, -1.0f, 0f), new Vector2(2f, 3f), string.Empty, "Level_01", new Color(0.7f, 0.85f, 1f, 1f));
            CreateBottomBoundary(new Vector3(0f, -5f, 0f));
        }

        private void CreateBackdrop(Color color)
        {
            GameObject backdrop = CreateVisualObject("Backdrop", new Vector3(0f, 0f, 8f), new Vector3(40f, 24f, 1f), DebugGeometryFactory.CreateSquare(color), -100, false);
            backdrop.transform.SetParent(runtimeRoot.transform, true);
        }

        private void CreateGround(Vector3 position, Vector2 size, Color color)
        {
            GameObject ground = CreateVisualObject("Ground", position, new Vector3(size.x, size.y, 1f), DebugGeometryFactory.CreateSquare(color), 0, false);
            ground.layer = LayerMask.NameToLayer("Ground");
            
            BoxCollider2D collider = ground.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            
            Rigidbody2D rb = ground.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            
            ground.transform.SetParent(runtimeRoot.transform, true);
        }

        private void CreateSinHazard(Vector3 position, Vector2 size)
        {
            GameObject hazard = CreateVisualObject("SinHazard", position, new Vector3(size.x, size.y, 1f), DebugGeometryFactory.CreateDiamond(new Color(0.9f, 0.1f, 0.15f, 1f)), 5, false);
            hazard.layer = LayerMask.NameToLayer("Hazard");
            BoxCollider2D collider = hazard.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            hazard.AddComponent<SinHazard>();
            hazard.transform.SetParent(runtimeRoot.transform, true);
        }

        private void CreateCheckpoint(Vector3 position)
        {
            GameObject checkpoint = CreateVisualObject("Checkpoint", position, new Vector3(0.6f, 1.4f, 1f), DebugGeometryFactory.CreateDiamond(new Color(0.2f, 0.75f, 1f, 1f)), 4, false);
            checkpoint.layer = LayerMask.NameToLayer("Checkpoint");
            BoxCollider2D collider = checkpoint.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            checkpoint.AddComponent<PlayerCheckpoint>();
            checkpoint.transform.SetParent(runtimeRoot.transform, true);
        }

        private void CreateVirtueAltar(Vector3 position, string virtueId, bool requireInteraction, bool hideAfterGrant, Color color)
        {
            GameObject altar = CreateVisualObject("VirtueAltar", position, new Vector3(1.2f, 1.8f, 1f), DebugGeometryFactory.CreateCircle(color), 4, false);
            altar.layer = LayerMask.NameToLayer("Interactable");
            BoxCollider2D collider = altar.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            
            VirtueAltar virtueAltar = altar.AddComponent<VirtueAltar>();
            virtueAltar.SetVirtueId(virtueId);
            virtueAltar.SetRequireInteraction(requireInteraction);
            if (hideAfterGrant)
            {
                virtueAltar.SetAltarVisual(altar);
            }
            
            altar.transform.SetParent(runtimeRoot.transform, true);
        }

        private void CreatePortal(Vector3 position, Vector2 size, string requiredVirtue, string targetSceneName, Color color)
        {
            GameObject portal = CreateVisualObject("GatewayPortal", position, new Vector3(size.x, size.y, 1f), DebugGeometryFactory.CreateRing(color), 6, false);
            portal.layer = LayerMask.NameToLayer("Interactable");
            BoxCollider2D collider = portal.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            
            SceneGatewayPortal gatewayPortal = portal.AddComponent<SceneGatewayPortal>();
            gatewayPortal.SetRequiredVirtue(requiredVirtue);
            gatewayPortal.SetTargetSceneName(targetSceneName);
            
            portal.transform.SetParent(runtimeRoot.transform, true);
        }

        private void CreateCircleShape(Vector3 position, Vector3 scale, Color color)
        {
            GameObject shape = CreateVisualObject("CircleShape", position, scale, DebugGeometryFactory.CreateCircle(color), 2, false);
            shape.transform.SetParent(runtimeRoot.transform, true);
        }

        private void CreateDiamondShape(Vector3 position, Vector3 scale, Color color)
        {
            GameObject shape = CreateVisualObject("DiamondShape", position, scale, DebugGeometryFactory.CreateDiamond(color), 2, false);
            shape.transform.SetParent(runtimeRoot.transform, true);
        }

        private void CreateBottomBoundary(Vector3 position)
        {
            GameObject boundary = new GameObject("BottomBoundary");
            boundary.transform.position = position;
            boundary.layer = LayerMask.NameToLayer("Default");

            BoxCollider2D collider = boundary.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(40f, 2f);
            collider.isTrigger = true;

            boundary.AddComponent<BottomBoundary>();
            boundary.transform.SetParent(runtimeRoot.transform, true);
        }

        private GameObject CreateVisualObject(string objectName, Vector3 position, Vector3 scale, Sprite sprite, int sortingOrder, bool addRigidbody)
        {
            GameObject visualObject = new GameObject(objectName);
            visualObject.transform.position = position;
            visualObject.transform.localScale = scale;

            SpriteRenderer renderer = visualObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;

            if (addRigidbody)
            {
                visualObject.AddComponent<Rigidbody2D>();
            }

            return visualObject;
        }
    }
}
