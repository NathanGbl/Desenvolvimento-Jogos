using UnityEngine;

public class ControladorJogador : MonoBehaviour {
    Rigidbody2D rb;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() { 
        Vector2 posicaoMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float xClamped = Mathf.Clamp(posicaoMouse.x, -4.2f, 4.2f);
        float yClamped = Mathf.Clamp(posicaoMouse.y, -6.94f, -0.1f);

        rb.MovePosition(new Vector2(xClamped, yClamped));
    }
}