using OCaminhoDoPeregrino.Player;
using UnityEngine;
using OCaminhoDoPeregrino.Core;

namespace OCaminhoDoPeregrino.World
{
    public class PlayerCheckpoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTagUtility.IsPlayer(other))
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
