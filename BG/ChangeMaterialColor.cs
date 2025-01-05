using UnityEngine;

public class ChangeMaterialGradient : MonoBehaviour
{
    public Material targetMaterial;
    private MaterialPropertyBlock propertyBlock;

    private Color startColor1, startColor2;
    private Color endColor1, endColor2;
    public float transitionDuration = 2.0f;
    private float transitionProgress = 0.0f;
    private bool isTransitioning = false;

    void Start()
    {
        if (targetMaterial == null)
        {
            Debug.LogError("Материал не назначен. Укажите материал в инспекторе.");
            return;
        }

        propertyBlock = new MaterialPropertyBlock();
        Renderer renderer = GetComponent<Renderer>();

        RandomStartColor(renderer);

        StartGradientTransition();
    }

    private void RandomStartColor(Renderer renderer)
    {
        // Генерация случайных начальных цветов
        startColor1 = GenerateRandomColorWithRange(0f, 1f);
        startColor2 = GenerateRandomColorWithRange(0f, 1f);

        // Применяем начальные цвета к материалу
        propertyBlock.SetColor("_Color1", startColor1);
        propertyBlock.SetColor("_Color2", startColor2);
        renderer.SetPropertyBlock(propertyBlock);
    }

    void Update()
    {
        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            transitionProgress = Mathf.Clamp01(transitionProgress);

            Color currentColor1 = Color.Lerp(startColor1, endColor1, transitionProgress);
            Color currentColor2 = Color.Lerp(startColor2, endColor2, transitionProgress);

            Renderer renderer = GetComponent<Renderer>();
            propertyBlock.SetColor("_Color1", currentColor1);
            propertyBlock.SetColor("_Color2", currentColor2);
            renderer.SetPropertyBlock(propertyBlock);

            if (transitionProgress >= 1.0f)
            {
                isTransitioning = false;
            }
        }
    }

    public void StartGradientTransition()
    {
        if (targetMaterial == null) return;

        Renderer renderer = GetComponent<Renderer>();
        RandomStartColor(renderer);

        // Генерация новых случайных целевых цветов
        endColor1 = GenerateRandomColorWithRange(0f, 1f);
        endColor2 = GenerateRandomColorWithRange(0f, 1f);

        transitionProgress = 0.0f;
        isTransitioning = true;

        Debug.Log($"Начат переход градиента: StartColor1={startColor1}, StartColor2={startColor2}, EndColor1={endColor1}, EndColor2={endColor2}");
    }

    private Color GenerateRandomColorWithRange(float min, float max)
    {
        float r = Random.Range(min, max);
        float g = Random.Range(min, max);
        float b = Random.Range(min, max);
        return new Color(r, g, b);
    }
}
