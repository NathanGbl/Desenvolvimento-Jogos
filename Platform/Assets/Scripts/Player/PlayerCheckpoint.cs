using OCaminhoDoPeregrino.Player;
using UnityEngine;

namespace OCaminhoDoPeregrino.World
{
    public class PlayerCheckpoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player == null)
            {
                return;
            }

            player.SetCheckpoint(transform.position);
        }
    }
}
