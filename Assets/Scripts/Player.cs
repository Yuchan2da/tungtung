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
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        float moveInput = Input.GetAxis("Horizontal");
        float absInput = Mathf.Abs(moveInput);

        rb.velocity = new Vector3(moveInput * moveSpeed, rb.velocity.y, rb.velocity.z);
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
            GameManager.Instance.PlaySFX(GameManager.Instance.hitSfx);
            GameManager.Instance.GameOver();
        }
    }
}
