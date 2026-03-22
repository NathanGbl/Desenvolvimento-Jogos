using UnityEngine;

public class ObstacleGate : MonoBehaviour
{
    [Header("Configuração do Gate")]
    [SerializeField] private RequiredVirtue requiredVirtue = RequiredVirtue.Charity;
    [SerializeField] private bool requiresDashForFortitude = true;
    [SerializeField] private bool destroySelfOnUnlock = true;
    [SerializeField] private bool disableColliderOnUnlock;

    [Header("Ação ao Abrir")]
    [SerializeField] private GameObject targetToActivate;
    [SerializeField] private GameObject targetToDisable;

    [Header("Prompt")]
    [SerializeField] private bool requiresInteractionKey = true;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private bool showPrompt = true;

    private Collider2D gateCollider;
    private bool unlocked;
    private bool playerInRange;

    private void Awake()
    {
        gateCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || unlocked)
        {
            return;
        }

        playerInRange = true;

        if (requiresInteractionKey && !Input.GetKeyDown(interactionKey))
        {
            return;
        }

        TryUnlock(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || unlocked)
        {
            return;
        }

        if (requiredVirtue == RequiredVirtue.Fortitude && requiresDashForFortitude)
        {
            PlayerMovement movement = collision.gameObject.GetComponent<PlayerMovement>();
            if (movement == null || !movement.IsDashing)
            {
                return;
            }
        }

        TryUnlock(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || unlocked)
        {
            return;
        }

        playerInRange = true;

        if (requiredVirtue == RequiredVirtue.Fortitude && requiresDashForFortitude)
        {
            PlayerMovement movement = collision.gameObject.GetComponent<PlayerMovement>();
            if (movement == null || !movement.IsDashing)
            {
                return;
            }
        }

        TryUnlock(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void OnGUI()
    {
        if (!showPrompt || unlocked || !playerInRange)
        {
            return;
        }

        string virtueName = requiredVirtue == RequiredVirtue.Charity ? "Caridade" : "Fortaleza";
        string actionText = requiresInteractionKey ? $"Pressione {interactionKey}" : "Aja com dash";

        GUIStyle style = new GUIStyle(GUI.skin.box)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
        };

        Rect rect = new Rect((Screen.width * 0.5f) - 260f, Screen.height - 120f, 520f, 70f);
        GUI.Box(rect, $"Prova de {virtueName}: {actionText}", style);
    }

    private void TryUnlock(GameObject actor)
    {
        if (VirtueSystem.Instance == null || !VirtueSystem.Instance.HasVirtue(requiredVirtue))
        {
            return;
        }

        unlocked = true;

        if (targetToActivate != null)
        {
            targetToActivate.SetActive(true);
        }

        if (targetToDisable != null)
        {
            targetToDisable.SetActive(false);
        }

        if (disableColliderOnUnlock && gateCollider != null)
        {
            gateCollider.enabled = false;
        }

        if (destroySelfOnUnlock)
        {
            Destroy(gameObject);
        }
    }
}
