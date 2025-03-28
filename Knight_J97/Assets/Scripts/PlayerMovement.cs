using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] float attackRate = 2f;
    [SerializeField] int attackDamage = 50;
    [SerializeField] int hazardDamage = 20;
    [SerializeField] GameObject slash;
    [SerializeField] Transform slashPoint;
    [SerializeField] float fireSkillCooldown = 5f;
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireCooldown = 5f;

    private bool canUseFireSkill = true;
    float nextAttackTime = 0f;
    AudioPlayer audioPlayer;
    const float groundCheckRadius = 0.2f;
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animator;
    CapsuleCollider2D capsule;
    BoxCollider2D box;
    bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider2D>();
        box = GetComponent<BoxCollider2D>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    void Update()
    {
        Run();
        FlipSprite();
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        GroundCheck();
        DieByHazard();

        if (Input.GetKeyDown(KeyCode.B))
        {
            OnFireSkill();
        }
    }

    void OnMove(InputValue value)
    {
        if (!PauseMenu.isPaused)
        {
            moveInput = value.Get<Vector2>();
        }
    }

    void OnJump(InputValue value){
        if(!box.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            return;
        }
        if(value.isPressed && !PauseMenu.isPaused){
            rb.linearVelocity += new Vector2(0f, jumpPower);
            animator.SetBool("isJumping", true);
            audioPlayer.PlayJumpClip();
        }
    }

    void OnAttack(){
        if(Time.time >= nextAttackTime && !PauseMenu.isPaused){
            animator.SetTrigger("Attack");
            StartCoroutine(SlashDelay());
            audioPlayer.PlayAttackClip();
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, LayerMask.GetMask("Enemy"));
            foreach(Collider2D enemy in hitEnemies){
                enemy.GetComponent<Health>().EnemyTakeDamage(attackDamage);
            }
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void OnFireSkill()
    {
        if (PauseMenu.isPaused || !canUseFireSkill) return;

        Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        canUseFireSkill = false;
        StartCoroutine(ResetFireSkillCooldown());
    }

    IEnumerator ResetFireSkillCooldown()
    {
        yield return new WaitForSeconds(fireCooldown);
        canUseFireSkill = true;
    }
    void OnDrawGizmosSelected() {
        if(attackPoint == null){
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.linearVelocity.y);
        rb.linearVelocity = playerVelocity;
        bool hasHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x) > 0;
        animator.SetBool("isRunning", hasHorizontalSpeed);
    }

    void FlipSprite(){
            bool hasHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x) > Mathf.Epsilon;
            if(hasHorizontalSpeed){
                transform.localScale = new Vector2(Mathf.Sign(rb.linearVelocity.x), 1f);
                slash.transform.localScale = new Vector2(Mathf.Sign(rb.linearVelocity.x), 1f);
            }
    }

    void GroundCheck(){
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, LayerMask.GetMask("Ground"));
        if(colliders.Length > 0){
            isGrounded = true;
        }
        animator.SetBool("isJumping", !isGrounded);
    }

    void DieByHazard(){
        if(capsule.IsTouchingLayers(LayerMask.GetMask("Hazards"))){
            capsule.GetComponent<Health>().PlayerTakeDamage(hazardDamage);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(capsule.IsTouchingLayers(LayerMask.GetMask("Water"))){
            audioPlayer.PlaySplashClip();
        }
    }

    IEnumerator SlashDelay(){
        yield return new WaitForSeconds(0.15f);
        Instantiate(slash, slashPoint.position, Quaternion.identity);
    }
}