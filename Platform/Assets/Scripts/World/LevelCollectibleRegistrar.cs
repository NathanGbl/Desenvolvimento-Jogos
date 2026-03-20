using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCollectibleRegistrar : MonoBehaviour
{
    private void Start()
    {
        CollectibleItem[] collectibles = FindObjectsOfType<CollectibleItem>(true);
        int required = 0;

        for (int i = 0; i < collectibles.Length; i++)
        {
            if (collectibles[i].RequiredForVictory)
            {
                required++;
            }
        }

        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.RegisterSceneCollectibles(SceneManager.GetActiveScene().name, required);
        }
    }
}
