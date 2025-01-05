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
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked on {gameObject.name} with data {assignedData?.name}");

        // Проверяем двойной клик на одной и той же ячейке
        if (lastClickedCell == this)
        {
            Debug.Log("Double click on the same cell detected. No action performed.");
            lastClickedCell = null; // Сбрасываем
            return;
        }

        if (lastClickedCell == null)
        {
            lastClickedCell = this;
            return;
        }

        // Проверяем данные ячеек
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

    private void DestroyCells(CellDataBinder cellA, CellDataBinder cellB)
    {
        Debug.Log("DestroyCells called.");

        if (cellA == cellB)
        {
            Debug.LogError("Attempted to destroy the same cell. Action aborted.");
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

        int indexA = cellA.transform.GetSiblingIndex();
        int indexB = cellB.transform.GetSiblingIndex();

        if (cellFreePrefab == null)
        {
            Debug.LogError("cellFreePrefab is not assigned.");
            return;
        }
        // Спавн случайной системы частиц
        SpawnRandomParticleEffect(cellA.transform.position);
        SpawnRandomParticleEffect(cellB.transform.position);

        // Создаем пустые ячейки
        GameObject freeCellA = Instantiate(cellFreePrefab, parentA);
        freeCellA.transform.SetSiblingIndex(indexA);
        GameObject freeCellB = Instantiate(cellFreePrefab, parentB);
        freeCellB.transform.SetSiblingIndex(indexB);

        // Настраиваем свойства свободных ячеек
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

        // Уничтожаем старые объекты
        Destroy(cellA.gameObject);
        Destroy(cellB.gameObject);

        // Проверяем условие выигрыша
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