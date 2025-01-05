using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public List<ObjectData> objects = new List<ObjectData>();
}

[System.Serializable]
public class ObjectData
{
    public string prefabName; // Имя префаба
    public Vector3 position;  // Позиция объекта
    public Vector3 rotation;  // Вращение объекта
}

public class LevelManager : MonoBehaviour
{
    public Transform parentContainer; // Контейнер для объектов уровня

    // Метод сохранения уровня
    [ContextMenu("Save Level to JSON")]
    public void SaveLevelToJSON()
    {
        if (parentContainer == null)
        {
            parentContainer = transform;
        }

        // Используем полное имя объекта, включая "(Clone)"
        string saveFileName = $"{gameObject.name}.json";

        LevelData levelData = new LevelData();

        foreach (Transform child in parentContainer)
        {
            ObjectData objectData = new ObjectData
            {
                prefabName = child.name, // Используем имя с "(Clone)"
                position = child.position,
                rotation = child.eulerAngles
            };
            levelData.objects.Add(objectData);
        }

        string json = JsonUtility.ToJson(levelData, true);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);

        File.WriteAllText(path, json);
        Debug.Log($"Уровень сохранён в файл: {path}");
    }

    // Метод восстановления уровня
    [ContextMenu("Load Level from JSON")]
    public void LoadLevelFromJSON()
    {
        if (parentContainer == null)
        {
            parentContainer = transform;
        }

        // Используем полное имя объекта, включая "(Clone)"
        string saveFileName = $"{gameObject.name}.json";

        // Загружаем JSON-файл из папки Resources/Boxes
        TextAsset jsonFile = Resources.Load<TextAsset>($"Boxes/{Path.GetFileNameWithoutExtension(saveFileName)}");

        if (jsonFile == null)
        {
            Debug.LogError($"Файл {saveFileName} не найден в Resources/Boxes!");
            return;
        }

        string json = jsonFile.text;
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        // Удаляем текущие объекты
        foreach (Transform child in parentContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        // Восстанавливаем объекты
        foreach (var objectData in levelData.objects)
        {
            // Загружаем префаб из папки Resources
            GameObject prefab = Resources.Load<GameObject>(objectData.prefabName);

            if (prefab == null)
            {
                Debug.LogError($"Префаб {objectData.prefabName} не найден в Resources!");
                continue;
            }

            GameObject newObject = Instantiate(prefab, objectData.position, Quaternion.Euler(objectData.rotation), parentContainer);
            newObject.name = objectData.prefabName; // Устанавливаем оригинальное имя
        }

        Debug.Log($"Уровень загружен из файла: {saveFileName}");
    }
}