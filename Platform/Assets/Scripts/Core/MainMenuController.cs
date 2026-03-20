using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Level_1";

    public void StartGame()
    {
        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.StartNewRun();
        }

        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
