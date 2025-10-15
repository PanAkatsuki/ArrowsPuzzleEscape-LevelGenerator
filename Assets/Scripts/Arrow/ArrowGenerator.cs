using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public static ArrowGenerator Instance { get; private set; }

    [Header("Arrow Prefabs")]
    public List<GameObject> arrowPrefabList;

    // Dictionary for arrow prefabs
    private Dictionary<(ArrowDirection, Vector2Int), int> arrowHeadPrefabIndexMap;
    private Dictionary<(Vector2Int, Vector2Int), int> arrowBodyPrefabIndexMap;
    private Dictionary<Vector2Int, int> arrowTailPrefabIndexMap;

    [SerializeField]
    private List<Arrow> arrowList = new List<Arrow>();

    [Header("Generation Settings")]
    [UnityEngine.Range(0, 5)] public int arrowMinLength = 5;
    [UnityEngine.Range(0, 20)] public int arrowMaxLength = 10;
    [UnityEngine.Range(0f, 1f)] public float arrowGenerationChangeDirectionChance = 0.3f;

    private void OnValidate()
    {
        if (arrowMinLength > arrowMaxLength)
            arrowMaxLength = arrowMinLength;
    }

    void Awake()
    {
        Instance = this;

        arrowHeadPrefabIndexMap = new Dictionary<(ArrowDirection, Vector2Int), int>
        {
            { (ArrowDirection.Right, Vector2Int.left), 0 },
            { (ArrowDirection.Left, Vector2Int.right), 1 },
            { (ArrowDirection.Up, Vector2Int.down), 2 },
            { (ArrowDirection.Down, Vector2Int.up), 3 },
            { (ArrowDirection.Right, Vector2Int.up), 4 },
            { (ArrowDirection.Right, Vector2Int.down), 5 },
            { (ArrowDirection.Left, Vector2Int.up), 6 },
            { (ArrowDirection.Left, Vector2Int.down), 7},
            { (ArrowDirection.Up, Vector2Int.right), 8},
            { (ArrowDirection.Up, Vector2Int.left), 9},
            { (ArrowDirection.Down, Vector2Int.right), 10},
            { (ArrowDirection.Down, Vector2Int.left), 11},
        };

        arrowBodyPrefabIndexMap = new Dictionary<(Vector2Int, Vector2Int), int>
        {
            { (Vector2Int.right, Vector2Int.left), 12},
            { (Vector2Int.left, Vector2Int.right), 12},
            { (Vector2Int.up, Vector2Int.down), 13},
            { (Vector2Int.down, Vector2Int.up), 13},
            { (Vector2Int.right, Vector2Int.up), 14},
            { (Vector2Int.up, Vector2Int.right), 14},
            { (Vector2Int.right, Vector2Int.down), 15},
            { (Vector2Int.down, Vector2Int.right), 15},
            { (Vector2Int.left, Vector2Int.up), 16},
            { (Vector2Int.up, Vector2Int.left), 16},
            { (Vector2Int.left, Vector2Int.down), 17},
            { (Vector2Int.down, Vector2Int.left), 17},
        };

        arrowTailPrefabIndexMap = new Dictionary<Vector2Int, int>
        {
            { Vector2Int.right, 12 },
            { Vector2Int.left, 12 },
            { Vector2Int.up, 13 },
            { Vector2Int.down, 13 },
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool allOccupied = false;
        while(!allOccupied)
        {
            GenerateMainArrow();

            allOccupied = LevelGenerator.Instance.GetGridList().Cast<Tile>().All(tile => tile.type == TileType.Occupy);
        }
    }

    private void GenerateMainArrow()
    {
        // Find exit position
        List<Vector2Int> exitPositionList = new List<Vector2Int>();

        int gridMapWidth = LevelGenerator.Instance.GetGridList().GetLength(0);
        int gridMapHeight = LevelGenerator.Instance.GetGridList().GetLength(1);
        Tile[,] gridList = LevelGenerator.Instance.GetGridList();

        // Search position can be used for exit point
        for (int x = 0; x < gridMapWidth; x++)
        {
            for (int y = 0; y < gridMapHeight; y++)
            {
                if (gridList[x, y].type == TileType.Occupy)
                {
                    continue;
                }

                // Add edge point to list
                bool isEdge = (x == 0 || x == gridMapWidth - 1 || y == 0 || y == gridMapHeight - 1);
                if (isEdge)
                {
                    Vector2Int newExitPosition = new Vector2Int(x, y);
                    if (!exitPositionList.Contains(newExitPosition))
                    {
                        exitPositionList.Add(newExitPosition);
                    }
                }

                // Add exit point(not edge point) to list
                bool isExit = false;
                for (int i = x - 1; i >=0; i--)
                {
                    if (isExit || gridList[i, y].type == TileType.Empty)
                    {
                        break;
                    }

                    if (i == 0)
                    {
                        isExit = true;
                    }
                }

                for (int i = x + 1; i < gridMapWidth; i++)
                {
                    if (isExit || gridList[i, y].type == TileType.Empty)
                    {
                        break;
                    }

                    if (i == gridMapWidth - 1)
                    {
                        isExit = true;
                    }
                }

                for (int j = y - 1; j >= 0; j--)
                {
                    if (isExit || gridList[x, j].type == TileType.Empty)
                    {
                        break;
                    }

                    if (j == 0)
                    {
                        isExit = true;
                    }
                }

                for (int j = y + 1; j < gridMapHeight; j++)
                {
                    if (isExit || gridList[x, j].type == TileType.Empty)
                    {
                        break;
                    }

                    if (j == gridMapHeight - 1)
                    {
                        isExit = true;
                    }
                }
                
                if (isExit)
                {
                    Vector2Int newExitPosition = new Vector2Int(x, y);
                    if (!exitPositionList.Contains(newExitPosition))
                    {
                        exitPositionList.Add(newExitPosition);
                    }
                }
            }
        }

        Vector2Int exitPosition = new Vector2Int(0, 0);
        if (exitPositionList.Count > 0)
        {
            int randomIndex = Random.Range(0, exitPositionList.Count);
            exitPosition = exitPositionList[randomIndex];
        }
        else
        {
            // There is no point can be exit point
            return;
        }

        // Set possible direction and find final direction
        List<ArrowDirection> possibleDirectionList = new List<ArrowDirection>();

        bool isLeftEdge = exitPosition.x == 0;
        bool isRightEdge = exitPosition.x == gridMapWidth - 1;
        bool isBottomEdge = exitPosition.y == 0;
        bool isTopEdge = exitPosition.y == gridMapHeight - 1;

        if (isLeftEdge) possibleDirectionList.Add(ArrowDirection.Left);
        if (isRightEdge) possibleDirectionList.Add(ArrowDirection.Right);
        if (isTopEdge) possibleDirectionList.Add(ArrowDirection.Up);
        if (isBottomEdge) possibleDirectionList.Add(ArrowDirection.Down);

        if (!isLeftEdge && !isRightEdge && !isTopEdge && !isBottomEdge)
        {
            for (int i = exitPosition.x - 1; i >= 0; i--)
            {
                if (gridList[i, exitPosition.y].type == TileType.Empty)
                {
                    break;
                }

                if (i == 0)
                {
                    possibleDirectionList.Add(ArrowDirection.Left);
                }
            }

            for (int i = exitPosition.x + 1; i < gridMapWidth; i++)
            {
                if (gridList[i, exitPosition.y].type == TileType.Empty)
                {
                    break;
                }

                if (i == gridMapWidth - 1)
                {
                    possibleDirectionList.Add(ArrowDirection.Right);
                }
            }

            for (int j = exitPosition.y - 1; j >= 0; j--)
            {
                if (gridList[exitPosition.x, j].type == TileType.Empty)
                {
                    break;
                }

                if (j == 0)
                {
                    possibleDirectionList.Add(ArrowDirection.Down);
                }
            }

            for (int j = exitPosition.y + 1; j < gridMapHeight; j++)
            {
                if (gridList[exitPosition.x, j].type == TileType.Empty)
                {
                    break;
                }

                if (j == gridMapHeight - 1)
                {
                    possibleDirectionList.Add(ArrowDirection.Up);
                }
            }
        }

        ArrowDirection finalDir = possibleDirectionList[Random.Range(0, possibleDirectionList.Count)];

        // Create head of arrow
        gridList[exitPosition.x, exitPosition.y].type = TileType.Occupy;
        Arrow arrow = new Arrow(arrowList.Count);
        arrow.SetDirection(finalDir);
        arrow.AddNode(exitPosition);

        // Generate arrow body
        int bodyLength = Random.Range(arrowMinLength - 1, arrowMaxLength - 1);

        bool isCanGenerate = true;
        for (int i = 0; i < bodyLength; i++)
        {
            (arrow, exitPosition, finalDir, isCanGenerate) = GenerateArrowBody(arrow, exitPosition, finalDir, isCanGenerate);
            if (!isCanGenerate)
            {
                break;
            }
        }

        // Add arrow to list
        CreateArrowVisual(arrow);
        arrowList.Add(arrow);
    }

    private (Arrow, Vector2Int, ArrowDirection, bool) GenerateArrowBody(Arrow inArrow, Vector2Int inStartPosition, ArrowDirection inStartDirection, bool inIsCanGenerate)
    {
        int gridMapWidth = LevelGenerator.Instance.GetGridList().GetLength(0);
        int gridMapHeight = LevelGenerator.Instance.GetGridList().GetLength(1);
        Tile[,] gridList = LevelGenerator.Instance.GetGridList();

        Vector2Int currentPosition = inStartPosition;
        ArrowDirection currentDirection = inStartDirection;

        // Find possible next position
        List<Vector2Int> nextPossiblePositionList = new List<Vector2Int>();

        Vector2Int nextPossiblePosition_0 = new Vector2Int(currentPosition.x - 1, currentPosition.y);
        Vector2Int nextPossiblePosition_1 = new Vector2Int(currentPosition.x + 1, currentPosition.y);
        Vector2Int nextPossiblePosition_2 = new Vector2Int(currentPosition.x, currentPosition.y - 1);
        Vector2Int nextPossiblePosition_3 = new Vector2Int(currentPosition.x, currentPosition.y + 1);
        Vector2Int gridSize = new Vector2Int(gridMapWidth, gridMapHeight);

        if (CheckPositionLegel(nextPossiblePosition_0, gridSize) && gridList[nextPossiblePosition_0.x, nextPossiblePosition_0.y].type == TileType.Empty)
        {
            nextPossiblePositionList.Add(new Vector2Int(nextPossiblePosition_0.x, nextPossiblePosition_0.y));
        }

        if (CheckPositionLegel(nextPossiblePosition_1, gridSize) && gridList[nextPossiblePosition_1.x, nextPossiblePosition_1.y].type == TileType.Empty)
        {
            nextPossiblePositionList.Add(new Vector2Int(nextPossiblePosition_1.x, nextPossiblePosition_1.y));
        }

        if (CheckPositionLegel(nextPossiblePosition_2, gridSize) && gridList[nextPossiblePosition_2.x, nextPossiblePosition_2.y].type == TileType.Empty)
        {
            nextPossiblePositionList.Add(new Vector2Int(nextPossiblePosition_2.x, nextPossiblePosition_2.y));
        }

        if (CheckPositionLegel(nextPossiblePosition_3, gridSize) && gridList[nextPossiblePosition_3.x, nextPossiblePosition_3.y].type == TileType.Empty)
        {
            nextPossiblePositionList.Add(new Vector2Int(nextPossiblePosition_3.x, nextPossiblePosition_3.y));
        }

        if (nextPossiblePositionList.Count == 0)
        {
            // There is no place to generate body
            return (inArrow, currentPosition, currentDirection, false);
        }

        // Decide if change direction
        float randomValue = Random.value; // 0~1
        Vector2Int nextPosition = new Vector2Int(0, 0);
        ArrowDirection nextDirection = ArrowDirection.Right;

        if ((currentDirection == ArrowDirection.Right && nextPossiblePositionList.Contains(nextPossiblePosition_0)) ||
            (currentDirection == ArrowDirection.Left && nextPossiblePositionList.Contains(nextPossiblePosition_1)) ||
            (currentDirection == ArrowDirection.Up && nextPossiblePositionList.Contains(nextPossiblePosition_2)) ||
            (currentDirection == ArrowDirection.Down && nextPossiblePositionList.Contains(nextPossiblePosition_3)))
        {
            if (randomValue < arrowGenerationChangeDirectionChance)
            {
                // Decide next position by changing direction
                if (nextPossiblePositionList.Count > 1)
                {
                    switch (currentDirection)
                    {
                        // Remove function will ONLY remove the first item found in list, should make sure list doesn't have same item.
                        case ArrowDirection.Right: nextPossiblePositionList.Remove(nextPossiblePosition_0); break;
                        case ArrowDirection.Left: nextPossiblePositionList.Remove(nextPossiblePosition_1); break;
                        case ArrowDirection.Up: nextPossiblePositionList.Remove(nextPossiblePosition_2); break;
                        case ArrowDirection.Down: nextPossiblePositionList.Remove(nextPossiblePosition_3); break;
                    }
                }

                nextPosition = nextPossiblePositionList[Random.Range(0, nextPossiblePositionList.Count)];
                nextDirection = GetDirectionFromDelta(currentPosition - nextPosition);
            }
            else
            {
                // Decide next position by direction
                switch (currentDirection)
                {
                    case ArrowDirection.Right: nextPosition = nextPossiblePosition_0;break;
                    case ArrowDirection.Left: nextPosition = nextPossiblePosition_1; break;
                    case ArrowDirection.Up: nextPosition = nextPossiblePosition_2; break;
                    case ArrowDirection.Down: nextPosition = nextPossiblePosition_3; break;
                }
                nextDirection = GetDirectionFromDelta(currentPosition - nextPosition);
            }
        }
        else
        {
            // There is no choice for deciding next position by direction
            switch (currentDirection)
            {
                case ArrowDirection.Right: nextPossiblePositionList.Remove(nextPossiblePosition_0); break;
                case ArrowDirection.Left: nextPossiblePositionList.Remove(nextPossiblePosition_1); break;
                case ArrowDirection.Up: nextPossiblePositionList.Remove(nextPossiblePosition_2); break;
                case ArrowDirection.Down: nextPossiblePositionList.Remove(nextPossiblePosition_3); break;
            }

            if (nextPossiblePositionList.Count > 0)
            {
                nextPosition = nextPossiblePositionList[Random.Range(0, nextPossiblePositionList.Count)];
                nextDirection = GetDirectionFromDelta(currentPosition - nextPosition);
            }
            else
            {
                // Can not generate body
                return (inArrow, currentPosition, currentDirection, false);
            }
        }

        // Create arrow body
        gridList[nextPosition.x, nextPosition.y].type = TileType.Occupy;
        inArrow.AddNode(nextPosition);

        return (inArrow, nextPosition, nextDirection, true);
    }

    private GameObject CreateArrowVisual(Arrow inArrow)
    {
        GameObject arrowParent = new GameObject($"Arrow_{inArrow.id}");
        arrowParent.transform.SetParent(this.transform);

        Tile[,] gridList = LevelGenerator.Instance.GetGridList();

        foreach (var position in inArrow.path)
        {
            Vector3 worldPosition = gridList[position.x, position.y].transform.position;

            if (position == inArrow.Head)
            {
                // Set head prefab
                if (inArrow.path.Count == 1)
                {
                    int Index = 0;
                    switch (inArrow.direction)
                    {
                        case ArrowDirection.Right:
                            Index = 0;
                            break;
                        case ArrowDirection.Left:
                            Index = 1; 
                            break;
                        case ArrowDirection.Up:
                            Index = 2;
                            break;
                        case ArrowDirection.Down:
                            Index = 3;
                            break;
                    }
                    GameObject prefab = arrowPrefabList[Index];
                    GameObject nodeObject = Instantiate(prefab, worldPosition, prefab.transform.rotation);

                    // Set collider
                    var collider = nodeObject.AddComponent<BoxCollider2D>();
                    collider.isTrigger = false;
                    nodeObject.AddComponent<ArrowClickHandler>().Initialize(inArrow.id);

                    nodeObject.transform.SetParent(arrowParent.transform);
                }
                else
                {
                    Vector2Int delta = inArrow.path[1] - inArrow.Head;
                    if (arrowHeadPrefabIndexMap.TryGetValue((inArrow.direction, delta), out int prefabIndex))
                    {
                        GameObject prefab = arrowPrefabList[prefabIndex];
                        GameObject nodeObject = Instantiate(prefab, worldPosition, prefab.transform.rotation);

                        // Set collider
                        var collider = nodeObject.AddComponent<BoxCollider2D>();
                        collider.isTrigger = false;
                        nodeObject.AddComponent<ArrowClickHandler>().Initialize(inArrow.id);

                        nodeObject.transform.SetParent(arrowParent.transform);
                    }
                }   
            }
            else if (position == inArrow.Tail)
            {
                // Set tail prefab
                Vector2Int delta = inArrow.Tail - inArrow.path[^2];
                if (arrowTailPrefabIndexMap.TryGetValue(delta, out int prefabIndex))
                {
                    GameObject prefab = arrowPrefabList[prefabIndex];
                    GameObject nodeObject = Instantiate(prefab, worldPosition, prefab.transform.rotation);

                    // Set collider
                    var collider = nodeObject.AddComponent<BoxCollider2D>();
                    collider.isTrigger = false;
                    nodeObject.AddComponent<ArrowClickHandler>().Initialize(inArrow.id);

                    nodeObject.transform.SetParent(arrowParent.transform);
                }
            }
            else
            {
                // Set body prefab
                int pathIndex = inArrow.path.IndexOf(position);
                Vector2Int delta_1 = inArrow.path[pathIndex - 1] - inArrow.path[pathIndex];
                Vector2Int delta_2 = inArrow.path[pathIndex + 1] - inArrow.path[pathIndex];
                if (arrowBodyPrefabIndexMap.TryGetValue((delta_1, delta_2), out int prefabIndex))
                {
                    GameObject prefab = arrowPrefabList[prefabIndex];
                    GameObject nodeObject = Instantiate(prefab, worldPosition, prefab.transform.rotation);

                    // Set collider
                    var collider = nodeObject.AddComponent<BoxCollider2D>();
                    collider.isTrigger = false;
                    nodeObject.AddComponent<ArrowClickHandler>().Initialize(inArrow.id);

                    nodeObject.transform.SetParent(arrowParent.transform);
                }
            }
        }

        return arrowParent;
    }

    private bool CheckPositionLegel(Vector2Int inPosition, Vector2Int inGridSize)
    {
        if (inPosition.x >= 0 && inPosition.x < inGridSize.x && inPosition.y >= 0 && inPosition.y < inGridSize.y) return true;

        return false;
    }

    private ArrowDirection GetDirectionFromDelta(Vector2Int delta)
    {
        if (delta == Vector2Int.up) return ArrowDirection.Up;
        if (delta == Vector2Int.down) return ArrowDirection.Down;
        if (delta == Vector2Int.left) return ArrowDirection.Left;
        if (delta == Vector2Int.right) return ArrowDirection.Right;
        return ArrowDirection.Up; // fallback
    }

    public List<Arrow> GetArrayList()
    {
        return arrowList;
    }
}
