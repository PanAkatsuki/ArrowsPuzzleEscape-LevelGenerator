using UnityEngine;

public enum TileType
{
    Empty,
    Occupy
}

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TileType type;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        switch (type)
        {
            case TileType.Empty:
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                break;

            case TileType.Occupy:
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                break;
        }
    }
}