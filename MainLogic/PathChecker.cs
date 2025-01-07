using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathChecker : MonoBehaviour
{
    public Transform gridParent;

    public bool AreCellsAdjacent(CellDataBinder cellA, CellDataBinder cellB)
    {
        Vector2Int posA = GetCellPosition(cellA);
        Vector2Int posB = GetCellPosition(cellB);
        int distance = Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        return distance == 1;
    }
    [SerializeField]private int totalCells; // Общее количество ячеек фиксируется при старте

    private void Start()
    {
        // Фиксируем общее количество дочерних объектов
        totalCells = gridParent.childCount;
        Debug.Log($"Initialized totalCells: {totalCells}");
    }

    public bool AreAllCellsFree()
    {
        int freeCells = 0;

        foreach (Transform child in gridParent)
        {
            CellDataBinder binder = child.GetComponent<CellDataBinder>();
            if (binder != null && binder.isFree)
            {
                freeCells++;
            }
        }

        Debug.Log($"Total Cells: {totalCells}, Free Cells: {freeCells}");
        return totalCells == freeCells; // Проверяем, свободны ли все ячейки
    }
    private Vector2Int GetCellPosition(CellDataBinder cell)
    {
        int siblingIndex = cell.transform.GetSiblingIndex();
        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();

        if (gridLayout == null)
        {
            Debug.LogError("Parent does not have a GridLayoutGroup.");
            return Vector2Int.zero;
        }

        int gridWidth = gridLayout.constraintCount;
        int x = siblingIndex % gridWidth;
        int y = siblingIndex / gridWidth;

        Debug.Log($"Cell {cell.name} at siblingIndex {siblingIndex} has position ({x}, {y})");
        return new Vector2Int(x, y);
    }

    public bool IsPathAvailable(CellDataBinder cellA, CellDataBinder cellB)
    {
        Vector2Int start = GetCellPosition(cellA);
        Vector2Int target = GetCellPosition(cellB);

        Debug.Log($"Checking path from {start} to {target}. StartFree: {cellA.isFree}, TargetFree: {cellB.isFree}");

        var openSet = new SortedSet<Vector2Int>(Comparer<Vector2Int>.Create((a, b) =>
        {
            float fA = GetCost(a, start, target);
            float fB = GetCost(b, start, target);
            return fA == fB ? a.GetHashCode().CompareTo(b.GetHashCode()) : fA.CompareTo(fB);
        }));

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();

        openSet.Add(start);
        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Min;
            openSet.Remove(current);

            Debug.Log($"Current node: {current}");

            if (current == target)
            {
                Debug.Log($"Path found from {start} to {target}");
                ReconstructPath(cameFrom, current);
                return true;
            }

            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                Debug.Log($"Evaluating neighbor: {neighbor}");

                if (closedSet.Contains(neighbor))
                {
                    Debug.Log($"Neighbor {neighbor} is in closedSet.");
                    continue;
                }

                if (!IsCellFree(neighbor) && neighbor != target)
                {
                    Debug.Log($"Neighbor {neighbor} is not free and not the target.");
                    continue;
                }

                float tentativeGScore = gScore[current] + 1;
                Debug.Log($"Tentative gScore for {neighbor}: {tentativeGScore}");

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    gScore[neighbor] = tentativeGScore;
                    cameFrom[neighbor] = current;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                        Debug.Log($"Added {neighbor} to openSet with gScore {tentativeGScore}");
                    }
                }
            }
        }

        Debug.LogWarning($"No path found from {start} to {target}");
        return false;
    }

    private void ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        Debug.Log($"Path: {string.Join(" -> ", path)}");
    }   
   
    float GetCost(Vector2Int position, Vector2Int start, Vector2Int target)
    {
        float g = Mathf.Abs(position.x - start.x) + Mathf.Abs(position.y - start.y); // Расстояние до текущей точки
        float h = Mathf.Abs(position.x - target.x) + Mathf.Abs(position.y - target.y); // Эвристика
        return g + h;
    }
    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
    {
        new Vector2Int(position.x + 1, position.y),
        new Vector2Int(position.x - 1, position.y),
        new Vector2Int(position.x, position.y + 1),
        new Vector2Int(position.x, position.y - 1)
    };

        int gridWidth = gridParent.GetComponent<GridLayoutGroup>().constraintCount;
        int gridHeight = Mathf.CeilToInt((float)gridParent.childCount / gridWidth);

        neighbors.RemoveAll(neighbor =>
            neighbor.x < 0 || neighbor.y < 0 ||
            neighbor.x >= gridWidth || neighbor.y >= gridHeight);

        return neighbors;
    }

    private bool IsCellFree(Vector2Int position)
    {
        int gridWidth = gridParent.GetComponent<GridLayoutGroup>().constraintCount;
        int siblingIndex = position.y * gridWidth + position.x;

        if (siblingIndex < 0 || siblingIndex >= gridParent.childCount)
        {
            Debug.LogWarning($"Invalid sibling index: {siblingIndex}");
            return false;
        }

        Transform cell = gridParent.GetChild(siblingIndex);
        CellDataBinder binder = cell.GetComponent<CellDataBinder>();

        if (binder == null)
        {
            Debug.LogWarning($"Cell at position {position} does not have a CellDataBinder component.");
            return false;
        }

        return binder.isFree;
    }

    public Vector3 GetWorldPositionFromGridPosition(Vector2Int gridPosition)
    {
        int gridWidth = gridParent.GetComponent<GridLayoutGroup>().constraintCount;

        // Вычисляем индекс объекта
        int index = gridPosition.y * gridWidth + gridPosition.x;

        if (index >= 0 && index < gridParent.childCount)
        {
            Transform cellTransform = gridParent.GetChild(index);
            return cellTransform.position;
        }

        return Vector3.zero;
    }
}
