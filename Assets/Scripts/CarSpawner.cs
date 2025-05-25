using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Car Settings")]
    public GameObject[] carPrefabs; // 7개의 자동차 프리팹을 저장할 배열
    public float minSpawnInterval = 2f; // 최소 스폰 간격
    public float maxSpawnInterval = 5f; // 최대 스폰 간격
    public float carSpeed = 3f; // 자동차 이동 속도
    
    [Header("Spawn Position")]
    public float spawnZPosition = 2f; // 자동차가 생성될 Z 위치
    public float destroyZPosition = -15f; // 자동차가 파괴될 Z 위치
    
    [Header("Lane Settings")]
    public float minXPosition = -2.2f; // 도로의 왼쪽 끝
    public float maxXPosition = 2.4f; // 도로의 오른쪽 끝
    public int laneCount = 5; // 차선 수
    private float[] spawnXPositions; // 자동차가 생성될 X 좌표 배열

    [Range(1, 3)]
    public int minCarsPerSpawn = 1; // 한 번에 생성될 최소 자동차 수
    [Range(1, 3)]
    public int maxCarsPerSpawn = 3; // 한 번에 생성될 최대 자동차 수

    private float nextSpawnTime;

    private void Start()
    {
        // 첫 스폰 시간 설정
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        Debug.Log("<color=yellow>자동차 스포너가 시작되었습니다!</color>");

        // 차선 위치 초기화
        InitializeLanePositions();
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
        // 스폰 시간이 되었는지 체크
        if (Time.time >= nextSpawnTime)
        {
            SpawnCars();
            // 다음 스폰 시간 설정
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

        // 이번에 생성할 자동차 수 결정
        int carsToSpawn = Random.Range(minCarsPerSpawn, maxCarsPerSpawn + 1);
        
        // 사용할 X 위치들을 섞어서 중복 없이 선택
        int[] availableLanes = new int[spawnXPositions.Length];
        for (int i = 0; i < spawnXPositions.Length; i++)
        {
            availableLanes[i] = i;
        }
        
        // Fisher-Yates 셔플 알고리즘
        for (int i = availableLanes.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = availableLanes[i];
            availableLanes[i] = availableLanes[j];
            availableLanes[j] = temp;
        }

        // 결정된 수만큼 자동차 생성
        for (int i = 0; i < carsToSpawn && i < spawnXPositions.Length; i++)
        {
            // 랜덤한 자동차 프리팹 선택
            int randomCarIndex = Random.Range(0, carPrefabs.Length);
            
            // 선택된 차선의 X 위치 사용
            float selectedX = spawnXPositions[availableLanes[i]];
            
            // 자동차 생성 - 180도 회전된 상태로 생성
            Vector3 spawnPosition = new Vector3(selectedX, 0, spawnZPosition);
            GameObject car = Instantiate(carPrefabs[randomCarIndex], spawnPosition, Quaternion.Euler(0, 180, 0));
            
            // 자동차에 이동 컴포넌트 추가
            CarMovement carMovement = car.AddComponent<CarMovement>();
            carMovement.speed = carSpeed;
            carMovement.destroyZPosition = destroyZPosition;

            Debug.Log($"<color=green>새로운 자동차가 생성되었습니다! 위치: {spawnPosition}, 목표 파괴 위치: {destroyZPosition}</color>");
        }
    }
}
