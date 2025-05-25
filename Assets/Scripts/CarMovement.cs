using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 10f; // 자동차 이동 속도
    public float destroyZPosition = -15f; // 자동차가 파괴될 Z 위치

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Debug.Log($"<color=cyan>자동차가 시작됨 - 현재 위치: {transform.position.z}, 목표 파괴 위치: {destroyZPosition}</color>");
    }

    // Update is called once per frame
    private void Update()
    {
        // 자동차를 플레이어 방향(forward)으로 이동
        // 자동차가 180도 회전된 상태이므로 forward는 플레이어 방향이 됨
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        // 자동차가 지정된 위치를 지나면 파괴
        if (transform.position.z <= destroyZPosition)
        {
            Debug.Log($"<color=red>자동차 파괴됨 - 위치: {transform.position.z}</color>");
            Destroy(gameObject);
        }
    }
}
