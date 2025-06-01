using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;  // 이동 속도
    public float minX = -5f;      // 도로의 최소 X 좌표
    public float maxX = 5f;       // 도로의 최대 X 좌표

    private Rigidbody rb;         // Rigidbody 참조
    private Animator animator;    // Animator 참조

    void Start()
    {
        // Rigidbody 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver || GameManager.Instance.isPaused)
            return;

        // 1. 상하 입력 제거 → 좌우 입력만 처리
        float moveInput = Input.GetAxis("Horizontal");
        float verticalInput = 0f; // 상하 이동을 강제로 0으로 설정

        // 2. Rigidbody 속도 설정 (좌우만 적용)
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0f); // Z축(앞뒤) 고정

        // 3. 방향 전환 (좌우만 회전)
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            transform.rotation = Quaternion.Euler(0, moveInput > 0 ? 90f : -90f, 0); // 오른쪽: 90°, 왼쪽: -90°
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        // 4. X축 경계 제한 (기존 코드 유지)
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 자동차와 충돌 시 게임 오버
        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log("플레이어가 자동차와 충돌!");
            GameManager.Instance.GameOver();
        }
    }
}
