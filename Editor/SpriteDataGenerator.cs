using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteDataGenerator : MonoBehaviour
{
    [MenuItem("Tools/Create SpriteData ScriptableObjects")]
    public static void CreateSpriteDataObjects()
    {
        // ���� � ����� Resources/icons
        string iconsPath = "Assets/Resources/icons";
        string scriptableObjectPath = "Assets/Scripts/Variants";

        // ��������� ������������� ����� ScriptableObjects
        if (!Directory.Exists(scriptableObjectPath))
        {
            Directory.CreateDirectory(scriptableObjectPath);
        }

        // ��������� ��� ������� �� ����� icons
        string[] spriteFiles = Directory.GetFiles(iconsPath, "*.png", SearchOption.AllDirectories);

        foreach (string spriteFile in spriteFiles)
        {
            // ���������� ����
            string relativePath = spriteFile.Replace("\\", "/");

            // ���������, ��� ���� ���������� � "Assets"
            if (!relativePath.StartsWith("Assets"))
            {
                relativePath = relativePath.Substring(relativePath.IndexOf("Assets"));
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);

            if (sprite != null)
            {
                // ������ ����� ScriptableObject
                SpriteDataAsset spriteData = ScriptableObject.CreateInstance<SpriteDataAsset>();

                // ����������� ScriptableObject
                spriteData.spriteEntries = new SpriteDataAsset.SpriteEntry[]
                {
                    new SpriteDataAsset.SpriteEntry { spriteName = sprite.name, spritePrefab = sprite }
                };

                // ��������� ScriptableObject � ����� ScriptableObjects
                string assetPath = $"{scriptableObjectPath}/{sprite.name}.asset";
                AssetDatabase.CreateAsset(spriteData, assetPath);

                Debug.Log($"������ ScriptableObject: {assetPath}");
            }
            else
            {
                Debug.LogWarning($"�� ������� ��������� ������ �� ����: {relativePath}");
            }
        }

        // ��������� ��������� � �������
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("��������� ScriptableObjects ���������.");
    }
}