using OCaminhoDoPeregrino.Player;
using UnityEngine;

namespace OCaminhoDoPeregrino.World
{
    public class SinHazard : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private bool forceCheckpointRespawn = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryApplyPenalty(other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryApplyPenalty(collision.gameObject);
        }

        private void TryApplyPenalty(GameObject target)
        {
            if (!target.CompareTag("Player"))
            {
                return;
            }

            PlayerMovement player = target.GetComponent<PlayerMovement>();
            if (player == null)
            {
                return;
            }

            player.ApplySinPenalty(damage, forceCheckpointRespawn);
        }
    }
}
