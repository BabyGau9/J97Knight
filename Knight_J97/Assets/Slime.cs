using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSlime : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float moveSpeed = 4f;      // Tốc độ di chuyển (tăng lên)
    [SerializeField] private float stopDistance = 0.3f; // Boss sẽ áp sát rất gần

    [Header("Random Attack Settings")]
    [SerializeField] private float minAttackInterval = 2f; // Thời gian tối thiểu giữa 2 chiêu
    [SerializeField] private float maxAttackInterval = 4f; // Tối đa
    private float attackTimer;         // Đếm thời gian
    private float nextAttackTime;      // Thời điểm random để tung chiêu kế

    [Header("Transform Settings")]
    [SerializeField] private float transformRange = 0.5f; // Khi Player rất gần (<= 0.5f) => Transform
    private bool hasTransformed = false;  // Để chỉ Transform 1 lần

    [Header("Damage Settings")]
    [SerializeField] private float range = 0.5f;       // Tầm đánh (BoxCast)
    [SerializeField] private float colliderDistance = 0.5f;
    [SerializeField] private int damage = 100;           // Sát thương
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Attack Cooldown")]
    [SerializeField] private float attackCooldown = 1f;
    private float cooldownTimer = Mathf.Infinity;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;
    private Health playerHealth;
    AudioPlayer audioPlayer;

    void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Tìm Player qua Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Đặt thời gian tấn công ngẫu nhiên ban đầu
        ResetAttackTime();
    }

    void Update()
    {
        if (player == null) return;

        cooldownTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        // 1. Luôn di chuyển áp sát Player
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer > stopDistance)
        {
            // Tiến về phía Player
            float directionX = player.position.x - transform.position.x;
            float moveDir = Mathf.Sign(directionX);
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

            animator.SetFloat("Speed", Mathf.Abs(moveDir));

            if (moveDir > 0 && !facingRight)
                Flip();
            else if (moveDir < 0 && facingRight)
                Flip();
        }
        else
        {
            // Rất gần => vẫn có thể đứng yên (hoặc di chuyển nhẹ tùy ý)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0f);
        }

        // 2. Random chiêu Cleave/Flame theo thời gian
        if (attackTimer >= nextAttackTime && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0f;
            // Random 0 => Cleave, 1 => Flame
            int randomAttack = Random.Range(0, 2);
            if (randomAttack == 0)
            {
                audioPlayer.PlayAttackClip();

                animator.SetTrigger("Claeve");
            }

            else
            {
                animator.SetTrigger("Flame");
            }

            ResetAttackTime();
        }

        // 3. Khi Player cực gần => Transform (1 lần)
        if (!hasTransformed && distToPlayer <= transformRange)
        {
            hasTransformed = true;
            animator.SetTrigger("Transform");
        }
    }

    // Hàm này được gọi bởi Animation Event (trong Cleave/Flame)
    void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.PlayerTakeDamage(damage);
        }
    }

    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0f,
            Vector2.left,
            0f,
            LayerMask.GetMask("Player")
        );
        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            return true;
        }
        return false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void ResetAttackTime()
    {
        attackTimer = 0f;
        nextAttackTime = Random.Range(minAttackInterval, maxAttackInterval);
    }

    // Debug vùng đánh
    void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z)
            );
        }
    }
}
