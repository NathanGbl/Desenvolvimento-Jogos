using OCaminhoDoPeregrino.Core;
using UnityEngine;
using UnityEngine.Events;

namespace OCaminhoDoPeregrino.World
{
    public class VirtueAltar : MonoBehaviour
    {
        [SerializeField] private string virtueId;
        [SerializeField] private bool requireInteraction = true;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private bool oneTimeOnly = true;
        [SerializeField] private GameObject altarVisual;
        [SerializeField] private UnityEvent onVirtueGranted;
        [SerializeField] private UnityEvent onAlreadyOwned;

        private bool playerInRange;
        private bool hasGranted;

        public void SetVirtueId(string id) => virtueId = id;
        public void SetRequireInteraction(bool require) => requireInteraction = require;
        public void SetAltarVisual(GameObject visual) => altarVisual = visual;

        private void Update()
        {
            if (!requireInteraction || !playerInRange)
            {
                return;
            }

            if (Input.GetKeyDown(interactKey))
            {
                TryGrantVirtue();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            playerInRange = true;

            if (!requireInteraction)
            {
                TryGrantVirtue();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            playerInRange = false;
        }

        public void TryGrantVirtue()
        {
            if (oneTimeOnly && hasGranted)
            {
                return;
            }

            if (VirtueManager.Instance == null)
            {
                return;
            }

            if (!VirtueManager.Instance.AcquireVirtue(virtueId))
            {
                onAlreadyOwned.Invoke();
                return;
            }

            hasGranted = true;

            if (altarVisual != null)
            {
                altarVisual.SetActive(false);
            }

            onVirtueGranted.Invoke();
        }
    }
}
