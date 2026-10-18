using UnityEngine;
using TMPro;

public class TotalScore : MonoBehaviour
{
    public TextMeshProUGUI totalScoreText;

    public SCORE SCORE;
    public TIMER timer;
    public ScoreManager SM;

    void OnEnable()
    {
        UpdateTotalScore();
    }

    public void UpdateTotalScore()
    {
        if (totalScoreText == null)
        {
            Debug.LogError(
                "Total Score Text not assigned!"
            );

            return;
        }

        if (SM == null)
        {
            Debug.LogError(
                "ScoreManager missing!"
            );

            return;
        }

        if (SCORE == null)
        {
            Debug.LogError(
                "SCORE missing!"
            );

            return;
        }

        int leftTime = 0;

        if (timer != null)
        {
            leftTime =
                Mathf.CeilToInt(
                    timer.LevelTime
                );
        }

        int totalScore =
            SM.coins * 5 +
            Mathf.FloorToInt(
                SCORE.DistanceScore * 0.5f
            ) +
            leftTime * 2;

        totalScoreText.text =
            "Total Score: " +
            totalScore;
    }
    public void DisplayFinalScore(int score)
{
    if (totalScoreText == null)
        return;

    totalScoreText.text = "Total Score: " + score;
}
}