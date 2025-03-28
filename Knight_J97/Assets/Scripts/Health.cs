using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] bool isAI;
    [SerializeField] bool isFinalBoss;
    [SerializeField] int health = 100;
    [SerializeField] int score = 50;
    [SerializeField] int healAmount = 30;
    [SerializeField] float healCooldown = 60f;
    [SerializeField] GameObject healEffectPrefab;


    Animator animator;
    AudioPlayer audioPlayer;
    bool isVulnerable = true;
    [HideInInspector] public bool isDead = false;
    private bool canHeal = true;
    ScoreKeeper scoreKeeper;
    LevelManager levelManager;
    static int MAX_HEALTH = 100;
    [SerializeField] GameObject heart;
    [SerializeField] Transform dropPoint;
    int dropChance;
    [SerializeField] bool isBoss;
    [SerializeField] GameObject exitPortal;
    [SerializeField] Transform exitDropPoint;

    void Start(){
        animator = GetComponent<Animator>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
        scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
        levelManager = FindObjectOfType<LevelManager>();
        dropChance = Random.Range(1, 101);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && canHeal)
        {
            TryHeal();
        }
    }

    void TryHeal()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("nPlayer Idling")) return;
        ActivateHealSkill();
        Debug.Log("Current State: " + animator.GetCurrentAnimatorStateInfo(0).IsName("nPlayer Idling"));
    }

    void ActivateHealSkill()
    {
        if (health < MAX_HEALTH)
        {
            health = Mathf.Min(health + healAmount, MAX_HEALTH);
            animator.SetTrigger("Heal");
            audioPlayer.PlayHealClip();

            if (healEffectPrefab != null)
            {
                Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            }

            StartCoroutine(HealCooldown());
        }
    }

    IEnumerator HealCooldown()
    {
        canHeal = false;
        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
    }

    public void PlayerTakeDamage(int damage){
        if(isVulnerable){
            health -= damage;
            isVulnerable = false;
            animator.SetTrigger("Hit");
            audioPlayer.PlayHitClip();
            StartCoroutine(DieDelay());
            StartCoroutine(Invunerable());
            StartCoroutine(VisualIndicator(Color.red));
        }
    }

    public void EnemyTakeDamage(int damage){
        health -= damage;
        StartCoroutine(DieDelay());
        StartCoroutine(VisualIndicator(Color.red));
    }

    IEnumerator VisualIndicator(Color color){
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.15f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator Invunerable(){
        if(!isDead){
            float blinkDelay = 0.0836f;
            for(int i = 0; i < 10; i++){
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                yield return new WaitForSeconds(blinkDelay);
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(blinkDelay);
            }
            isVulnerable = true;
        }
    }

    IEnumerator DieDelay(){
        if(health <= 0 && isAI){
            Debug.Log($"dropChance: {dropChance}, isBoss: {isBoss}, isFinalBoss: {isFinalBoss}");
            yield return new WaitForSeconds(0.15f);
            if(dropChance >= 80 && !isBoss && !isFinalBoss)
            {
                Instantiate(heart, dropPoint.transform.position, Quaternion.identity);
            }
            if(isBoss && !isFinalBoss)
            {
                Instantiate(exitPortal, exitDropPoint.transform.position, Quaternion.identity);
            }
            if(isBoss && isFinalBoss)
            {
                animator.SetTrigger("Death");
                audioPlayer.PlaySlimeFlame();
            }
            Destroy(transform.parent.gameObject);
            scoreKeeper.ModifyScore(score);
        }
        if(health <= 0 && !isAI){
            isDead = true;
            yield return new WaitForSeconds(0.15f);
            animator.SetTrigger("Die");
            GetComponent<PlayerMovement>().enabled = false;
			PlayerPrefs.SetInt("FinalScore", scoreKeeper.GetScore());

			levelManager.LoadGameOver();
        }
    }

    public void Heal(int value){
        health = Mathf.Min(health + value, MAX_HEALTH);
    }

    public int GetHealth(){
        return health;
    }
}
