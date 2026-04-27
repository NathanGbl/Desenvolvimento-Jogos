using UnityEngine;

namespace OCaminhoDoPeregrino.Core
{
    public static class PlayerTagUtility
    {
        public static bool IsPlayer(GameObject target)
        {
            return target != null && (target.CompareTag("Player") || target.CompareTag("player"));
        }

        public static bool IsPlayer(Component component)
        {
            return component != null && IsPlayer(component.gameObject);
        }
    }
}