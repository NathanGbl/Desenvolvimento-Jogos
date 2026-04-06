using OCaminhoDoPeregrino.Core;
using UnityEngine;

namespace OCaminhoDoPeregrino.World
{
    public class SceneGatewayPortal : MonoBehaviour
    {
        [SerializeField] private string requiredVirtue;
        [SerializeField] private string targetSceneName;
        [SerializeField] private bool oneTimeOnly = true;

        private bool used;

        public void SetRequiredVirtue(string virtue) => requiredVirtue = virtue;
        public void SetTargetSceneName(string scene) => targetSceneName = scene;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            TryActivate();
        }

        public void TryActivate()
        {
            if (oneTimeOnly && used)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(requiredVirtue))
            {
                if (VirtueManager.Instance == null || !VirtueManager.Instance.HasVirtue(requiredVirtue))
                {
                    return;
                }
            }

            used = true;

            if (GameFlowManager.Instance != null)
            {
                GameFlowManager.Instance.LoadSceneByName(targetSceneName);
            }
        }
    }
}
