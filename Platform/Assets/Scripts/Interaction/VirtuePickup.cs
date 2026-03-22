using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VirtuePickup : MonoBehaviour
{
    [SerializeField] private RequiredVirtue virtueToUnlock = RequiredVirtue.Charity;
    [SerializeField] private bool unlockDoubleJump;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || VirtueSystem.Instance == null)
        {
            return;
        }

        if (virtueToUnlock == RequiredVirtue.Charity)
        {
            VirtueSystem.Instance.UnlockCharity();
        }
        else if (virtueToUnlock == RequiredVirtue.Fortitude)
        {
            VirtueSystem.Instance.UnlockFortitude();
        }

        if (unlockDoubleJump)
        {
            VirtueSystem.Instance.UnlockDoubleJump();
        }

        Destroy(gameObject);
    }
}
