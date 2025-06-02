using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴을 위한 정적 인스턴스
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool isGameOver = false;   // 게임 오버 상태 여부
    public bool isPaused = false;     // 게임 일시정지 상태 여부

    [Header("Score")]
    public float survivalTime = 0f;   // 현재 생존 시간
    public float bestTime = 0f;       // 저장된 최고 생존 시간

    [Header("UI References")]
    public TextMeshProUGUI timeText;        // 생존 시간 표시 UI
    public TextMeshProUGUI bestTimeText;    // 최고 기록 표시 UI
    public GameObject gameOverPanel;        // 게임 오버 UI 패널
    public TextMeshProUGUI finalScoreText;  // 최종 점수 표시 텍스트

    [Header("Difficulty Settings")]
    public float difficultyIncreaseInterval = 10f; // 난이도 상승 간격 (초)
    public float speedIncreaseAmount = 0.5f;       // 자동차 속도 증가량
    public float spawnIntervalDecreaseAmount = 0.1f; // 자동차 생성 간격 감소량

    private CarSpawner carSpawner;       // CarSpawner 컴포넌트 참조
    private float nextDifficultyIncrease; // 다음 난이도 증가 시간

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 중복 방지
            return;
        }
    }

    private void Start()
    {
        // CarSpawner 컴포넌트 찾기
        carSpawner = FindObjectOfType<CarSpawner>();

        // 게임오버 UI 패널 초기 상태 비활성화
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 저장된 최고 기록 불러오기
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        UpdateBestTimeUI();

        // 난이도 증가 시점 초기화
        nextDifficultyIncrease = difficultyIncreaseInterval;
    }

    private void Update()
    {
        // 게임이 오버되거나 일시정지 상태가 아닐 때만 실행
        if (!isGameOver && !isPaused)
        {
            // 생존 시간 증가
            survivalTime += Time.deltaTime;
            UpdateTimeUI();

            // 정해진 시간마다 난이도 증가
            if (survivalTime >= nextDifficultyIncrease)
            {
                IncreaseDifficulty();
                nextDifficultyIncrease += difficultyIncreaseInterval;
            }

            // ESC 키로 일시정지/해제 토글
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }

    // 생존 시간 UI 업데이트
    private void UpdateTimeUI()
    {
        if (timeText != null)
            timeText.text = $"Time: {survivalTime:F1}";
    }

    // 최고 시간 UI 업데이트
    private void UpdateBestTimeUI()
    {
        if (bestTimeText != null)
            bestTimeText.text = $"Best: {bestTime:F1}";
    }

    // 난이도 증가 처리
    private void IncreaseDifficulty()
    {
        if (carSpawner != null)
        {
            // 자동차 속도 증가
            carSpawner.carSpeed += speedIncreaseAmount;

            // 자동차 생성 간격 감소 (최솟값 제한)
            carSpawner.minSpawnInterval = Mathf.Max(0.5f, carSpawner.minSpawnInterval - spawnIntervalDecreaseAmount);
            carSpawner.maxSpawnInterval = Mathf.Max(1f, carSpawner.maxSpawnInterval - spawnIntervalDecreaseAmount);

            // 디버그 로그로 현재 상태 출력
            Debug.Log($"<color=yellow>난이도 증가! 현재 속도: {carSpawner.carSpeed}, 생성 간격: {carSpawner.minSpawnInterval}-{carSpawner.maxSpawnInterval}</color>");
        }
    }

    // 게임 오버 처리
    public void GameOver()
    {
        if (isGameOver) return; // 중복 실행 방지

        isGameOver = true;
        Debug.Log("<color=red>게임 오버!</color>");

        // 최고 기록 갱신 여부 확인 및 저장
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

    // 재시작 버튼 클릭 시 호출
    public void OnRestartButtonClicked()
    {
        Debug.Log("재시작 버튼 클릭됨!"); // 디버그용 로그
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드
    }

    // 일시정지 상태 토글
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f; // 시간 정지 또는 재개
    }
}