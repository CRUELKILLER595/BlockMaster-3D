
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    [Header("References")]
    public SCORE SCORE;
    public TIMER timer;
    public PlayerMovement player;

    [Header("Gameplay UI")]
    [Tooltip("Assign gameplay UI objects to hide at completion. Do not include the completion panel or its parent Canvas.")]
    public GameObject[] gameplayUIToHide;

    private bool levelCompleted = false;

    public void CompleteLevel()
    {
        if (levelCompleted)
            return;

        // Build index 2 = Level 1, so this gives Level 1 = 1.
        int levelNumber =
            SceneManager.GetActiveScene().buildIndex - 1;

        if (levelNumber < 1 ||
            levelNumber > LevelManager.TotalLevels)
        {
            Debug.LogError(
                "LevelEnd: Unexpected gameplay scene build index: " +
                SceneManager.GetActiveScene().buildIndex
            );
            return;
        }

        levelCompleted = true;

        Debug.Log("Completed Level " + levelNumber);

        if (player != null)
            player.enabled = false;

        int timeRemaining = 0;

        if (timer != null)
        {
            timer.StopTimer();
            timeRemaining = Mathf.CeilToInt(timer.LevelTime);
        }

        float distance = 0f;

        if (SCORE != null)
        {
            distance = SCORE.DistanceScore;
        }
        else if (global::SCORE.instance != null)
        {
            distance = global::SCORE.instance.DistanceScore;
        }

        int coins = 0;

        if (ScoreManager.instance != null)
            coins = ScoreManager.instance.coins;

        int totalScore =
            coins * 5 +
            Mathf.FloorToInt(distance * 0.5f) +
            timeRemaining * 2;

        // Save using the project's existing 1-based key.
        string scoreKey = "HighScore_" + levelNumber;

        int previousHighScore =
            PlayerPrefs.GetInt(scoreKey, 0);

        if (totalScore > previousHighScore)
        {
            PlayerPrefs.SetInt(scoreKey, totalScore);
            PlayerPrefs.Save();

            Debug.Log(
                "New high score for Level " +
                levelNumber + ": " + totalScore
            );

        }
        Debug.Log("Current run score: " + totalScore);
Debug.Log("Saved high score key: " + scoreKey);
Debug.Log("Previous high score: " + previousHighScore);

        if (Analytics.Instance != null)
        {
            Analytics.Instance.LevelCompleted(
                levelNumber,
                distance,
                coins,
                totalScore
            );
        }

        // Hide gameplay UI, keeping the completion UI visible.
        if (gameplayUIToHide != null)
        {
            foreach (GameObject uiObject in gameplayUIToHide)
            {
                if (uiObject != null)
                    uiObject.SetActive(false);
            }
        }

        GAMEMANAGER gm =
            FindFirstObjectByType<GAMEMANAGER>();
            if (gm == null)
        {
            Debug.LogError(
                "LevelEnd: GAMEMANAGER not found."
            );
        }

        if (gm != null)
        {
            // GAMEMANAGER.OnLevelComplete must call
            // LevelManager.UnlockLevel(levelNumber).
            gm.OnLevelComplete(levelNumber);
            TotalScore scoreUI =
    FindFirstObjectByType<TotalScore>();

if (scoreUI != null)
{
    scoreUI.DisplayFinalScore(totalScore);
}
        }
        else
        {
            Debug.LogError(
                "LevelEnd: GAMEMANAGER not found."
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        CompleteLevel();
    }
}
