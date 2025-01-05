using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [Header("Particle System Prefab")]
    public GameObject particlePrefab; // ������ ������� ������

    void Update()
    {
        // ���������, ��� �� ���� ����� ������� ����
        if (Input.GetMouseButtonDown(0))
        {
            SpawnParticleAtMousePosition();
        }
    }

    private void SpawnParticleAtMousePosition()
    {
        // ���������� ������� ���� � ������� �����������
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane; // ������������� Z ��� ����������� ��������������
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // ������ ��������� ������� ������
        if (particlePrefab != null)
        {
            GameObject particleInstance = Instantiate(particlePrefab, worldPosition, Quaternion.identity);
            ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();

            if (particleSystem != null)
            {
                // ��������� �������� ��� �������� ������� ������
                StartCoroutine(DestroyParticleAfterPlay(particleSystem));
            }
        }
        else
        {
            Debug.LogWarning("������ ������� ������ �� ��������!");
        }
    }

    private System.Collections.IEnumerator DestroyParticleAfterPlay(ParticleSystem particleSystem)
    {
        // ���, ���� ������� ������ �������� ������������
        while (particleSystem.isPlaying)
        {
            yield return null;
        }

        // ������� ������
        Destroy(particleSystem.gameObject);
    }
}
