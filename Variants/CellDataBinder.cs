using UnityEngine;
using UnityEngine.EventSystems;

public class CellDataBinder : MonoBehaviour, IPointerClickHandler
{
    public SpriteDataAsset assignedData;
    private static CellDataBinder lastClickedCell;
    public GameObject cellFreePrefab;
    public bool isFree;
    public bool isInitialized = false;
    public GameObject[] particleEffectPrefabs; // ������ ������ ������
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked on {gameObject.name} with data {assignedData?.name}");

        // ��������� ������� ���� �� ����� � ��� �� ������
        if (lastClickedCell == this)
        {
            Debug.Log("Double click on the same cell detected. No action performed.");
            lastClickedCell = null; // ����������
            return;
        }

        if (lastClickedCell == null)
        {
            lastClickedCell = this;
            return;
        }

        // ��������� ������ �����
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
        // ����� ��������� ������� ������
        SpawnRandomParticleEffect(cellA.transform.position);
        SpawnRandomParticleEffect(cellB.transform.position);

        // ������� ������ ������
        GameObject freeCellA = Instantiate(cellFreePrefab, parentA);
        freeCellA.transform.SetSiblingIndex(indexA);
        GameObject freeCellB = Instantiate(cellFreePrefab, parentB);
        freeCellB.transform.SetSiblingIndex(indexB);

        // ����������� �������� ��������� �����
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

        // ���������� ������ �������
        Destroy(cellA.gameObject);
        Destroy(cellB.gameObject);

        // ��������� ������� ��������
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

        // �������� ��������� prefab
        int randomIndex = Random.Range(0, particleEffectPrefabs.Length);
        GameObject selectedPrefab = particleEffectPrefabs[randomIndex];

        // ������� ������� ������
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