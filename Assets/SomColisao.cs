using UnityEngine;

public class SomColisao : MonoBehaviour {
    void OnCollisionEnter2D(Collision2D colisao) {
        AudioSource fonteAudio = GetComponent<AudioSource>();
        if (fonteAudio != null) {
            fonteAudio.Play();
        }
    }
}