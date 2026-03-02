using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    public float moveSpeed = 11f;
    public float minX = -7f;
    public float maxX = 7f;

    private Vector3 defaultScale;

    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    private void Update()
    {
        float direction = ReadHorizontalInput();
        if (Mathf.Abs(direction) < 0.01f)
        {
            return;
        }

        float nextX = transform.position.x + direction * moveSpeed * Time.deltaTime;
        nextX = Mathf.Clamp(nextX, minX, maxX);

        transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
    }

    public void SetWidthMultiplier(float multiplier)
    {
        transform.localScale = new Vector3(defaultScale.x * multiplier, defaultScale.y, defaultScale.z);
    }

    private static float ReadHorizontalInput()
    {
        if (Keyboard.current == null)
        {
            return Input.GetAxisRaw("Horizontal");
        }

        float value = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            value -= 1f;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            value += 1f;
        }

        return value;
    }
}
