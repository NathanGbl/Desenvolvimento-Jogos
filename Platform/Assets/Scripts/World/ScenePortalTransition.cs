using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class ScenePortalTransition : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "Pilgrim_Sanctuary";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}
