using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellDataBinder : MonoBehaviour, IPointerClickHandler
{
    public SpriteDataAsset assignedData;
    private static CellDataBinder lastClickedCell;
    public GameObject cellFreePrefab;
    public bool isFree;
    public bool isInitialized = false;
    public GameObject[] particleEffectPrefabs; // Массив систем частиц
    private static List<CellDataBinder> clickHistory = new List<CellDataBinder>(); // История кликов
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked on {gameObject.name} with data {assignedData?.name}");

        if (isFree)
        {
            Debug.Log("Clicked on a free cell. Ignoring.");
            return;
        }

        clickHistory.RemoveAll(cell => cell == null || !cell.gameObject.activeInHierarchy);

        if (clickHistory.Count > 0 && clickHistory[^1] == this)
        {
            Debug.LogWarning("Clicked on the same cell twice consecutively. Ignoring.");
            return;
        }

        clickHistory.Add(this);

        if (clickHistory.Count > 2)
        {
            clickHistory.RemoveAt(0);
        }

        if (clickHistory.Count == 2)
        {
            CellDataBinder firstCell = clickHistory[0];
            CellDataBinder secondCell = clickHistory[1];

            Debug.Log($"Attempting to match {firstCell.name} and {secondCell.name}");

            if (firstCell.assignedData == secondCell.assignedData)
            {
                PathChecker pathChecker = transform.parent.GetComponent<PathChecker>();

                if (pathChecker != null &&
                    (pathChecker.AreCellsAdjacent(firstCell, secondCell) ||
                     pathChecker.IsPathAvailable(firstCell, secondCell)))
                {
                    Debug.Log("Path found. Destroying cells.");
                    DestroyCells(firstCell, secondCell);
                    ResetClickState();
                }
                else
                {
                    Debug.LogWarning("No path available between selected cells. Keeping history.");
                }
            }
        }
    }


    private void ResetClickState()
    {
        Debug.Log("Resetting click state and clearing history.");

        // Удаляем из истории кликов недействительные ссылки
        clickHistory.RemoveAll(cell => cell == null || !cell.gameObject.activeInHierarchy);

        // Полный сброс истории
        clickHistory.Clear();
    }


    private void DestroyCells(CellDataBinder cellA, CellDataBinder cellB)
    {
        if (cellA == null || cellB == null)
        {
            Debug.LogError("One or both cells are null. Cannot destroy.");
            return;
        }

        if (cellA == cellB)
        {
            Debug.LogError("Attempted to destroy the same cell twice. Action aborted.");
            ResetClickState();
            return;
        }

        PathChecker pathChecker = transform.parent.GetComponent<PathChecker>();
        if (pathChecker == null)
        {
            Debug.LogError("PathChecker component not found on parent object.");
            return;
        }

        if (cellFreePrefab == null)
        {
            Debug.LogError("cellFreePrefab is not assigned.");
            return;
        }

        Debug.Log($"Destroying cells: {cellA.name} and {cellB.name}");

        Transform parentA = cellA.transform.parent;
        Transform parentB = cellB.transform.parent;

        int indexA = cellA.transform.GetSiblingIndex();
        int indexB = cellB.transform.GetSiblingIndex();

        // Защита от повторного создания клеток
        if (parentA.GetChild(indexA).gameObject != cellA.gameObject ||
            parentB.GetChild(indexB).gameObject != cellB.gameObject)
        {
            Debug.LogWarning("Cells have already been replaced. Action aborted.");
            return;
        }

        SpawnRandomParticleEffect(cellA.transform.position);
        SpawnRandomParticleEffect(cellB.transform.position);

        GameObject freeCellA = Instantiate(cellFreePrefab, parentA);
        freeCellA.transform.SetSiblingIndex(indexA);
        GameObject freeCellB = Instantiate(cellFreePrefab, parentB);
        freeCellB.transform.SetSiblingIndex(indexB);

        CellDataBinder freeBinderA = freeCellA.GetComponent<CellDataBinder>();
        CellDataBinder freeBinderB = freeCellB.GetComponent<CellDataBinder>();

        if (freeBinderA != null)
        {
            freeBinderA.isFree = true;
            freeBinderA.isInitialized = true;
        }
        else
        {
            Debug.LogError("Free cell A does not contain a CellDataBinder component.");
        }

        if (freeBinderB != null)
        {
            freeBinderB.isFree = true;
            freeBinderB.isInitialized = true;
        }
        else
        {
            Debug.LogError("Free cell B does not contain a CellDataBinder component.");
        }

        Destroy(cellA.gameObject);
        Destroy(cellB.gameObject);

        if (pathChecker.AreAllCellsFree())
        {
            Debug.Log("Victory! All cells are free.");
            HandleVictory();
        }
    }

    private void HandleVictory()
    {
        Debug.Log("You win! Game Over.");
        CanvasButtons canvasButtons = FindObjectOfType<CanvasButtons>();
        canvasButtons.ShowWinPanel();
    }
    private void SpawnRandomParticleEffect(Vector3 position)
    {
        if (particleEffectPrefabs == null || particleEffectPrefabs.Length == 0)
        {
            Debug.LogWarning("Particle effect prefabs array is empty or not assigned.");
            return;
        }

        // Выбираем случайный prefab
        int randomIndex = Random.Range(0, particleEffectPrefabs.Length);
        GameObject selectedPrefab = particleEffectPrefabs[randomIndex];

        // Спавним систему частиц
        if (selectedPrefab != null)
        {
            Instantiate(selectedPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"Prefab at index {randomIndex} is null.");
        }
    }
}