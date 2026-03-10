using UnityEngine;

namespace SpaceShipGame
{
    public class Parallax : MonoBehaviour
    {
        private float lenght;
        public float parallaxEffect = 0.5f;

        private void Start()
        {
            lenght = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void Update()
        {
            transform.position += Vector3.left * Time.unscaledDeltaTime * parallaxEffect;
            if (transform.position.x < -lenght)
            {
                transform.position = new Vector3(lenght, transform.position.y, transform.position.z);
            }
        }
    }
}
