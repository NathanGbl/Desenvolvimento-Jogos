using UnityEngine;

public class ControladorIA : MonoBehaviour {
    public Transform disco;
    public float velocidade = 5f;
    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() { // Melhor para física
        if (disco == null) return;

        // Limites laterais (X) e verticais (Y)
        // Substitua os números abaixo pelos que você anotou na sua cena!
        float xAlvo = Mathf.Clamp(disco.position.x, -4.2f, 4.2f);
        float yAlvo = Mathf.Clamp(disco.position.y, 0.5f, 6.94f); // O '0.5' impede que ela desça

        Vector2 posicaoAlvo = new Vector2(xAlvo, yAlvo);
        
        // Usar MovePosition faz com que a IA respeite a física e as paredes
        rb.MovePosition(Vector2.MoveTowards(rb.position, posicaoAlvo, velocidade * Time.fixedDeltaTime));
    }
}