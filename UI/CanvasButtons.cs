using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class CanvasButtons : MonoBehaviour
{
    [Header ("Panels")]
    public GameObject helpPanel;
    public GameObject settingsPanel;
    public GameObject menuPanel;
    public GameObject leadersPanel;
    public GameObject defeatPanel;
    public GameObject winPanel;
    
    [Header("Canvases")]
    public GameObject startCanvas;
    public GameObject gameCanvas;
    
    [Header("Buttons")]
    public GameObject skipButton;   
    public GameObject restartButton;

    [Header ("Others")]
    public Transform spawnPoint;
    public Text counterText;

    private int currentLevelIndex;
    private GameObject currentPrefabInstance; // Текущий экземпляр префаба
    private int AdID = 1;

    public delegate void DeactivationAction();
    public static event DeactivationAction OnDectevated;
    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
    }

    public void Rewarded(int id)
    {
        if (id != AdID) return; // Игнорируем события с другим ID

        OnExitButtonClick();
        skipButton.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
    }
    private void Awake()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");
        }
        else
        {
            currentLevelIndex = 1; // Устанавливаем уровень по умолчанию
            PlayerPrefs.SetInt("Level", currentLevelIndex);
            PlayerPrefs.Save();
        }

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    private void Start() => OnCloseButtonClick();

    public void OnCloseButtonClick()
    {
        if (helpPanel != null) helpPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(false);
        if (leadersPanel != null) leadersPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }
    private void UpdateUI()
    {
        counterText.text = currentLevelIndex.ToString();
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        restartButton.gameObject.SetActive(false);
    }

    public void ShowDefeatPanel() => defeatPanel.SetActive(true);

    public void OnHelpButtonClick() => helpPanel.SetActive(true);

    public void OnSettingsButtonClick() => settingsPanel.SetActive(true);

    public void OnLeadersButtonClick() => leadersPanel.SetActive(true);

    public void OnStartButtonClick()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");
        }

        gameCanvas.SetActive(true);
        skipButton.gameObject.SetActive(true);
        startCanvas.SetActive(false);
        UpdateUI();

        InstantiatePrefabForCurrentLevel();
    }

    public void OnGoToMenuButton()
    {
        DeactivateCurrentLevelPrefab();

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void OnExitButtonClick()
    {
        YandexGame.FullscreenShow();
        PlayerPrefs.SetInt("Level", currentLevelIndex + 1);

        YandexGame.NewLeaderboardScores("Levels", currentLevelIndex);
        PlayerPrefs.Save();

        DeactivateCurrentLevelPrefab();

        winPanel.SetActive(false);
        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void OnPauseMenuClick() => menuPanel.SetActive(true);

    public void RestartCurrentScene()
    {
        UpdateUI();
        winPanel.SetActive(false);
        YandexGame.FullscreenShow();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void RestartCurrentLevel()
    {
        UpdateUI();
        YandexGame.FullscreenShow();
        DeactivateCurrentLevelPrefab();
        InstantiatePrefabForCurrentLevel();
    }
    private void InstantiatePrefabForCurrentLevel()
    {
        GameObject prefab;

        if (currentLevelIndex >= 90) // Если уровень 90 или выше
        {
            // Загружаем все префабы из папки Resources/Prefabs
            GameObject[] allPrefabs = Resources.LoadAll<GameObject>("Prefabs");
            if (allPrefabs.Length > 0)
            {
                // Выбираем случайный префаб
                prefab = allPrefabs[Random.Range(0, allPrefabs.Length)];
            }
            else
            {
                Debug.LogWarning("В папке Resources/Prefabs нет префабов!");
                return;
            }
        }
        else
        {
            // Формируем путь к префабу текущего уровня
            string prefabName = $"Prefabs/Grid_{currentLevelIndex}";
            prefab = Resources.Load<GameObject>(prefabName);
            if (prefab == null)
            {
                Debug.LogWarning($"Префаб {prefabName} не найден!");
                return;
            }
        }

        // Инстанцируем префаб на сцене
        if (prefab != null && spawnPoint != null)
        {
            currentPrefabInstance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            currentPrefabInstance.name = prefab.name;
        }
    }


    private void DeactivateCurrentLevelPrefab()
    {
        if (currentPrefabInstance != null)
        {
            Destroy(currentPrefabInstance);
            currentPrefabInstance = null;
        }

        OnDectevated?.Invoke();
    }
}
