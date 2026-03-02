using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Tamanho ortográfico da câmera (maior = mais afastado)")]
    public float orthographicSize = 10f;
    
    [Tooltip("Posição da câmera no eixo Z")]
    public float zPosition = -10f;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = orthographicSize;
            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        }
    }
}
