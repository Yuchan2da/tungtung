using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Car Settings")]
    public GameObject[] carPrefabs; // 7개의 자동차 프리팹
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;
    public float carSpeed = 3f;

    [Header("Spawn Position")]
    public float spawnZPosition = 2f;
    public float destroyZPosition = -15f;

    [Header("Lane Settings")]
    public float minXPosition = -2.2f;
    public float maxXPosition = 2.4f;
    public int laneCount = 5;
    private float[] spawnXPositions;

    [Range(1, 3)]
    public int minCarsPerSpawn = 1;
    [Range(1, 3)]
    public int maxCarsPerSpawn = 3;

    private float nextSpawnTime;

    private void Start()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        InitializeLanePositions();
        Debug.Log("<color=yellow>자동차 스포너가 시작되었습니다!</color>");
    }

    private void InitializeLanePositions()
    {
        spawnXPositions = new float[laneCount];
        float laneWidth = (maxXPosition - minXPosition) / (laneCount - 1);

        for (int i = 0; i < laneCount; i++)
        {
            spawnXPositions[i] = minXPosition + (laneWidth * i);
            Debug.Log($"<color=yellow>차선 {i + 1} 위치: {spawnXPositions[i]}</color>");
        }
    }

    private void Update()
    {
        // 게임 일시정지 또는 게임오버 상태면 스폰 중지
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isPaused || GameManager.Instance.isGameOver)
                return;
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnCars();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnCars()
    {
        if (carPrefabs.Length == 0)
        {
            Debug.LogError("<color=red>자동차 프리팹이 설정되지 않았습니다!</color>");
            return;
        }

        int carsToSpawn = Random.Range(minCarsPerSpawn, maxCarsPerSpawn + 1);

        int[] availableLanes = new int[spawnXPositions.Length];
        for (int i = 0; i < spawnXPositions.Length; i++)
            availableLanes[i] = i;

        for (int i = availableLanes.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = availableLanes[i];
            availableLanes[i] = availableLanes[j];
            availableLanes[j] = temp;
        }

        for (int i = 0; i < carsToSpawn && i < spawnXPositions.Length; i++)
        {
            int randomCarIndex = Random.Range(0, carPrefabs.Length);
            float selectedX = spawnXPositions[availableLanes[i]];
            Vector3 spawnPosition = new Vector3(selectedX, 0, spawnZPosition);
            GameObject car = Instantiate(carPrefabs[randomCarIndex], spawnPosition, Quaternion.Euler(0, 180, 0));

            CarMovement carMovement = car.AddComponent<CarMovement>();
            carMovement.speed = carSpeed;
            carMovement.destroyZPosition = destroyZPosition;

            Debug.Log($"<color=green>새로운 자동차가 생성되었습니다! 위치: {spawnPosition}</color>");
        }
    }
}
