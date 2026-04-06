using UnityEngine;
using UnityEngine.SceneManagement;

namespace OCaminhoDoPeregrino.Core
{
    public class GameFlowManager : MonoBehaviour
    {
        public static GameFlowManager Instance { get; private set; }

        [SerializeField] private string firstLevelSceneName = "Level_01";
        [SerializeField] private string victorySceneName = "Victory";
        [SerializeField] private string defeatSceneName = "Defeat";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadFirstLevel()
        {
            SceneManager.LoadScene(firstLevelSceneName);
        }

        public void LoadSceneByName(string sceneName)
        {
            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void LoadVictory()
        {
            SceneManager.LoadScene(victorySceneName);
        }

        public void LoadDefeat()
        {
            SceneManager.LoadScene(defeatSceneName);
        }

        public void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
