using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float minX = -5f;
    public float maxX = 5f;

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver || GameManager.Instance.isPaused)
            return;

        // 좌우 입력 받기
        float moveInput = Input.GetAxis("Horizontal");

        // Rigidbody 속도 설정 (X축 이동만)
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);

        // 애니메이션 매개변수 설정 (Speed 파라미터로 Idle ↔ Walk 전환)
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // 이동 방향을 바라보게 회전
        if (moveInput > 0.1f)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);   // 오른쪽
        else if (moveInput < -0.1f)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);  // 왼쪽

        // X축 위치 제한
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log("플레이어가 자동차와 충돌!");
            GameManager.Instance.GameOver();
        }
    }
}
