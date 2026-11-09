
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Level Settings")]
    [Tooltip("Use 1 for Level 1, 2 for Level 2, etc.")]
    public int levelIndex = 1;

    [Header("UI References")]
    public TMP_Text highScoreText;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError(
                "LevelButton: Missing Button component on " +
                gameObject.name
            );

            return;
        }

        if (levelIndex < 1 ||
            levelIndex > LevelManager.TotalLevels)
        {
            Debug.LogError(
                "LevelButton: Invalid levelIndex on " +
                gameObject.name
            );

            button.interactable = false;
            return;
        }

        RefreshButton();

        button.onClick.RemoveListener(LoadLevel);
        button.onClick.AddListener(LoadLevel);
    }

    private void RefreshButton()
    {
        int unlockedLevel = LevelManager.GetUnlockedLevel();

        button.interactable = levelIndex <= unlockedLevel;

        int highScore = PlayerPrefs.GetInt(
            "HighScore_" + levelIndex,
            0
        );

        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString();
        }

        Debug.Log(
            $"Level {levelIndex} | High Score: {highScore}"
        );
    }

    private void LoadLevel()
    {
        if (levelIndex > LevelManager.GetUnlockedLevel())
        {
            Debug.Log("Level " + levelIndex + " is locked.");
            return;
        }

        // Load by scene name instead of Build Index.
        string targetScene = "Level " + levelIndex;

        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogError(
                "LevelButton: Cannot load scene: " +
                targetScene +
                ". Check the active Build Profile."
            );

            return;
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            targetScene,
            LoadSceneMode.Single
        );
    }
}
