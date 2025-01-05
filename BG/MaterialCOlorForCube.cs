using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCOlorForCube : MonoBehaviour
{
    // Указываем материал, цвет которого нужно изменить
    public Material targetMaterial;

    // Новый цвет для Albedo
    private Color newColor;

    void Start()
    {
        // Проверяем, назначен ли материал
        if (targetMaterial != null)
        {
            // Устанавливаем начальный цвет
            ChangeMaterialToRandomColor();
        }
        else
        {
            Debug.LogError("Материал не назначен. Пожалуйста, укажите материал в инспекторе.");
        }
    }

    // Метод для генерации случайного цвета
    private Color GenerateRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    // Метод для изменения цвета материала
    public void ChangeMaterialToRandomColor()
    {
        if (targetMaterial != null)
        {
            // Генерируем случайный цвет
            newColor = GenerateRandomColor();

            // Изменяем цвет Albedo на случайный цвет
            targetMaterial.color = newColor;

            // Убедимся, что Rendering Mode установлен в Opaque
            if (targetMaterial.HasProperty("_Mode"))
            {
                targetMaterial.SetFloat("_Mode", 0); // 0 = Opaque
            }

            // Обновляем шейдер для материала
            targetMaterial.EnableKeyword("_ALPHATEST_ON");
            targetMaterial.DisableKeyword("_ALPHABLEND_ON");
            targetMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            targetMaterial.renderQueue = -1;

            Debug.Log($"Цвет материала успешно изменён на: {newColor}");
        }
        else
        {
            Debug.LogError("Материал не назначен. Пожалуйста, укажите материал в инспекторе.");
        }
    }
}
