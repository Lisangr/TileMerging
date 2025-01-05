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
    public string prefabName; // ��� �������
    public Vector3 position;  // ������� �������
    public Vector3 rotation;  // �������� �������
}

public class LevelManager : MonoBehaviour
{
    public Transform parentContainer; // ��������� ��� �������� ������

    // ����� ���������� ������
    [ContextMenu("Save Level to JSON")]
    public void SaveLevelToJSON()
    {
        if (parentContainer == null)
        {
            parentContainer = transform;
        }

        // ���������� ������ ��� �������, ������� "(Clone)"
        string saveFileName = $"{gameObject.name}.json";

        LevelData levelData = new LevelData();

        foreach (Transform child in parentContainer)
        {
            ObjectData objectData = new ObjectData
            {
                prefabName = child.name, // ���������� ��� � "(Clone)"
                position = child.position,
                rotation = child.eulerAngles
            };
            levelData.objects.Add(objectData);
        }

        string json = JsonUtility.ToJson(levelData, true);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);

        File.WriteAllText(path, json);
        Debug.Log($"������� ������� � ����: {path}");
    }

    // ����� �������������� ������
    [ContextMenu("Load Level from JSON")]
    public void LoadLevelFromJSON()
    {
        if (parentContainer == null)
        {
            parentContainer = transform;
        }

        // ���������� ������ ��� �������, ������� "(Clone)"
        string saveFileName = $"{gameObject.name}.json";

        // ��������� JSON-���� �� ����� Resources/Boxes
        TextAsset jsonFile = Resources.Load<TextAsset>($"Boxes/{Path.GetFileNameWithoutExtension(saveFileName)}");

        if (jsonFile == null)
        {
            Debug.LogError($"���� {saveFileName} �� ������ � Resources/Boxes!");
            return;
        }

        string json = jsonFile.text;
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        // ������� ������� �������
        foreach (Transform child in parentContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        // ��������������� �������
        foreach (var objectData in levelData.objects)
        {
            // ��������� ������ �� ����� Resources
            GameObject prefab = Resources.Load<GameObject>(objectData.prefabName);

            if (prefab == null)
            {
                Debug.LogError($"������ {objectData.prefabName} �� ������ � Resources!");
                continue;
            }

            GameObject newObject = Instantiate(prefab, objectData.position, Quaternion.Euler(objectData.rotation), parentContainer);
            newObject.name = objectData.prefabName; // ������������� ������������ ���
        }

        Debug.Log($"������� �������� �� �����: {saveFileName}");
    }
}