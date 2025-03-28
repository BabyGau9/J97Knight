using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI? scoreText;
	[SerializeField] private TextMeshProUGUI? currentScoreText;
	[SerializeField] private TextMeshProUGUI? highestScoreText;

	void Start()
	{
       
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
		int highesScore = PlayerPrefs.GetInt("HighestScore", 0);

        if (finalScore >= 0)
		{
			if (scoreText != null)
				scoreText.text = "Your Spirit: " + finalScore.ToString();
			if (currentScoreText != null)
				currentScoreText.text = "Current Spirit: " + finalScore.ToString();

			if (finalScore > highesScore)
			{
				PlayerPrefs.SetInt("HighestScore", finalScore);
				highesScore = finalScore;
			}
			//PlayerPrefs.SetInt("FinalScore", 0);
		}
        if (highesScore > 0)
        {
            if (highestScoreText != null)
                highestScoreText.text = "Highest Score: " + highesScore.ToString();
            else
                Debug.LogError("highestScoreText is NULL!");
        }
    }
}
