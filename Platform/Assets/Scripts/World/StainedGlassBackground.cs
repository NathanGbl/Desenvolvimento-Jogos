using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class StainedGlassBackground : MonoBehaviour
{
    [SerializeField] private Texture2D panel01;
    [SerializeField] private Texture2D panel04;
    [SerializeField] private Texture2D panel07;
    [SerializeField] private float pixelsPerUnit = 50f;
    [SerializeField] private int sortingOrder = -100;

    private void Awake()
    {
        Build();
    }

    private void OnEnable()
    {
        Build();
    }

    public void Build()
    {
        if (panel01 == null || panel04 == null || panel07 == null)
        {
            return;
        }

        Transform existingGrid = transform.Find("VitralGrid");
        if (existingGrid != null)
        {
            return;
        }

        GameObject gridObject = new GameObject("VitralGrid");
        gridObject.transform.SetParent(transform, false);

        Grid grid = gridObject.AddComponent<Grid>();
        grid.cellSize = new Vector3(20f, 20f, 0f);

        GameObject tilemapObject = new GameObject("VitralTilemap");
        tilemapObject.transform.SetParent(gridObject.transform, false);

        Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingOrder = sortingOrder;

        SetTile(tilemap, new Vector3Int(0, 0, 0), panel01);
        SetTile(tilemap, new Vector3Int(1, 0, 0), panel04);
        SetTile(tilemap, new Vector3Int(2, 0, 0), panel07);
    }

    private void SetTile(Tilemap tilemap, Vector3Int position, Texture2D texture)
    {
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.None;
        tilemap.SetTile(position, tile);
    }
}