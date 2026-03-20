using UnityEngine;

public class SecretPassageActivator : MonoBehaviour
{
    [SerializeField] private GameObject targetToActivate;
    [SerializeField] private GameObject targetToDisable;

    public void ActivatePassage()
    {
        if (targetToActivate != null)
        {
            targetToActivate.SetActive(true);
        }

        if (targetToDisable != null)
        {
            targetToDisable.SetActive(false);
        }
    }
}
