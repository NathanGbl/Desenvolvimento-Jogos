using OCaminhoDoPeregrino.Player;
using UnityEngine;

namespace OCaminhoDoPeregrino.World
{
    public class BottomBoundary : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerMovement player = collision.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    // Respeita o checkpoint do jogador em vez de matar
                    player.RespawnAtCheckpoint();
                }
            }
        }
    }
}
