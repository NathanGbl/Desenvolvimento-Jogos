using OCaminhoDoPeregrino.Core;
using UnityEngine;
using UnityEngine.Events;

namespace OCaminhoDoPeregrino.World
{
    public class VirtueInteractable : MonoBehaviour
    {
        public enum InteractionMode
        {
            OnTriggerEnterAutomatic,
            OnTriggerStayWithInput
        }

        [Header("Requirement")]
        [SerializeField] private string requiredVirtue;
        [SerializeField] private InteractionMode interactionMode = InteractionMode.OnTriggerStayWithInput;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private bool oneTimeOnly = true;

        [Header("Result")]
        [SerializeField] private Collider2D blockingCollider;
        [SerializeField] private bool disableBlockingCollider = true;
        [SerializeField] private GameObject[] activateOnSuccess;
        [SerializeField] private GameObject[] deactivateOnSuccess;
        [SerializeField] private UnityEvent onSuccess;
        [SerializeField] private UnityEvent onFail;

        private bool hasTriggered;
        private bool playerInRange;

        private void Update()
        {
            if (interactionMode != InteractionMode.OnTriggerStayWithInput)
            {
                return;
            }

            if (!playerInRange)
            {
                return;
            }

            if (Input.GetKeyDown(interactKey))
            {
                TryPracticeVirtue();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            playerInRange = true;

            if (interactionMode == InteractionMode.OnTriggerEnterAutomatic)
            {
                TryPracticeVirtue();
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

        public void TryPracticeVirtue()
        {
            if (oneTimeOnly && hasTriggered)
            {
                return;
            }

            if (VirtueManager.Instance == null)
            {
                onFail.Invoke();
                return;
            }

            if (!VirtueManager.Instance.HasVirtue(requiredVirtue))
            {
                onFail.Invoke();
                return;
            }

            TriggerSuccess();
        }

        private void TriggerSuccess()
        {
            hasTriggered = true;

            if (disableBlockingCollider && blockingCollider != null)
            {
                blockingCollider.enabled = false;
            }

            for (int i = 0; i < activateOnSuccess.Length; i++)
            {
                if (activateOnSuccess[i] != null)
                {
                    activateOnSuccess[i].SetActive(true);
                }
            }

            for (int i = 0; i < deactivateOnSuccess.Length; i++)
            {
                if (deactivateOnSuccess[i] != null)
                {
                    deactivateOnSuccess[i].SetActive(false);
                }
            }

            onSuccess.Invoke();
        }
    }
}
