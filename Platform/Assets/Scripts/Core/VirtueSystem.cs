using UnityEngine;
using UnityEngine.SceneManagement;

public class VirtueSystem : MonoBehaviour
{
    public static VirtueSystem Instance { get; private set; }
    private static Sprite fallbackSprite;

    [Header("Virtudes")]
    [SerializeField] private bool hasCharity;
    [SerializeField] private bool hasFortitude;

    [Header("Movimento")]
    [SerializeField] private bool doubleJumpUnlocked;

    public bool HasCharity => hasCharity;
    public bool HasFortitude => hasFortitude;
    public bool HasDoubleJump => doubleJumpUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        EnsureVisibleSpritesInActiveScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureVisibleSpritesInActiveScene();
    }

    private void EnsureVisibleSpritesInActiveScene()
    {
        Sprite visibleFallback = GetOrCreateFallbackSprite();
        SpriteRenderer[] renderers = FindObjectsOfType<SpriteRenderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].sprite == null)
            {
                renderers[i].sprite = visibleFallback;
            }
        }
    }

    private static Sprite GetOrCreateFallbackSprite()
    {
        if (fallbackSprite != null)
        {
            return fallbackSprite;
        }

        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        fallbackSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        fallbackSprite.name = "RuntimeFallbackSprite";
        return fallbackSprite;
    }

    public bool HasVirtue(RequiredVirtue virtue)
    {
        return virtue switch
        {
            RequiredVirtue.Charity => hasCharity,
            RequiredVirtue.Fortitude => hasFortitude,
            _ => false,
        };
    }

    public void UnlockCharity()
    {
        hasCharity = true;
    }

    public void UnlockFortitude()
    {
        hasFortitude = true;
    }

    public void UnlockDoubleJump()
    {
        doubleJumpUnlocked = true;
    }

    public void ResetVirtues()
    {
        hasCharity = false;
        hasFortitude = false;
        doubleJumpUnlocked = false;
    }
}

public enum RequiredVirtue
{
    Charity,
    Fortitude,
}
