using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] Transform leftEdge;
    [SerializeField] Transform rightEdge;

    [Header("Boss Settings")]
    [SerializeField] Transform boss;
    [SerializeField] float speed;
    Vector3 initScale;
    bool movingLeft;

    [Header("Idle Settings")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Boss Animator")]
    [SerializeField] private Animator animator;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private float attackCooldown = 2f;
    private float lastAttackTime;

    void Awake()
    {
        initScale = boss.localScale;
    }

    void OnDisable()
    {
        animator.SetBool("isMoving", false);
    }

    void Update()
    {
        if (boss == null) return;

        float distanceToPlayer = Vector3.Distance(boss.position, player.position);

        // Kiểm tra khoảng cách để tấn công
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (movingLeft)
        {
            if (boss.position.x >= leftEdge.position.x)
            {
                MoveInDirection(-1);
            }
            else
            {
                DirectionChange();
            }
        }
        else
        {
            if (boss.position.x <= rightEdge.position.x)
            {
                MoveInDirection(1);
            }
            else
            {
                DirectionChange();
            }
        }
    }

    void DirectionChange()
    {
        animator.SetBool("isMoving", false);
        animator.Play("Boss2 idling");
        idleTimer += Time.deltaTime;
        if (idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
        }
    }

    void MoveInDirection(int direction)
    {
        idleTimer = 0;
        animator.SetBool("isMoving", true);
        animator.Play("Boss2 walk");
        boss.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
        boss.position = new Vector3(boss.position.x + Time.deltaTime * direction * speed, boss.position.y, boss.position.z);
    }

    void Attack()
    {
        animator.SetBool("isMoving", false);
        int attackType = Random.Range(1, 4); // Chọn random kiểu tấn công
        switch (attackType)
        {
            case 1:
                animator.Play("Boss2 attack");
                break;
            case 2:
                animator.Play("Boss2 flykick");
                break;
            case 3:
                animator.Play("Boss2 strike");
                break;
        }
    }

    public void TakeDamage()
    {
        animator.Play("Boss2 hurt");
    }

    public void GetDizzy()
    {
        animator.Play("Boss2 dizzy");
    }

    public void Die()
    {
        animator.Play("Boss2 die");
        Destroy(gameObject, 2f); // Xóa boss sau 2s
    }

    public void Win()
    {
        animator.Play("Boss2 win");
    }
}
