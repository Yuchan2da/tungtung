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

    private void Awake()
    {
        // 싱글톤 패턴 구현
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
        // 컴포넌트 찾기
        carSpawner = FindObjectOfType<CarSpawner>();
        
        // UI 초기화
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        // 최고 기록 불러오기
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        UpdateBestTimeUI();
        
        // 난이도 증가 타이머 초기화
        nextDifficultyIncrease = difficultyIncreaseInterval;
    }

    private void Update()
    {
        if (!isGameOver && !isPaused)
        {
            // 생존 시간 업데이트
            survivalTime += Time.deltaTime;
            UpdateTimeUI();

            // 난이도 증가 체크
            if (survivalTime >= nextDifficultyIncrease)
            {
                IncreaseDifficulty();
                nextDifficultyIncrease += difficultyIncreaseInterval;
            }

            // 일시정지 체크
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
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
            // 자동차 속도 증가
            carSpawner.carSpeed += speedIncreaseAmount;
            
            // 생성 간격 감소 (최소값 제한)
            carSpawner.minSpawnInterval = Mathf.Max(0.5f, carSpawner.minSpawnInterval - spawnIntervalDecreaseAmount);
            carSpawner.maxSpawnInterval = Mathf.Max(1f, carSpawner.maxSpawnInterval - spawnIntervalDecreaseAmount);
            
            Debug.Log($"<color=yellow>난이도 증가! 현재 속도: {carSpawner.carSpeed}, 생성 간격: {carSpawner.minSpawnInterval}-{carSpawner.maxSpawnInterval}</color>");
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("<color=red>게임 오버!</color>");

        // 최고 기록 갱신 체크
        if (survivalTime > bestTime)
        {
            bestTime = survivalTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();
            UpdateBestTimeUI();
        }

        // 게임오버 UI 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = $"Final Time: {survivalTime:F1}";
        }
    }

    public void OnRestartButtonClicked()
    {
        Debug.Log("재시작 버튼 클릭됨!"); // 로그 확인용
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
}