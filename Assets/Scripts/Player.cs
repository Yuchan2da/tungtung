using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;  // 이동 속도
    public float minX = -5f;      // 도로의 최소 X 좌표 (왼쪽 경계)
    public float maxX = 5f;       // 도로의 최대 X 좌표 (오른쪽 경계)

    private Rigidbody rb;         // Rigidbody 참조

    void Start()
    {
        // Rigidbody 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 입력을 받아서 X축으로만 움직임
        float moveInput = Input.GetAxis("Horizontal");

        // Rigidbody의 속도 (관성 효과를 없애기 위해 y, z 축은 그대로 두고 x축만 제어)
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);

        // 플레이어가 X축 범위를 벗어나지 않도록 제한
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX); // X좌표 제한
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z); // 위치 재설정
    }
}
