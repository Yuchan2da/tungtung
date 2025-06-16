using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool isGameOver = false;
    public bool isPaused = false;

    [Header("Score")]
    public float survivalTime = 0f;
    public float bestTime = 0f;

    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI bestTimeText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Difficulty Settings")]
    public float difficultyIncreaseInterval = 10f;
    public float speedIncreaseAmount = 0.5f;
    public float spawnIntervalDecreaseAmount = 0.1f;

    private CarSpawner carSpawner;
    private float nextDifficultyIncrease;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip[] bgmClips; // 🎵 3개 이상의 BGM을 등록할 배열
    public AudioClip clickSfx;
    public AudioClip hitSfx;

    private int currentBgmIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        carSpawner = Object.FindFirstObjectByType <CarSpawner>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        UpdateBestTimeUI();

        nextDifficultyIncrease = difficultyIncreaseInterval;

        PlayNextBgm(); // 🎵 첫 번째 BGM 재생 시작
    }

    private void Update()
    {
        if (!isGameOver && !isPaused)
        {
            survivalTime += Time.deltaTime;
            UpdateTimeUI();

            if (survivalTime >= nextDifficultyIncrease)
            {
                IncreaseDifficulty();
                nextDifficultyIncrease += difficultyIncreaseInterval;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            // 🎵 BGM이 끝났으면 다음 곡 재생
            if (!bgmSource.isPlaying && bgmClips.Length > 0)
            {
                PlayNextBgm();
            }
        }
    }

    private void PlayNextBgm()
    {
        if (bgmClips.Length == 0 || bgmSource == null) return;

        bgmSource.clip = bgmClips[currentBgmIndex];
        bgmSource.loop = false;
        bgmSource.Play();

        currentBgmIndex = (currentBgmIndex + 1) % bgmClips.Length; // 다음 인덱스로
    }

    private void UpdateTimeUI()
    {
        if (timeText != null)
            timeText.text = $"Time: {survivalTime:F1}";
    }

    private void UpdateBestTimeUI()
    {
        if (bestTimeText != null)
            bestTimeText.text = $"Best: {bestTime:F1}";
    }

    private void IncreaseDifficulty()
    {
        if (carSpawner != null)
        {
            carSpawner.carSpeed += speedIncreaseAmount;
            carSpawner.minSpawnInterval = Mathf.Max(0.5f, carSpawner.minSpawnInterval - spawnIntervalDecreaseAmount);
            carSpawner.maxSpawnInterval = Mathf.Max(1f, carSpawner.maxSpawnInterval - spawnIntervalDecreaseAmount);

            Debug.Log($"<color=yellow>난이도 증가! 속도: {carSpawner.carSpeed}, 생성 간격: {carSpawner.minSpawnInterval}-{carSpawner.maxSpawnInterval}</color>");
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (survivalTime > bestTime)
        {
            bestTime = survivalTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();
            UpdateBestTimeUI();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = $"Final Time: {survivalTime:F1}";
        }
    }

    public void OnRestartButtonClicked()
    {
        PlaySFX(clickSfx);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}
