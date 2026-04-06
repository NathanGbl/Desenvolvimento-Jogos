using OCaminhoDoPeregrino.Core;
using UnityEngine;
using UnityEngine.Events;

namespace OCaminhoDoPeregrino.World
{
    public class RelicCollectible : MonoBehaviour
    {
        [SerializeField] private string uniqueId;
        [SerializeField] private string virtueId;
        [SerializeField] private int scoreValue = 1;
        [SerializeField] private bool grantVirtue = true;
        [SerializeField] private GameObject visualRoot;
        [SerializeField] private UnityEvent onCollected;

        public int ScoreValue => scoreValue;

        private bool collected;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (collected || !other.CompareTag("Player"))
            {
                return;
            }

            collected = true;

            if (grantVirtue && VirtueManager.Instance != null)
            {
                VirtueManager.Instance.AcquireVirtue(virtueId);
            }

            if (StageObjectiveManager.Instance != null)
            {
                StageObjectiveManager.Instance.RegisterScore(uniqueId, scoreValue);
            }

            if (visualRoot != null)
            {
                visualRoot.SetActive(false);
            }

            onCollected.Invoke();
            Destroy(gameObject, 0.05f);
        }
    }
}
