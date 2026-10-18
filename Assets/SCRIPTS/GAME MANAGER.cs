using UnityEngine;
using UnityEngine.SceneManagement;

public class GAMEMANAGER : MonoBehaviour
{
    private bool gameHasEnded = false;
    private bool levelCompleted = false;

    public GameObject completeLevelUI;
    

    

    void Awake()
    {
        // Every newly loaded level must start unfrozen.
        Time.timeScale = 1f;
    }

    

    

public void OnLevelComplete(int levelIndex)
{
    if (levelCompleted)
        return;

    levelCompleted = true;

    Debug.Log("LEVEL " + levelIndex + " COMPLETED");

    Time.timeScale = 0f;


    if (completeLevelUI != null)
    {
        completeLevelUI.SetActive(true);
    }
    Debug.Log(
    "UnlockedLevel immediately after completion: " +
    PlayerPrefs.GetInt("UnlockedLevel", 1)
);
    LevelManager.UnlockLevel(levelIndex);
} 

    public void RecordLevelFailure()
    {
        if (
            gameHasEnded ||
            levelCompleted
        )
        {
            return;
        }

        gameHasEnded = true;

        float distance = 0f;
        int coins = 0;

        if (SCORE.instance != null)
        {
            distance =
                SCORE.instance.DistanceScore;
        }

        if (ScoreManager.instance != null)
        {
            coins =
                ScoreManager.instance.coins;
        }

        if (Analytics.Instance != null)
        {
            Analytics.Instance.LevelFailed(
                Analytics.Instance.GetCurrentLevel(),
                distance,
                coins
            );
        }

        Debug.Log(
            "LEVEL FAILURE RECORDED"
        );
    }

  

    public void EndGame()
    {
        if (
            gameHasEnded
        )
        {
            return;
        }

        Debug.Log(
            "GAME OVER"
        );

        RecordLevelFailure();

        Restart();
    }


    public void Restart()
{
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    Time.timeScale = 1f;
    
    Debug.Log("LEVEL RESTARTED");
}

       public void LoadNextLevel()
    {
        // CRITICAL:
        // Completion sets Time.timeScale to 0.
        // Next level must restore it before loading.
        Time.timeScale = 1f;

        CancelInvoke();

        int nextScene =
            SceneManager.GetActiveScene().buildIndex + 1;

        if (
            nextScene >=
            SceneManager.sceneCountInBuildSettings
        )
        {
            Debug.Log(
                "No next level exists."
            );

            return;
        }

        SceneManager.LoadScene(
            nextScene
        );
    }

    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        CancelInvoke();

        SceneManager.LoadScene(
            "LEVELS"
        );
    }
}