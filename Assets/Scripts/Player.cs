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

        float moveInput = Input.GetAxis("Horizontal");
        float absInput = Mathf.Abs(moveInput);

        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
        animator.SetFloat("Speed", absInput);

        if (moveInput > 0.1f)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (moveInput < -0.1f)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            GameManager.Instance.PlaySFX(GameManager.Instance.hitSfx); // 충돌 효과음
            GameManager.Instance.GameOver();
        }
    }
}
