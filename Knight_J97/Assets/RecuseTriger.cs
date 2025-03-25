using UnityEngine;
using UnityEngine.SceneManagement;

public class RescueTrigger : MonoBehaviour
{
    public int currentScore = 0; // Có thể truyền từ GameManager vào nếu cần

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Lưu điểm trước khi chuyển scene
            PlayerPrefs.SetInt("FinalScore", currentScore);
            PlayerPrefs.Save();

            // Chuyển scene
            SceneManager.LoadScene("Victory");
        }
    }
}
