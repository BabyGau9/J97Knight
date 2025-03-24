using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDisplay : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Slider healthSlider;      // Kéo thả Slider vào đây
    [SerializeField] Health playerHealth;      // Kéo thả object có script Health

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText; // Kéo thả TMP Text
    ScoreKeeper scoreKeeper;

    void Awake()
    {
        // Tìm ScoreKeeper trong scene
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        if (!scoreKeeper)
        {
            Debug.LogError("Không tìm thấy ScoreKeeper trong scene!");
        }
    }

    void Start()
    {
        // Kiểm tra null
        if (!healthSlider)
        {
            Debug.LogError("healthSlider chưa được gán trong UIDisplay!", this);
            return;
        }
        if (!playerHealth)
        {
            Debug.LogError("playerHealth chưa được gán trong UIDisplay!", this);
            return;
        }
        if (!scoreText)
        {
            Debug.LogError("scoreText chưa được gán trong UIDisplay!", this);
            return;
        }

        // Đặt maxValue của thanh máu = máu hiện tại của player
        healthSlider.maxValue = playerHealth.GetHealth();
    }

    void Update()
    {
        if (healthSlider && playerHealth)
        {
            healthSlider.value = playerHealth.GetHealth();
        }

        if (scoreText && scoreKeeper)
        {
            scoreText.text = "Score: " + scoreKeeper.GetScore().ToString();
        }
    }
}
