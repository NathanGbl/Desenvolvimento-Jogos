using UnityEngine;

public class PilgrimGameplayHUD : MonoBehaviour
{
    [SerializeField] private bool showControls = true;

    private void OnGUI()
    {
        VirtueSystem vs = VirtueSystem.Instance;

        string charity = vs != null && vs.HasCharity ? "SIM" : "NAO";
        string fortitude = vs != null && vs.HasFortitude ? "SIM" : "NAO";
        string doubleJump = vs != null && vs.HasDoubleJump ? "SIM" : "NAO";

        GUIStyle panelStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 16,
            alignment = TextAnchor.UpperLeft,
            wordWrap = true,
        };

        string hudText =
            "MISSAO: peregrinar ate o Santuario e purificar o caminho\n" +
            "Reliquias luminosas = virtudes concedidas por Graca\n" +
            "Muralha escura = obstaculo do pecado (Fortaleza + Dash)\n" +
            "Penitente azul = ato de Caridade (tecla E)\n\n" +
            $"Caridade: {charity} | Fortaleza: {fortitude} | Pulo Duplo: {doubleJump}";

        GUI.Box(new Rect(15f, 15f, 520f, 165f), hudText, panelStyle);

        if (showControls)
        {
            GUI.Box(
                new Rect(15f, 190f, 520f, 72f),
                "Controles: A/D ou Setas | Espaco: Pulo | Shift: Dash | J: Santo Rosario | E: Interagir",
                panelStyle
            );
        }
    }
}
