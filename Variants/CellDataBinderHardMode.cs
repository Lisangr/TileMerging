/*using UnityEngine;
using UnityEngine.EventSystems;

public class CellDataBinderHardMode : MonoBehaviour, IPointerClickHandler
{
    public SpriteDataAsset assignedData;
    private static CellDataBinderHardMode lastClickedCell;
    public GameObject cellFreePrefab;
    public bool isFree;
    public bool isInitialized = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked on {gameObject.name} with data {assignedData?.name}");

        // ѕровер€ем двойной клик на одной и той же €чейке
        if (lastClickedCell == this)
        {
            Debug.Log("Double click on the same cell detected. No action performed.");
            lastClickedCell = null; // —брасываем
            return;
        }

        if (lastClickedCell == null)
        {
            lastClickedCell = this;
            return;
        }

        // ѕровер€ем данные €чеек
        if (assignedData != lastClickedCell.assignedData)
        {
            Debug.Log("Cells have different data, cannot be destroyed.");
            lastClickedCell = null;
            return;
        }

        PathChecker pathChecker = transform.parent.GetComponent<PathChecker>();

        if (pathChecker.AreCellsAdjacent(this, lastClickedCell))
        {
            DestroyCells(this, lastClickedCell);
        }
        else if (pathChecker.IsPathAvailable(this, lastClickedCell))
        {
            DestroyCells(this, lastClickedCell);
        }
        else
        {
            Debug.Log("No path available.");
        }

        lastClickedCell = null;
    }
    private void DestroyCells(CellDataBinderHardMode cellA, CellDataBinderHardMode cellB)
    {
        Debug.Log($"Destroying cells: {cellA.name}, {cellB.name}");

        if (cellA == cellB)
        {
            Debug.LogError("Attempted to destroy the same cell. Aborting.");
            return;
        }

        PathChecker pathChecker = transform.parent.GetComponent<PathChecker>();
        if (pathChecker == null)
        {
            Debug.LogError("PathChecker component not found on parent object.");
            return;
        }

        Transform parentA = cellA.transform.parent;
        Transform parentB = cellB.transform.parent;

        if (cellFreePrefab == null)
        {
            Debug.LogError("cellFreePrefab is not assigned.");
            return;
        }

        // ѕеремещаем старые €чейки в конец списка
        cellA.transform.SetSiblingIndex(parentA.childCount - 1);
        cellB.transform.SetSiblingIndex(parentB.childCount - 1);

        // —оздаем пустые €чейки
        GameObject freeCellA = Instantiate(cellFreePrefab, parentA);
        GameObject freeCellB = Instantiate(cellFreePrefab, parentB);

        // ”станавливаем индексы дл€ новых €чеек
        freeCellA.transform.SetSiblingIndex(cellA.transform.GetSiblingIndex());
        freeCellB.transform.SetSiblingIndex(cellB.transform.GetSiblingIndex());

        Debug.Log($"freeCellA placed at index {freeCellA.transform.GetSiblingIndex()}, freeCellB placed at index {freeCellB.transform.GetSiblingIndex()}");

        // Ќастраиваем свойства новых €чеек
        CellDataBinder freeBinderA = freeCellA.GetComponent<CellDataBinder>();
        CellDataBinder freeBinderB = freeCellB.GetComponent<CellDataBinder>();

        if (freeBinderA != null)
        {
            freeBinderA.isFree = true;
            freeBinderA.isInitialized = true;
        }

        if (freeBinderB != null)
        {
            freeBinderB.isFree = true;
            freeBinderB.isInitialized = true;
        }

        // ”ничтожаем старые объекты
        Destroy(cellA.gameObject);
        Destroy(cellB.gameObject);

        // ѕровер€ем условие выигрыша
        if (pathChecker.AreAllCellsFree())
        {
            Debug.Log("Victory! All cells are free.");
            HandleVictory();
        }
    }

    private void VerifySiblingIndices(Transform parent)
    {
        Debug.Log($"Verifying sibling indices for parent {parent.name}");
        int previousIndex = -1;
        foreach (Transform child in parent)
        {
            int currentIndex = child.GetSiblingIndex();
            if (currentIndex <= previousIndex)
            {
                Debug.LogError($"Sibling index inconsistency detected for {child.name}");
            }
            previousIndex = currentIndex;
        }
    }

    private void HandleVictory()
    {
        Debug.Log("You win! Game Over.");
        CanvasButtons canvasButtons = FindObjectOfType<CanvasButtons>();
        if (canvasButtons == null)
        {
            Debug.LogError("CanvasButtons component not found in the scene.");
            return;
        }

        canvasButtons.ShowWinPanel();
    }
}
*/