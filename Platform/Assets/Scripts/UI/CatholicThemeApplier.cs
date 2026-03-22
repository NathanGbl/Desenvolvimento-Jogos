using TMPro;
using UnityEngine;

public class CatholicThemeApplier : MonoBehaviour
{
    [SerializeField] private bool applyOnStart = true;

    private void Start()
    {
        if (applyOnStart)
        {
            ApplyTheme();
        }
    }

    public void ApplyTheme()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.backgroundColor = new Color(0.03f, 0.03f, 0.08f);
        }

        ApplyVisualToObject("Player", new Color(0.88f, 0.9f, 1f), "Peregrino", new Vector3(0f, 1.2f, 0f));
        ApplyVisualToObject("CharityPickup", new Color(0.43f, 0.78f, 1f), "Reliquia de Caridade", new Vector3(0f, 0.9f, 0f));
        ApplyVisualToObject("FortitudePickup", new Color(1f, 0.62f, 0.24f), "Reliquia de Fortaleza", new Vector3(0f, 0.9f, 0f));
        ApplyVisualToObject("DoubleJumpPickup", new Color(0.92f, 0.88f, 0.35f), "Graca do Pulo Duplo", new Vector3(0f, 0.9f, 0f));
        ApplyVisualToObject("Npc_Purificacao", new Color(0.6f, 0.8f, 1f), "Penitente a Purificar", new Vector3(0f, 1.1f, 0f));
        ApplyVisualToObject("Barreira_Fisica", new Color(0.35f, 0.2f, 0.16f), "Muralha do Pecado", new Vector3(0f, 1.5f, 0f));
        ApplyVisualToObject("SantuarioPortal", new Color(1f, 0.9f, 0.62f), "Entrada do Santuario", new Vector3(0f, 1.2f, 0f));

        RenameEnemies();
    }

    private void RenameEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            AddWorldLabel(enemies[i], "Pecado Manifesto", new Vector3(0f, 1f, 0f), new Color(1f, 0.55f, 0.55f));

            SpriteRenderer sr = enemies[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.86f, 0.35f, 0.35f);
            }
        }
    }

    private void ApplyVisualToObject(string objectName, Color color, string label, Vector3 labelOffset)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null)
        {
            return;
        }

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }

        AddWorldLabel(obj, label, labelOffset, new Color(0.98f, 0.95f, 0.8f));
    }

    private void AddWorldLabel(GameObject target, string label, Vector3 offset, Color textColor)
    {
        if (target.transform.Find("ThemeLabel") != null)
        {
            return;
        }

        GameObject labelObj = new GameObject("ThemeLabel");
        labelObj.transform.SetParent(target.transform, false);
        labelObj.transform.localPosition = offset;

        TextMeshPro text = labelObj.AddComponent<TextMeshPro>();
        text.text = label;
        text.fontSize = 3f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = textColor;
        text.outlineWidth = 0.15f;
    }
}
