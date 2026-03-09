using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI instructionsText;
    private TextMeshProUGUI startText;

    void Start()
    {
        CreateMenuUI();
    }

    void CreateMenuUI()
    {
        // Criar Canvas
        GameObject canvasObj = new GameObject("MenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Título
        titleText = CreateText("TitleText", canvas.transform, new Vector2(0, 150), 72);
        titleText.text = "SPACE INVADERS";
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.green;

        // Instruções
        instructionsText = CreateText("InstructionsText", canvas.transform, new Vector2(0, 0), 32);
        instructionsText.text = "Use as SETAS para mover\nESPAÇO para atirar\n\nDestrua todos os invasores!";
        instructionsText.alignment = TextAlignmentOptions.Center;
        instructionsText.color = Color.white;

        // Botão Start
        startText = CreateText("StartText", canvas.transform, new Vector2(0, -150), 48);
        startText.text = "Pressione ESPAÇO para começar";
        startText.alignment = TextAlignmentOptions.Center;
        startText.color = Color.yellow;

        // Piscar texto de start
        InvokeRepeating("BlinkStartText", 0f, 0.5f);
    }

    void BlinkStartText()
    {
        if (startText != null)
        {
            startText.enabled = !startText.enabled;
        }
    }

    TextMeshProUGUI CreateText(string name, Transform parent, Vector2 position, float fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800, 200);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.text = string.Empty;
        
        return text;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
