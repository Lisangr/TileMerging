using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField] private Toggle m_SoundToggle;
    [SerializeField] private Slider m_VolumeSlider;
    [SerializeField] private AudioSource m_AudioSource;
    private float m_Volume = 1f;
    private bool isMuted = false;

    void Start()
    {
        // Если объекты не привязаны через инспектор, ищем их в иерархии, включая неактивные
        if (m_SoundToggle == null)
        {
            GameObject toggleObject = FindInActiveObjectByName("VolumeToggle");
            if (toggleObject != null)
                m_SoundToggle = toggleObject.GetComponent<Toggle>();
        }

        if (m_VolumeSlider == null)
        {
            GameObject sliderObject = FindInActiveObjectByName("VolumeSlider");
            if (sliderObject != null)
                m_VolumeSlider = sliderObject.GetComponent<Slider>();
        }

        // Получаем компонент AudioSource
        m_AudioSource = GetComponent<AudioSource>();

        // Загружаем громкость и состояние звука из PlayerPrefs
        m_Volume = PlayerPrefs.HasKey("Volume") ? PlayerPrefs.GetFloat("Volume") : .5f;
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        // Применяем громкость в AudioSource в зависимости от состояния звука
        m_AudioSource.volume = isMuted ? 0f : m_Volume;

        // Инициализация состояния тумблера
        if (m_SoundToggle != null)
        {
            m_SoundToggle.isOn = !isMuted;  // Если звук выключен, тумблер в положении "off"
            m_SoundToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        // Инициализация значения слайдера
        if (m_VolumeSlider != null)
        {
            m_VolumeSlider.value = m_Volume;  // Устанавливаем значение слайдера в сохраненное значение
            m_VolumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void Update()
    {
        // Сохраняем громкость и состояние звука
        PlayerPrefs.SetFloat("Volume", m_Volume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Метод вызывается при изменении громкости слайдером
    public void OnSliderChanged(float volume)
    {
        m_Volume = volume;  // Обновляем внутреннюю переменную громкости
        if (!isMuted)
        {
            m_AudioSource.volume = m_Volume;  // Применяем громкость, если звук включен
        }
    }

    // Метод вызывается при изменении состояния тумблера
    private void OnToggleChanged(bool isOn)
    {
        isMuted = !isOn;
        if (isMuted)
        {
            m_AudioSource.volume = 0f;  // Если звук выключен, устанавливаем громкость в 0
        }
        else
        {
            m_AudioSource.volume = m_Volume;  // Если звук включен, применяем сохраненную громкость
        }
    }

    // Рекурсивный метод для поиска объекта по имени, включая неактивные
    private GameObject FindInActiveObjectByName(string name)
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in transforms)
        {
            if (t.hideFlags == HideFlags.None && t.name == name)
                return t.gameObject;
        }
        return null;
    }
}
