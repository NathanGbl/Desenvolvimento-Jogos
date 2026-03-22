using UnityEngine;
using UnityEngine.SceneManagement;

public class PilgrimStartSceneController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Pilgrim_Chapter01";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}
