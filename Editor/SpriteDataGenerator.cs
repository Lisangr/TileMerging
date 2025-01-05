using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteDataGenerator : MonoBehaviour
{
    [MenuItem("Tools/Create SpriteData ScriptableObjects")]
    public static void CreateSpriteDataObjects()
    {
        // Путь к папке Resources/icons
        string iconsPath = "Assets/Resources/icons";
        string scriptableObjectPath = "Assets/Scripts/Variants";

        // Проверяем существование папки ScriptableObjects
        if (!Directory.Exists(scriptableObjectPath))
        {
            Directory.CreateDirectory(scriptableObjectPath);
        }

        // Загружаем все спрайты из папки icons
        string[] spriteFiles = Directory.GetFiles(iconsPath, "*.png", SearchOption.AllDirectories);

        foreach (string spriteFile in spriteFiles)
        {
            // Исправляем путь
            string relativePath = spriteFile.Replace("\\", "/");

            // Проверяем, что путь начинается с "Assets"
            if (!relativePath.StartsWith("Assets"))
            {
                relativePath = relativePath.Substring(relativePath.IndexOf("Assets"));
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);

            if (sprite != null)
            {
                // Создаём новый ScriptableObject
                SpriteDataAsset spriteData = ScriptableObject.CreateInstance<SpriteDataAsset>();

                // Настраиваем ScriptableObject
                spriteData.spriteEntries = new SpriteDataAsset.SpriteEntry[]
                {
                    new SpriteDataAsset.SpriteEntry { spriteName = sprite.name, spritePrefab = sprite }
                };

                // Сохраняем ScriptableObject в папке ScriptableObjects
                string assetPath = $"{scriptableObjectPath}/{sprite.name}.asset";
                AssetDatabase.CreateAsset(spriteData, assetPath);

                Debug.Log($"Создан ScriptableObject: {assetPath}");
            }
            else
            {
                Debug.LogWarning($"Не удалось загрузить спрайт из пути: {relativePath}");
            }
        }

        // Сохраняем изменения в проекте
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Генерация ScriptableObjects завершена.");
    }
}