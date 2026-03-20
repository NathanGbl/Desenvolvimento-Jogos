using UnityEngine;

public class PhysicalBarrier : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (PlayerProgress.Instance == null || !PlayerProgress.Instance.HasAbility(AbilityType.Fortitude))
        {
            return;
        }

        PlayerController2D controller = collision.gameObject.GetComponent<PlayerController2D>();
        if (controller == null)
        {
            return;
        }

        if (controller.IsDashing())
        {
            Destroy(gameObject);
        }
    }
}
