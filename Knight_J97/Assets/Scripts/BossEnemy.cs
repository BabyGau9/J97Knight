using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Combat Parameters")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float range = 2f;
    [SerializeField] private float colliderDistance = 2.1f;
    [SerializeField] private int damage = 40;
    [SerializeField] private BoxCollider2D boxCollider;

    private float cooldownTimer = Mathf.Infinity;
    private Animator animator;
    private BossPatrol bossPatrol;
    private Health playerHealth;
    private bool isDead = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        bossPatrol = GetComponentInParent<BossPatrol>();
    }

    void Update()
    {
        if (isDead) return;

        cooldownTimer += Time.deltaTime;

        if (PlayerInSight() && playerHealth != null && !playerHealth.isDead)
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("Attack");
            }

            if (bossPatrol != null)
            {
                bossPatrol.StopPatrol(); // Dừng di chuyển khi chuẩn bị tấn công
            }
        }
        else
        {
            if (bossPatrol != null)
            {
                bossPatrol.ResumePatrol(); // Tiếp tục tuần tra khi không thấy player
            }
        }
    }

    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, LayerMask.GetMask("Player"));

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    // Gọi trong Animation Event để gây sát thương
    public void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.PlayerTakeDamage(damage);
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        Destroy(gameObject, 3f); // Xóa boss sau 3 giây
    }
}
