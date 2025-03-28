using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioPlayer : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("Jump")]
    [SerializeField] AudioClip jumpClip;
    [SerializeField] [Range(0f, 1f)] float jumpVolume = 1f;

    [Header("Attack")]
    [SerializeField] AudioClip attackClip;
    [SerializeField] [Range(0f, 1f)] float attackVolume = 1f;

    [Header("Get Hit")]
    [SerializeField] AudioClip hitClip;
    [SerializeField] [Range(0f, 1f)] float hitVolume = 1f;

    [Header("Pickup Coin")]
    [SerializeField] AudioClip coinClip;
    [SerializeField] [Range(0f, 1f)] float coinVolume = 1f;

    [Header("Pickup Heart")]
    [SerializeField] AudioClip healClip;
    [SerializeField] [Range(0f, 1f)] float healVolume = 1f;

    [Header("Water Splash")]
    [SerializeField] AudioClip waterClip;
    [SerializeField] [Range(0f, 1f)] float waterVolume = 1f;
    [Header("SlimeTransform")]
    [SerializeField] AudioClip slimeTransform;
    [SerializeField][Range(0f, 1f)] float slimeTransformVolume = 4f;
    [Header("SlimeCleave")]
    [SerializeField] AudioClip slimeCleaveClip;
    [SerializeField][Range(0f, 1f)] float slimeCleaveVolumn = 4f;
    [Header("SlimeFlame")]
    [SerializeField] AudioClip slimeFlameClip;
    [SerializeField][Range(0f, 1f)] float slimeFlameVolumn = 4f;
    static AudioPlayer instance;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void PlaySlimeFlame()
    {
        PlayClip(slimeFlameClip, slimeFlameVolumn);
    }
    public void PlaySlimeCleave()
    {
        PlayClip(slimeCleaveClip, slimeCleaveVolumn);
    }
    public void PlayTransform()
    {
        PlayClip(slimeTransform, slimeTransformVolume);
    }
    public void PlayJumpClip(){
        PlayClip(jumpClip, jumpVolume);
    }

    public void PlayAttackClip(){
        PlayClip(attackClip, attackVolume);
    }

    public void PlayHitClip(){
        PlayClip(hitClip, hitVolume);
    }
    
    public void PlayCoinClip(){
        PlayClip(coinClip, coinVolume);
    }

    public void PlayHealClip(){
        PlayClip(healClip, healVolume);
    }

    public void PlaySplashClip(){
        PlayClip(waterClip, waterVolume);
    }

    void PlayClip(AudioClip clip, float volume){
        if(clip != null){
            Vector3 playerPos = player.transform.position;
            AudioSource.PlayClipAtPoint(clip, playerPos, volume);
        }
    }
}
