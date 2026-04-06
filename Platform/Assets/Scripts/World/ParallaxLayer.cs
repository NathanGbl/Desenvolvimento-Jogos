using UnityEngine;

namespace OCaminhoDoPeregrino.World
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] private Transform targetCamera;
        [SerializeField] private float parallaxFactor = 0.5f;
        [SerializeField] private bool lockY;

        private Vector3 lastCameraPosition;

        private void Start()
        {
            if (targetCamera == null && Camera.main != null)
            {
                targetCamera = Camera.main.transform;
            }

            if (targetCamera != null)
            {
                lastCameraPosition = targetCamera.position;
            }
        }

        private void LateUpdate()
        {
            if (targetCamera == null)
            {
                return;
            }

            Vector3 cameraDelta = targetCamera.position - lastCameraPosition;
            Vector3 translation = new Vector3(cameraDelta.x * parallaxFactor, lockY ? 0f : cameraDelta.y * parallaxFactor, 0f);
            transform.position += translation;
            lastCameraPosition = targetCamera.position;
        }
    }
}
