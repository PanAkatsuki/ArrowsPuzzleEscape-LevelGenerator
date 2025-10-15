using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ArrowClickHandler : MonoBehaviour
{
    private int arrowId;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnArrowClicked();
            }
        }
    }

    public void Initialize(int id)
    {
        arrowId = id;
    }

    private void OnArrowClicked()
    {
        //Debug.Log("Clicked Arrow: " + name);
        List<Arrow> arrowList = ArrowGenerator.Instance.GetArrayList();
        Tile[,] gridList = LevelGenerator.Instance.GetGridList();
        int gridMapWidth = gridList.GetLength(0);
        int gridMapHeight = gridList.GetLength(1);

        Arrow arrow = arrowList.Find(a => a.id == arrowId);
        if (arrow == null)
        {
            Debug.LogWarning($"Arrow {arrowId} not found in ArrowList.");
            return;
        }

        Vector2Int headPos = arrow.Head;
        ArrowDirection dir = arrow.direction;

        Debug.Log($"Clicked Arrow {arrowId}, Head={headPos}, Direction={dir}");
        switch(dir)
        {
            case ArrowDirection.Right:
                if (headPos.x == gridMapWidth - 1)
                {
                    ResetTileType(arrow, gridList);
                    DeleteArrow(arrow, arrowList);
                }
                else
                {
                    for (int i = headPos.x + 1; i < gridMapWidth; i++)
                    {
                        if (gridList[i, headPos.y].type == TileType.Occupy)
                        {
                            Debug.Log($"Can NOT remove Arrow {arrow.id}.");
                            break;
                        }

                        if (i == gridMapWidth - 1)
                        {
                            ResetTileType(arrow, gridList);
                            DeleteArrow(arrow, arrowList);
                        }
                    }
                }
                break;
            case ArrowDirection.Left:
                if (headPos.x == 0)
                {
                    ResetTileType(arrow, gridList);
                    DeleteArrow(arrow, arrowList);
                }
                else
                {
                    for (int i = headPos.x - 1; i >= 0; i--)
                    {
                        if (gridList[i, headPos.y].type == TileType.Occupy)
                        {
                            Debug.Log($"Can NOT remove Arrow {arrow.id}.");
                            break;
                        }

                        if (i == 0)
                        {
                            ResetTileType(arrow, gridList);
                            DeleteArrow(arrow, arrowList);
                        }
                    }
                }
                break;
            case ArrowDirection.Up:
                if (headPos.y == gridMapHeight - 1)
                {
                    ResetTileType(arrow, gridList);
                    DeleteArrow(arrow, arrowList);
                }
                else
                {
                    for (int j = headPos.y + 1; j < gridMapHeight; j++)
                    {
                        if (gridList[headPos.x, j].type == TileType.Occupy)
                        {
                            Debug.Log($"Can NOT remove Arrow {arrow.id}.");
                            break;
                        }

                        if (j == gridMapHeight - 1)
                        {
                            ResetTileType(arrow, gridList);
                            DeleteArrow(arrow, arrowList);
                        }
                    }
                }
                break;
            case ArrowDirection.Down:
                if (headPos.y == 0)
                {
                    ResetTileType(arrow, gridList);
                    DeleteArrow(arrow, arrowList);
                }
                else
                {
                    for (int j = headPos.y - 1; j >= 0; j--)
                    {
                        if (gridList[headPos.x, j].type == TileType.Occupy)
                        {
                            Debug.Log($"Can NOT remove Arrow {arrow.id}.");
                            break;
                        }

                        if (j == 0)
                        {
                            ResetTileType(arrow, gridList);
                            DeleteArrow(arrow, arrowList);
                        }
                    }
                }
                break;
        }
    }

    private void ResetTileType(Arrow inArrow, Tile[,] inTileList)
    {
        if (inArrow == null || inArrow.path == null)
        {
            Debug.LogWarning("ResetTileType: Arrow or path is null.");
            return;
        }

        int width = inTileList.GetLength(0);
        int height = inTileList.GetLength(1);

        foreach (Vector2Int position in inArrow.path)
        {
            if (position.x >= 0 && position.x < width && position.y >= 0 && position.y < height)
            {
                inTileList[position.x, position.y].type = TileType.Empty;
            }
            else
            {
                Debug.LogWarning($"ResetTileType: Over range {position}");
            }
        }
    }

    private void DeleteArrow(Arrow inArrow, List<Arrow> inArrowList)
    {
        inArrowList.Remove(inArrow);

        GameObject arrowObject = GameObject.Find($"Arrow_{inArrow.id}");
        if (arrowObject != null)
        {
            GameObject.Destroy(arrowObject);
        }

        Debug.Log($"Deleted Arrow {inArrow.id}");
        LevelGenerator.Instance.CheckWin();
    }
}
