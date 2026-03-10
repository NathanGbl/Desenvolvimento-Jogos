using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShipGame
{
    public class SpaceShipGlobalInitializer : MonoBehaviour
    {
        private static SpaceShipGlobalInitializer instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterGlobalInitializer()
        {
            if (instance != null)
            {
                return;
            }

            GameObject root = new GameObject("__SpaceShipGlobalInitializer");
            DontDestroyOnLoad(root);
            instance = root.AddComponent<SpaceShipGlobalInitializer>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            SpaceShipBootstrap.EnsureBuiltForActiveScene();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SpaceShipBootstrap.EnsureBuiltForActiveScene();
        }
    }
}
