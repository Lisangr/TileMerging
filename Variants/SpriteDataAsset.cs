using UnityEngine;

[CreateAssetMenu(fileName = "SpriteDataAsset", menuName = "UI/Sprite Data", order = 1)]
public class SpriteDataAsset : ScriptableObject
{
    [System.Serializable]
    public class SpriteEntry
    {
        public string spriteName;       // Имя спрайта
        public Sprite spritePrefab;     // Спрайт, если требуется отображение
    }

    public SpriteEntry[] spriteEntries;

    /// <summary>
    /// Найти спрайт по имени.
    /// </summary>
    /// <param name="name">Имя спрайта</param>
    /// <returns>Prefab спрайта или null, если не найден</returns>
    public Sprite GetSpriteByName(string name)
    {
        foreach (var entry in spriteEntries)
        {
            if (entry.spriteName == name)
            {
                return entry.spritePrefab;
            }
        }
        return null;
    }
}
