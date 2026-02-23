using UnityEngine;

public class ControleDisco : MonoBehaviour {
    public float velocidadeMaxima = 3f;
    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        
        if (rb.linearVelocity.magnitude > velocidadeMaxima) {
            rb.linearVelocity = rb.linearVelocity.normalized * velocidadeMaxima;
        }

        
        if (Mathf.Abs(transform.position.y) > 10 || Mathf.Abs(transform.position.x) > 10) {
            ResetarDisco();
        }
    }

    public void ResetarDisco() {
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector2.zero;
    }
}