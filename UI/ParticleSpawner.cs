using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [Header("Particle System Prefab")]
    public GameObject particlePrefab; // Префаб системы частиц

    void Update()
    {
        // Проверяем, был ли клик левой кнопкой мыши
        if (Input.GetMouseButtonDown(0))
        {
            SpawnParticleAtMousePosition();
        }
    }

    private void SpawnParticleAtMousePosition()
    {
        // Определяем позицию мыши в мировых координатах
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane; // Устанавливаем Z для корректного преобразования
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Создаём экземпляр системы частиц
        if (particlePrefab != null)
        {
            GameObject particleInstance = Instantiate(particlePrefab, worldPosition, Quaternion.identity);
            ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();

            if (particleSystem != null)
            {
                // Запускаем корутину для удаления системы частиц
                StartCoroutine(DestroyParticleAfterPlay(particleSystem));
            }
        }
        else
        {
            Debug.LogWarning("Префаб системы частиц не назначен!");
        }
    }

    private System.Collections.IEnumerator DestroyParticleAfterPlay(ParticleSystem particleSystem)
    {
        // Ждём, пока система частиц завершит проигрывание
        while (particleSystem.isPlaying)
        {
            yield return null;
        }

        // Удаляем объект
        Destroy(particleSystem.gameObject);
    }
}
