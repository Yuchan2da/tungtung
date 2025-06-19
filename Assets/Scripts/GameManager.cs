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

    [Header("Settings UI")]
    public GameObject settingsPanel;
    public Slider carSpeedSlider;
    public Slider playerSpeedSlider;
    public Slider volumeSlider;

    [Header("Difficulty Settings")]
    public float difficultyIncreaseInterval = 10f;
    public float speedIncreaseAmount = 0.5f;
    public float spawnIntervalDecreaseAmount = 0.1f;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip[] bgmClips;
    public AudioClip clickSfx;
    public AudioClip hitSfx;

    private int currentBgmIndex = 0;

    private CarSpawner carSpawner;
    private Player player;
    private float nextDifficultyIncrease;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        carSpawner = Object.FindFirstObjectByType<CarSpawner>();
        player = Object.FindFirstObjectByType<Player>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        UpdateBestTimeUI();

        nextDifficultyIncrease = difficultyIncreaseInterval;

        PlayNextBgm();

        if (carSpeedSlider != null && carSpawner != null)
            carSpeedSlider.value = carSpawner.carSpeed;

        if (playerSpeedSlider != null && player != null)
            playerSpeedSlider.value = player.moveSpeed;

        if (volumeSlider != null && bgmSource != null)
            volumeSlider.value = bgmSource.volume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (!isGameOver && !isPaused)
        {
            survivalTime += Time.deltaTime;
            UpdateTimeUI();

            if (survivalTime >= nextDifficultyIncrease)
            {
                IncreaseDifficulty();
                nextDifficultyIncrease += difficultyIncreaseInterval;
            }

            if (!bgmSource.isPlaying && bgmClips.Length > 0)
                PlayNextBgm();
        }
    }

    private void PlayNextBgm()
    {
        if (bgmClips.Length == 0 || bgmSource == null) return;

        bgmSource.clip = bgmClips[currentBgmIndex];
        bgmSource.loop = false;
        bgmSource.Play();

        currentBgmIndex = (currentBgmIndex + 1) % bgmClips.Length;
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
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 1f;

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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (settingsPanel != null)
            settingsPanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnCarSpeedChanged(float value)
    {
        if (carSpawner != null)
            carSpawner.carSpeed = value;
    }

    public void OnPlayerSpeedChanged(float value)
    {
        if (player != null)
            player.moveSpeed = value;
    }

    public void OnVolumeChanged(float value)
    {
        if (bgmSource != null)
            bgmSource.volume = value;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}
