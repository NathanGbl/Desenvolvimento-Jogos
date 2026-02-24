using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LogicaGol : MonoBehaviour {
    public GameObject textoFimDeJogo;
    private bool jogoAcabou = false;

    void Start() {
        if(textoFimDeJogo != null) textoFimDeJogo.SetActive(false);
        Time.timeScale = 1f; 
    }

    void Update() {
        if (jogoAcabou && Input.GetKeyDown(KeyCode.R)) {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnTriggerEnter2D(Collider2D outro) {
        if (outro.CompareTag("Disco")) {
            Debug.Log("GOL CONFIRMADO!");
            FinalizarJogo();
        }
    }

    void FinalizarJogo() {
        jogoAcabou = true;
        
        if(textoFimDeJogo != null) {
            textoFimDeJogo.SetActive(true);
            var textoTMP = textoFimDeJogo.GetComponent<TextMeshProUGUI>();
            if(textoTMP != null) textoTMP.text = "FIM DE JOGO!\nAperte 'R' para Reiniciar";
        }

        Time.timeScale = 0f;
    }
}