using TMPro;
using UnityEngine;

public class StartScreenInstructions : MonoBehaviour
{
    [SerializeField] private TMP_Text instructionsText;

    private void Start()
    {
        if (instructionsText == null)
        {
            return;
        }

        instructionsText.text =
            "O Caminho do Peregrino\n\n" +
            "A/D ou Setas: mover\n" +
            "Espaço: pular\n" +
            "Shift: dash\n" +
            "J: disparo de luz\n" +
            "E: interagir\n\n" +
            "Colete todos os itens sagrados para vencer.\n" +
            "Evite os inimigos: ao perder todas as vidas, é derrota.";
    }
}
