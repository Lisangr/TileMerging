using UnityEngine;

[CreateAssetMenu(fileName = "SpriteDataAsset", menuName = "UI/Sprite Data", order = 1)]
public class SpriteDataAsset : ScriptableObject
{
    [System.Serializable]
    public class SpriteEntry
    {
        public string spriteName;       // ��� �������
        public Sprite spritePrefab;     // ������, ���� ��������� �����������
    }

    public SpriteEntry[] spriteEntries;

    /// <summary>
    /// ����� ������ �� �����.
    /// </summary>
    /// <param name="name">��� �������</param>
    /// <returns>Prefab ������� ��� null, ���� �� ������</returns>
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
