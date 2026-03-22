using UnityEngine;
using UnityEngine.SceneManagement;

public class PilgrimSanctuaryController : MonoBehaviour
{
    [SerializeField] private string startSceneName = "Pilgrim_Start";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (VirtueSystem.Instance != null)
            {
                VirtueSystem.Instance.ResetVirtues();
            }

            SceneManager.LoadScene(startSceneName);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
