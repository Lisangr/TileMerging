using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GridGeneratorEditor : EditorWindow
{
    private int gridWidth; // Ширина сетки
    private int gridHeight; // Высота сетки
    private SpriteDataAsset[] spriteDataAssets; // Список объектов SpriteDataAsset
    private int selectedAssetIndex = -1; // Индекс выбранного объекта
    private int[,] gridData; // Данные сетки (хранение индексов объектов)
    private GameObject cellPrefab; // Префаб ячейки
    private Transform parentGrid; // Родительский объект с GridLayoutGroup
    private GameObject cellFreePrefab; // Префаб для пустых ячеек

    private int currentLevelNumber = 3; // Номер текущего уровня, начинается с 3

    [MenuItem("Tools/Generate Grid")]
    public static void ShowWindow()
    {
        var window = GetWindow<GridGeneratorEditor>("Grid Generator");
        window.minSize = new Vector2(500, 500);
    }

    private void OnEnable()
    {
        LoadSpriteDataAssets();
        ResetGridData();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        gridWidth = Mathf.Max(1, EditorGUILayout.IntField("Width", gridWidth));
        gridHeight = Mathf.Max(1, EditorGUILayout.IntField("Height", gridHeight));

        EditorGUILayout.Space();

        // Выбор префаба ячейки
        cellPrefab = (GameObject)EditorGUILayout.ObjectField("Cell Prefab", cellPrefab, typeof(GameObject), false);

        // Выбор префаба для пустых ячеек
        cellFreePrefab = (GameObject)EditorGUILayout.ObjectField("Cell Free Prefab", cellFreePrefab, typeof(GameObject), false);

        // Выбор родительского объекта
        parentGrid = (Transform)EditorGUILayout.ObjectField("Parent Grid", parentGrid, typeof(Transform), true);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField($"Next SpriteDataAsset Name: {currentLevelNumber}");

        if (GUILayout.Button("Reset Grid"))
        {
            ResetGridData();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Available Sprite Data Assets", EditorStyles.boldLabel);

        if (spriteDataAssets != null && spriteDataAssets.Length > 0)
        {
            string[] assetNames = new string[spriteDataAssets.Length];
            for (int i = 0; i < assetNames.Length; i++)
            {
                assetNames[i] = spriteDataAssets[i].name;
            }

            selectedAssetIndex = EditorGUILayout.Popup("Select Asset", selectedAssetIndex, assetNames);

            if (selectedAssetIndex >= 0)
            {
                EditorGUILayout.LabelField($"Selected Asset: {spriteDataAssets[selectedAssetIndex].name}");
            }
        }
        else
        {
            EditorGUILayout.LabelField("No SpriteDataAssets found in Assets/Scripts/Variants.");
        }

        EditorGUILayout.Space();

        DrawGrid();

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            GenerateGridInScene();
            ResetGridData();
        }
    }

    private void LoadSpriteDataAssets()
    {
        string path = "Assets/Scripts/Variants";
        string[] assetGuids = AssetDatabase.FindAssets("t:SpriteDataAsset", new[] { path });
        spriteDataAssets = new SpriteDataAsset[assetGuids.Length];

        for (int i = 0; i < assetGuids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
            spriteDataAssets[i] = AssetDatabase.LoadAssetAtPath<SpriteDataAsset>(assetPath);
        }
    }

    private void ResetGridData()
    {
        gridData = new int[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridData[x, y] = 0;
            }
        }
    }

    private void DrawGrid()
    {
        EditorGUILayout.LabelField("Grid Preview", EditorStyles.boldLabel);

        for (int y = 0; y < gridHeight; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < gridWidth; x++)
            {
                GUI.backgroundColor = GetColorForAsset(gridData[x, y]);

                if (GUILayout.Button("", GUILayout.Width(50), GUILayout.Height(50)))
                {
                    if (selectedAssetIndex >= 0)
                    {
                        gridData[x, y] = selectedAssetIndex + 1;
                    }
                }

                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private Color GetColorForAsset(int assetIndex)
    {
        if (assetIndex <= 0 || assetIndex > spriteDataAssets.Length)
        {
            return Color.white;
        }

        return Color.HSVToRGB((assetIndex - 1) / (float)spriteDataAssets.Length, 0.7f, 1.0f);
    }

    private void GenerateGridInScene()
    {
        if (parentGrid == null)
        {
            Debug.LogError("Parent Grid is not assigned.");
            return;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("Cell Prefab is not assigned.");
            return;
        }

        if (cellFreePrefab == null)
        {
            Debug.LogError("Cell Free Prefab is not assigned.");
            return;
        }

        // Удаление предыдущих ячеек
        foreach (Transform child in parentGrid)
        {
            DestroyImmediate(child.gameObject);
        }

        GridLayoutGroup gridLayout = parentGrid.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            Debug.LogError("Parent Grid must have a GridLayoutGroup component.");
            return;
        }

        // Устанавливаем фиксированный размер ячеек
        gridLayout.cellSize = new Vector2(100, 100);

        // Устанавливаем количество строк и столбцов
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridWidth;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int assetIndex = gridData[x, y] - 1;

                if (assetIndex >= 0 && assetIndex < spriteDataAssets.Length)
                {
                    // Создаем ячейку
                    GameObject cell = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, parentGrid);

                    // Назначаем спрайт
                    Image innerImage = cell.transform.GetChild(0).GetComponent<Image>();
                    if (innerImage != null && spriteDataAssets[assetIndex].spriteEntries.Length > 0)
                    {
                        innerImage.sprite = spriteDataAssets[assetIndex].spriteEntries[0].spritePrefab;
                    }

                    // Назначаем данные
                    CellDataBinder cellDataBinder = cell.GetComponent<CellDataBinder>();
                    if (cellDataBinder != null)
                    {
                        cellDataBinder.assignedData = spriteDataAssets[assetIndex];
                    }
                }
                else
                {
                    // Создаем пустую ячейку
                    GameObject freeCell = (GameObject)PrefabUtility.InstantiatePrefab(cellFreePrefab, parentGrid);
                }
            }
        }

        Debug.Log("Grid generated in the scene.");
    }
}