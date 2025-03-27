using UnityEngine;

public class BossSlime : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float moveSpeed = 4f;          // Tốc độ di chuyển tối đa
    [SerializeField] private float approachThreshold = 2f;  // Khoảng cách từ safeDistance tới đó Boss di chuyển full speed

    [Header("Random Attack Settings")]
    [SerializeField] private float minAttackInterval = 0.5f;  // Thời gian tối thiểu giữa 2 chiêu
    [SerializeField] private float maxAttackInterval = 1f;    // Thời gian tối đa giữa 2 chiêu
    private float attackTimer = 0f;         // Đếm thời gian attack
    private float nextAttackTime = 0f;      // Thời điểm để tung chiêu kế

    [Header("Transform Settings")]
    [SerializeField] private float transformRange = 1.5f; // Khi Player rất gần, kích hoạt Transform
    private bool hasTransformed = false;  // Chỉ transform 1 lần

    [Header("Damage Settings")]
    [SerializeField] private float range = 0.5f;        // Tầm đánh (BoxCast)
    [SerializeField] private float colliderDistance = 0.5f;
    [SerializeField] private int damage = 1;            // Sát thương
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float bossRadius = 0.2f;     // Bán kính Boss
    [SerializeField] private float playerRadius = 0.2f;   // Bán kính Player
    [SerializeField] private float offsetDistance = 0.1f; // Khoảng bù an toàn

    private float safeDistance; // = bossRadius + playerRadius + offsetDistance

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;
    private Health playerHealth;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Tìm đối tượng Player qua Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Đặt thời gian attack ban đầu
        ResetAttackTime();
    }

    void Update()
    {
        if (player == null) return;

        // Cập nhật bộ đếm thời gian cho attack
        attackTimer += Time.deltaTime;

        // Tính safeDistance dựa trên kích thước của Boss và Player
        safeDistance = bossRadius + playerRadius + offsetDistance;
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. Di chuyển: Boss sẽ di chuyển nếu cách Player hơn safeDistance.
        // Tốc độ sẽ được tính giảm dần khi gần safeDistance (càng gần càng chậm).
        if (distToPlayer > safeDistance)
        {
            // Tính hệ số tốc độ: từ 0 (khi dist == safeDistance) đến 1 (khi dist == safeDistance + approachThreshold)
            float factor = Mathf.InverseLerp(safeDistance, safeDistance + approachThreshold, distToPlayer);
            float effectiveSpeed = moveSpeed * factor;

            float directionX = player.position.x - transform.position.x;
            float moveDir = Mathf.Sign(directionX);

            rb.linearVelocity = new Vector2(moveDir * effectiveSpeed, rb.linearVelocity.y);
            animator.SetFloat("Speed", Mathf.Abs(effectiveSpeed));

            // Lật hướng nếu cần
            if (moveDir > 0 && !facingRight)
                Flip();
            else if (moveDir < 0 && facingRight)
                Flip();
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0f);
        }

        // 2. Random Attack: Khi đủ thời gian, Boss sẽ tung chiêu "Cleave" hoặc "Flame"
        if (attackTimer >= nextAttackTime)
        {
            int randomAttack = Random.Range(0, 2);
            if (randomAttack == 0)
                animator.SetTrigger("Claeve");
            else
                animator.SetTrigger("Flame");

            ResetAttackTime();
        }

        // 3. Transform: Khi Player cực gần, kích hoạt Transform (chỉ 1 lần)
        if (!hasTransformed && distToPlayer <= transformRange)
        {
            animator.SetTrigger("Transform");
            hasTransformed = true;
            moveSpeed += 1f;
        }

        // Debug thông tin (có thể tạm thời để kiểm tra)
        Debug.Log("Attack Timer: " + attackTimer + " / Next Attack: " + nextAttackTime);
        Debug.Log("Distance to Player: " + distToPlayer);
    }

    // Hàm này được gọi bởi Animation Event trong chiêu Cleave/Flame để gây sát thương cho Player
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

    // Debug: Vẽ vùng đánh (BoxCast) để kiểm tra
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
