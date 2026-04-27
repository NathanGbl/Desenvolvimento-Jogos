using OCaminhoDoPeregrino.Player;
using UnityEngine;
using OCaminhoDoPeregrino.Core;

namespace OCaminhoDoPeregrino.World
{
    public class BottomBoundary : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (PlayerTagUtility.IsPlayer(collision.gameObject))
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
