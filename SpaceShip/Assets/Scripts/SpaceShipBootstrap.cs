using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceShipGame
{
    public enum SpaceShipSceneKind
    {
        Auto,
        Presentation,
        Game,
        Victory,
        Defeat
    }

    public static class SpaceShipBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BuildCurrentScene()
        {
            EnsureBuiltForActiveScene();
        }

        public static void EnsureBuiltForActiveScene()
        {
            EnsureBuilt(ResolveSceneKind(SceneManager.GetActiveScene()));
        }

        public static void EnsureBuilt(SpaceShipSceneKind kind)
        {
            if (kind == SpaceShipSceneKind.Auto)
            {
                return;
            }

            if (GameObject.Find("__SpaceShipGenerated") != null)
            {
                return;
            }

            EnsureCamera();

            switch (kind)
            {
                case SpaceShipSceneKind.Presentation:
                    BuildMenuScene("SPACESHIP", "Setas: mover | Espaco: atirar\nShift: desacelerar tempo\nEnter: iniciar", "GameScene");
                    break;
                case SpaceShipSceneKind.Game:
                    BuildGameScene();
                    break;
                case SpaceShipSceneKind.Victory:
                    BuildMenuScene("VITORIA", "Voce protegeu a galaxia!\nEnter: voltar", "PresentationScene");
                    break;
                case SpaceShipSceneKind.Defeat:
                    BuildMenuScene("DERROTA", "Sua nave foi destruida...\nEnter: tentar novamente", "PresentationScene");
                    break;
            }
        }

        public static SpaceShipSceneKind ResolveSceneKind(Scene scene)
        {
            string sceneName = scene.name.ToLowerInvariant();
            string scenePath = scene.path.ToLowerInvariant();

            if (sceneName.Contains("presentation") || scenePath.Contains("presentationscene"))
            {
                return SpaceShipSceneKind.Presentation;
            }

            if (sceneName.Contains("game") || scenePath.Contains("gamescene"))
            {
                return SpaceShipSceneKind.Game;
            }

            if (sceneName.Contains("victory") || scenePath.Contains("victoryscene"))
            {
                return SpaceShipSceneKind.Victory;
            }

            if (sceneName.Contains("defeat") || scenePath.Contains("defeatscene"))
            {
                return SpaceShipSceneKind.Defeat;
            }

            if (scene.buildIndex == 0)
            {
                return SpaceShipSceneKind.Presentation;
            }

            if (scene.buildIndex == 1)
            {
                return SpaceShipSceneKind.Game;
            }

            if (scene.buildIndex == 2)
            {
                return SpaceShipSceneKind.Victory;
            }

            if (scene.buildIndex == 3)
            {
                return SpaceShipSceneKind.Defeat;
            }

            return SpaceShipSceneKind.Auto;
        }

        private static void EnsureCamera()
        {
            Camera cam = Object.FindFirstObjectByType<Camera>();
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                cam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }

            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.backgroundColor = new Color(0.03f, 0.06f, 0.12f, 1f);
        }

        // Carrega uma Texture2D da pasta Resources e cria um Sprite a partir dela.
        // Retorna null se o arquivo não existir (fallback para sprite gerado).
        private static Sprite LoadSprite(string resourceName)
        {
            Texture2D tex = Resources.Load<Texture2D>(resourceName);
            if (tex == null)
            {
                return null;
            }

            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private static void BuildMenuScene(string title, string subtitle, string targetScene)
        {
            GameObject root = new GameObject("__SpaceShipGenerated");
            Canvas canvas = CreateCanvas(root.transform);

            CreateText(canvas.transform, title, new Vector2(0.5f, 0.68f), 72, TextAnchor.MiddleCenter, new Color(0.85f, 0.95f, 1f));
            CreateText(canvas.transform, subtitle, new Vector2(0.5f, 0.42f), 30, TextAnchor.MiddleCenter, Color.white);

            SceneChangeOnInput sceneChangeOnInput = root.AddComponent<SceneChangeOnInput>();
            sceneChangeOnInput.targetScene = targetScene;
        }

        private static void BuildGameScene()
        {
            GameObject root = new GameObject("__SpaceShipGenerated");

            GameObject managerObj = new GameObject("SpaceShipGameManager");
            managerObj.transform.SetParent(root.transform);
            managerObj.AddComponent<SpaceShipGameManager>();

            GameObject bgRoot = new GameObject("BackgroundRoot");
            bgRoot.transform.SetParent(root.transform);
            BuildParallaxBackground(bgRoot.transform);

            GameObject player = new GameObject("PlayerShip");
            player.transform.SetParent(root.transform);
            player.transform.position = new Vector3(-7.2f, 0f, 0f);
            player.tag = "Player";

            SpriteRenderer playerRenderer = player.AddComponent<SpriteRenderer>();
            playerRenderer.sortingOrder = 5;

            Sprite ship01 = LoadSprite("Ship01");
            if (ship01 != null)
            {
                playerRenderer.sprite = ship01;
                playerRenderer.color = Color.white;
                float s = 1.5f / Mathf.Max(ship01.bounds.size.x, ship01.bounds.size.y);
                player.transform.localScale = Vector3.one * s;
            }
            else
            {
                playerRenderer.sprite = SpaceShipSpriteFactory.GetSquareSprite();
                playerRenderer.color = new Color(0.2f, 0.9f, 1f);
                player.transform.localScale = new Vector3(0.8f, 0.55f, 1f);
            }

            player.transform.rotation = Quaternion.identity;

            BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();
            playerCollider.isTrigger = true;

            Rigidbody2D playerRb = player.AddComponent<Rigidbody2D>();
            playerRb.bodyType = RigidbodyType2D.Kinematic;

            player.AddComponent<PlayerShipController>();

            GameObject spawner = new GameObject("EnemySpawner");
            spawner.transform.SetParent(root.transform);
            spawner.AddComponent<EnemySpawner>();
        }

        private static void BuildParallaxBackground(Transform parent)
        {
            Sprite stars    = LoadSprite("Stars");
            Sprite farback1 = LoadSprite("Farback01");
            Sprite farback2 = LoadSprite("Farback02");

            // Stars – plano mais distante, mais devagar
            CreateParallaxLayer(parent, "StarsA",     0f,  stars,    new Color(0.05f, 0.1f,  0.2f),  0.15f, -50);
            CreateParallaxLayer(parent, "StarsB",    20f,  stars,    new Color(0.05f, 0.1f,  0.2f),  0.15f, -50);
            // Farback01 – plano intermediário
            CreateParallaxLayer(parent, "FarbackA1",  0f,  farback1, new Color(0.12f, 0.18f, 0.3f),  0.40f, -40);
            CreateParallaxLayer(parent, "FarbackB1", 20f,  farback1, new Color(0.12f, 0.18f, 0.3f),  0.40f, -40);
            // Farback02 – plano mais próximo, mais rápido
            CreateParallaxLayer(parent, "FarbackA2",  0f,  farback2, new Color(0.20f, 0.24f, 0.35f), 0.80f, -30);
            CreateParallaxLayer(parent, "FarbackB2", 20f,  farback2, new Color(0.20f, 0.24f, 0.35f), 0.80f, -30);
        }

        // Cria uma camada de parallax usando o sprite real (se disponível) ou fallback.
        // Duas instâncias (A e B) são criadas 20 unidades afastadas – o script Parallax
        // detecta a largura real pelo bounds e faz o wrap automático.
        private static void CreateParallaxLayer(Transform parent, string name, float x, Sprite sprite, Color fallback, float effect, int sortingOrder)
        {
            GameObject layer = new GameObject(name);
            layer.transform.SetParent(parent);
            layer.transform.position = new Vector3(x, 0f, 0f);

            SpriteRenderer renderer = layer.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = sortingOrder;

            if (sprite != null)
            {
                renderer.sprite = sprite;
                // Escala para cobrir exatamente 20 unidades de largura e 11 de altura
                // (cobre qualquer janela 16:9 com orthographicSize = 5)
                float sx = 20f / renderer.bounds.size.x;
                float sy = 11f / renderer.bounds.size.y;
                layer.transform.localScale = new Vector3(sx, sy, 1f);
            }
            else
            {
                renderer.sprite = SpaceShipSpriteFactory.GetSquareSprite();
                renderer.color = fallback;
                layer.transform.localScale = new Vector3(10.24f, 10f, 1f);
            }

            Parallax parallax = layer.AddComponent<Parallax>();
            parallax.parallaxEffect = effect;
        }

        private static Canvas CreateCanvas(Transform parent)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.transform.SetParent(parent);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static Text CreateText(Transform parent, string content, Vector2 anchor, int fontSize, TextAnchor alignment, Color color)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(parent);
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(1400f, 300f);

            Text uiText = textObj.AddComponent<Text>();
            uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            uiText.text = content;
            uiText.fontSize = fontSize;
            uiText.alignment = alignment;
            uiText.color = color;
            return uiText;
        }
    }

    public class SceneChangeOnInput : MonoBehaviour
    {
        public string targetScene;

        private void Update()
        {
            if (SpaceShipInput.SubmitPressedThisFrame())
            {
                SceneManager.LoadScene(targetScene);
            }
        }
    }

    public static class SpaceShipSpriteFactory
    {
        private static Sprite squareSprite;

        public static Sprite GetSquareSprite()
        {
            if (squareSprite != null)
            {
                return squareSprite;
            }

            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            squareSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            squareSprite.name = "GeneratedSquare";
            return squareSprite;
        }
    }
}
