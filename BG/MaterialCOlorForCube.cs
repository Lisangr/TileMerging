using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCOlorForCube : MonoBehaviour
{
    // ��������� ��������, ���� �������� ����� ��������
    public Material targetMaterial;

    // ����� ���� ��� Albedo
    private Color newColor;

    void Start()
    {
        // ���������, �������� �� ��������
        if (targetMaterial != null)
        {
            // ������������� ��������� ����
            ChangeMaterialToRandomColor();
        }
        else
        {
            Debug.LogError("�������� �� ��������. ����������, ������� �������� � ����������.");
        }
    }

    // ����� ��� ��������� ���������� �����
    private Color GenerateRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    // ����� ��� ��������� ����� ���������
    public void ChangeMaterialToRandomColor()
    {
        if (targetMaterial != null)
        {
            // ���������� ��������� ����
            newColor = GenerateRandomColor();

            // �������� ���� Albedo �� ��������� ����
            targetMaterial.color = newColor;

            // ��������, ��� Rendering Mode ���������� � Opaque
            if (targetMaterial.HasProperty("_Mode"))
            {
                targetMaterial.SetFloat("_Mode", 0); // 0 = Opaque
            }

            // ��������� ������ ��� ���������
            targetMaterial.EnableKeyword("_ALPHATEST_ON");
            targetMaterial.DisableKeyword("_ALPHABLEND_ON");
            targetMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            targetMaterial.renderQueue = -1;

            Debug.Log($"���� ��������� ������� ������ ��: {newColor}");
        }
        else
        {
            Debug.LogError("�������� �� ��������. ����������, ������� �������� � ����������.");
        }
    }
}
