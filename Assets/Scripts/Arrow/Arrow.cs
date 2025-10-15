using System.Collections.Generic;
using UnityEngine;

public enum ArrowDirection
{
    Up,
    Down,
    Left,
    Right
}

[System.Serializable]
public class Arrow
{
    public int id;
    public ArrowDirection direction;
    public List<Vector2Int> path = new List<Vector2Int>();

    public Arrow(int id)
    {
        this.id = id;
    }

    public void SetDirection(ArrowDirection inDirection)
    {
        this.direction = inDirection;
    }

    public void AddNode(Vector2Int inPosition)
    {
        path.Add(inPosition);
    }

    public int Length => path.Count;

    public Vector2Int Head => path.Count > 0 ? path[0] : Vector2Int.zero;
    public Vector2Int Tail => path.Count > 0 ? path[^1] : Vector2Int.zero;
}
