using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DefeatScene : MonoBehaviour
{
    void Start()
    {
        CreateDefeatUI();
    }

    void CreateDefeatUI()
    {
        // Criar Canvas
        GameObject canvasObj = new GameObject("DefeatCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Título de Derrota
        TextMeshProUGUI titleText = CreateText("TitleText", canvas.transform, new Vector2(0, 150), 72);
        titleText.text = "GAME OVER";
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.red;

        // Mensagem
        TextMeshProUGUI messageText = CreateText("MessageText", canvas.transform, new Vector2(0, 50), 40);
        messageText.text = "Os invasores venceram...";
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.white;

        // Pontuação
        if (GameManager.Instance != null)
        {
            TextMeshProUGUI scoreText = CreateText("ScoreText", canvas.transform, new Vector2(0, -20), 36);
            scoreText.text = "Pontuação: " + GameManager.Instance.score;
            scoreText.alignment = TextAlignmentOptions.Center;
            scoreText.color = Color.yellow;
        }

        // Instruções
        TextMeshProUGUI instructionsText = CreateText("InstructionsText", canvas.transform, new Vector2(0, -120), 28);
        instructionsText.text = "Pressione R para TENTAR NOVAMENTE\nPressione ESC para SAIR";
        instructionsText.alignment = TextAlignmentOptions.Center;
        instructionsText.color = Color.white;
    }

    TextMeshProUGUI CreateText(string name, Transform parent, Vector2 position, float fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800, 100);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        return text;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
