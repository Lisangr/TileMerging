// Путь: Assets/Editor/RenamePrefabsTool.cs
using UnityEngine;
using UnityEditor;
using System.IO;

public class RenamePrefabsTool : EditorWindow
{
    private string folderPath = "Assets/Resources/Prefabs";
    private string renameFormat = "Grid_{0}";

    [MenuItem("Tools/Rename Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<RenamePrefabsTool>("Rename Prefabs");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Renamer Tool", EditorStyles.boldLabel);

        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
        renameFormat = EditorGUILayout.TextField("Rename Format", renameFormat);

        if (GUILayout.Button("Rename Prefabs"))
        {
            RenamePrefabs();
        }
    }

    private void RenamePrefabs()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"Directory not found: {folderPath}");
            return;
        }

        string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

        int counter = 1;
        foreach (string path in prefabPaths)
        {
            string assetPath = path.Replace("\\", "/");
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                string newName = string.Format(renameFormat, counter);
                string newAssetPath = Path.Combine(Path.GetDirectoryName(assetPath), newName + ".prefab").Replace("\\", "/");

                AssetDatabase.RenameAsset(assetPath, newName);
                Debug.Log($"Renamed: {assetPath} -> {newAssetPath}");

                counter++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Prefab renaming completed.");
    }
}
