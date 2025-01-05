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
        // ���� ������� �� ��������� ����� ���������, ���� �� � ��������, ������� ����������
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

        // �������� ��������� AudioSource
        m_AudioSource = GetComponent<AudioSource>();

        // ��������� ��������� � ��������� ����� �� PlayerPrefs
        m_Volume = PlayerPrefs.HasKey("Volume") ? PlayerPrefs.GetFloat("Volume") : .5f;
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        // ��������� ��������� � AudioSource � ����������� �� ��������� �����
        m_AudioSource.volume = isMuted ? 0f : m_Volume;

        // ������������� ��������� ��������
        if (m_SoundToggle != null)
        {
            m_SoundToggle.isOn = !isMuted;  // ���� ���� ��������, ������� � ��������� "off"
            m_SoundToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        // ������������� �������� ��������
        if (m_VolumeSlider != null)
        {
            m_VolumeSlider.value = m_Volume;  // ������������� �������� �������� � ����������� ��������
            m_VolumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void Update()
    {
        // ��������� ��������� � ��������� �����
        PlayerPrefs.SetFloat("Volume", m_Volume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ����� ���������� ��� ��������� ��������� ���������
    public void OnSliderChanged(float volume)
    {
        m_Volume = volume;  // ��������� ���������� ���������� ���������
        if (!isMuted)
        {
            m_AudioSource.volume = m_Volume;  // ��������� ���������, ���� ���� �������
        }
    }

    // ����� ���������� ��� ��������� ��������� ��������
    private void OnToggleChanged(bool isOn)
    {
        isMuted = !isOn;
        if (isMuted)
        {
            m_AudioSource.volume = 0f;  // ���� ���� ��������, ������������� ��������� � 0
        }
        else
        {
            m_AudioSource.volume = m_Volume;  // ���� ���� �������, ��������� ����������� ���������
        }
    }

    // ����������� ����� ��� ������ ������� �� �����, ������� ����������
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
