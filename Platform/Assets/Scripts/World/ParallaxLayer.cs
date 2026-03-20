using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float parallaxMultiplier = 0.3f;
    [SerializeField] private bool lockY = true;

    private Vector3 lastTargetPosition;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 delta = target.position - lastTargetPosition;
        Vector3 movement = new Vector3(delta.x * parallaxMultiplier, lockY ? 0f : delta.y * parallaxMultiplier, 0f);
        transform.position += movement;
        lastTargetPosition = target.position;
    }
}
