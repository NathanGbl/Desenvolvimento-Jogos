using UnityEngine;

public class Paddle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float minX = -7f;
    public float maxX = 7f;
    
    [Header("Size Settings")]
    public float normalWidth = 1.5f;
    public float expandedWidth = 2.5f;
    private float currentWidth;
    private BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        currentWidth = normalWidth;
        UpdateColliderSize();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            input = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            input = 1f;

        if (input != 0)
        {
            Vector3 newPos = transform.position + Vector3.right * input * moveSpeed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            transform.position = newPos;
        }
    }

    public void ExpandPaddle(float duration = 5f)
    {
        currentWidth = expandedWidth;
        UpdateColliderSize();
        
        // Retorna ao tamanho normal após a duração
        Invoke("ResetPaddleSize", duration);
    }

    public void ResetPaddleSize()
    {
        currentWidth = normalWidth;
        UpdateColliderSize();
    }

    void UpdateColliderSize()
    {
        if (boxCollider != null)
        {
            Vector2 newSize = boxCollider.size;
            newSize.x = currentWidth;
            boxCollider.size = newSize;
        }
    }

    public float GetWidth() => currentWidth;
}
