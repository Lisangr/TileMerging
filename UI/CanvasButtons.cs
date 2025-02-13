using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;
using static UnityEngine.ParticleSystem;

public class CanvasButtons : MonoBehaviour
{
    [Header ("Panels")]
    public GameObject helpPanel;
    public GameObject settingsPanel;
    public GameObject menuPanel;
    public GameObject leadersPanel;
    public GameObject defeatPanel;
    public GameObject winPanel;
    public GameObject infoPanel;

    [Header("Canvases")]
    public GameObject startCanvas;
    public GameObject gameCanvas;

    [Header("Buttons")]
    public GameObject restartButton;

    [Header ("Others")]
    public Transform spawnPoint;
    public Text counterText;
    public GameObject[] particles;

    private int currentLevelIndex;
    private GameObject currentPrefabInstance; // Текущий экземпляр префаба
    private int AdID = 1;
    private int levelFromYG;

    public delegate void DeactivationAction();
    public static event DeactivationAction OnDectevated;
    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
        YandexGame.GetDataEvent += LoadFromCloud;
    }

    private void LoadFromCloud()
    {
        levelFromYG = YandexGame.savesData.level;
        Debug.Log("Загружен уровень" + levelFromYG);
    }

    public void Rewarded(int id)
    {
        if (id != AdID) return; // Игнорируем события с другим ID

        ExitAfterReward();
    }
    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
        YandexGame.GetDataEvent -= LoadFromCloud;
    }
    private void Awake()
    {
        LoadFromCloud();
        
        currentLevelIndex = PlayerPrefs.GetInt("Level", 1);
        if (currentLevelIndex >= levelFromYG)
        {
            gameCanvas.SetActive(false);
            startCanvas.SetActive(true);
            YandexGame.savesData.level = currentLevelIndex;
        }
        else
        {
            currentLevelIndex = levelFromYG;
            PlayerPrefs.SetInt("Level", currentLevelIndex);
        }
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
        if (infoPanel != null) infoPanel.SetActive(false);

        ActivateParticles();
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
    public void ShowInfoPanel() => infoPanel.SetActive(true);
    public void ShowDefeatPanel() => defeatPanel.SetActive(true);

    public void OnHelpButtonClick() => helpPanel.SetActive(true);

    public void OnSettingsButtonClick()
    {
        settingsPanel.SetActive(true);
        DeactivateParticles();
    }

    public void OnLeadersButtonClick()
    {
        leadersPanel.SetActive(true);
        DeactivateParticles();
    }
    public void OnStartButtonClick()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");
        }

        gameCanvas.SetActive(true);
        ShowInfoPanel();
        startCanvas.SetActive(false);
        UpdateUI();

        restartButton.gameObject.SetActive(true);
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
        PlayerPrefs.Save();

        if (currentLevelIndex > levelFromYG)
        {
            YandexGame.savesData.level = currentLevelIndex + 1;
            YandexGame.SaveProgress();
            YandexGame.NewLeaderboardScores("Levels", currentLevelIndex);
        }

        DeactivateCurrentLevelPrefab();

        winPanel.SetActive(false);
        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }
    private void ExitAfterReward()
    {
        PlayerPrefs.SetInt("Level", currentLevelIndex + 1);
        PlayerPrefs.Save();

        if (currentLevelIndex > levelFromYG)
        {
            YandexGame.savesData.level = currentLevelIndex + 1;
            YandexGame.SaveProgress();
            YandexGame.NewLeaderboardScores("Levels", currentLevelIndex);
        }

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
    private void DeactivateParticles()
    {
        foreach (GameObject particle in particles)
        {
            if (particle != null)
            {
                particle.SetActive(false);
            }
        }
    }
    private void ActivateParticles()
    {
        foreach (GameObject particle in particles)
        {
            if (particle != null)
            {
                particle.SetActive(true);
            }
        }
    }

}
