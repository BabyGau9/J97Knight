using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;  // Tốc độ di chuyển
    [SerializeField] private bool canJump = false;  // Nếu slime biết nhảy, đặt = true
    [SerializeField] private float jumpForce = 5f;  // Lực nhảy

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;  // Thời gian chờ giữa 2 đòn đánh
    [SerializeField] private float range = 0.5f;         // Tầm đánh
    [SerializeField] private float colliderDistance = 0.5f;
    [SerializeField] private int damage = 1;             // Sát thương
    [SerializeField] private BoxCollider2D boxCollider;

    private float cooldownTimer = Mathf.Infinity;        // Đếm thời gian hồi chiêu
    private Animator animator;
    private Health playerHealth;                         // Tham chiếu script Health của player (nếu có)
    private bool facingRight = true;                     // Kiểm tra slime đang quay mặt bên nào
    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
    }
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- 1. Di chuyển ---
        float horizontal = Input.GetAxisRaw("Horizontal");  // Lấy input A/D hoặc phím mũi tên

        // Cập nhật tham số Speed cho Animator (lấy giá trị tuyệt đối để tránh âm/dương)
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        // Lật hướng nhân vật nếu cần (khi nhấn phím ngược hướng)
        if (horizontal > 0 && !facingRight)
            Flip();
        else if (horizontal < 0 && facingRight)
            Flip();

        // --- 2. Nhảy (nếu slime biết nhảy) ---
        if (canJump && Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump");
        }

        // --- 3. Tấn công ---
        cooldownTimer += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && cooldownTimer >= attackCooldown)
        {
            // Nhấn chuột trái hoặc phím (theo Input Manager) để đánh
            cooldownTimer = 0;
            animator.SetTrigger("Attack");
        }
    }

    // Hàm này được gọi bởi Animation Event (nếu bạn đặt trong animation Attack)
    void DamagePlayer()
    {
        if (PlayerInSight())
        {
            Debug.Log("Gây sát thương cho Player!");
            playerHealth.PlayerTakeDamage(damage);
        }
        else
        {
            Debug.Log("Không trúng Player!");
        }
    }

    // Tương tự như TrollController, bạn BoxCast để phát hiện Player
    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, LayerMask.GetMask("Player")
        );

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            return true;
        }
        return false;
    }

    // Vẽ Gizmos để kiểm tra vùng đánh
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z)
        );
    }
   
    // Hàm lật hướng slime
    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
