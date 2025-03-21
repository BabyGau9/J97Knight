using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] Transform leftEdge;
    [SerializeField] Transform rightEdge;

    [Header("Boss")]
    [SerializeField] Transform boss;

    [Header("Movement Parameters")]
    [SerializeField] float speed;
    private Vector3 initScale;
    private bool movingLeft;

    [Header("Idle")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Boss Animator")]
    [SerializeField] private Animator animator;

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
        if (boss != null)
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
    }

    void DirectionChange()
    {
        animator.SetBool("isMoving", false);
        idleTimer += Time.deltaTime;
        if (idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0;
        }
    }

    void MoveInDirection(int direction)
    {
        idleTimer = 0;
        animator.SetBool("isMoving", true);
        boss.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
        boss.position = new Vector3(boss.position.x + Time.deltaTime * direction * speed, boss.position.y, boss.position.z);
    }

    public void StopPatrol()
    {
        animator.SetBool("isMoving", false);
        this.enabled = false; // Dừng tuần tra khi boss chuẩn bị tấn công
    }

    public void ResumePatrol()
    {
        this.enabled = true; // Tiếp tục tuần tra khi không phát hiện player
    }
}
