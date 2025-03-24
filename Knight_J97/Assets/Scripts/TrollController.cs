using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollController : MonoBehaviour
{
    [SerializeField] float attackCooldown;
    [SerializeField] float range;
    [SerializeField] float colliderDistance;
    [SerializeField] int damage;
    [SerializeField] BoxCollider2D boxCollider;
    float cooldownTimer = Mathf.Infinity;
    AudioPlayer audioPlayer;
    Animator animator;
    EnemyPatrol enemyPatrol;
    Health playerHealth;
    private float fixedY;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        animator.applyRootMotion = false;
    }

    void Start()
    {
        fixedY = transform.position.y; 
    }
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (PlayerInSight() && !playerHealth.isDead)
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("Attack");
                audioPlayer.PlayAttackClip();
            }
        }

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }
    }

    void OnAnimatorMove()
    {
        
        Vector3 newPosition = transform.position;
        newPosition.y = fixedY;
        transform.position = newPosition;
    }

    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
        new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z)
        , 0, Vector2.left, 0, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            Debug.Log("Phát hiện Player!");
        }
        else
        {
            Debug.Log("Không có Player trong tầm đánh!");
        }
        return hit.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
        new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    void DamagePlayer()
    {
        if (PlayerInSight())
        {
            Debug.Log("Yes!");
            playerHealth.PlayerTakeDamage(damage);
        }
        else
        {
            Debug.Log("No!");
        }
    }
}
