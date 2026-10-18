
using UnityEngine;

public static class LevelManager
{
    private const string KEY = "UnlockedLevel";
    public const int TotalLevels = 10;

    // The stored value is the highest unlocked level number.
    // Default: Level 1.
    public static int GetUnlockedLevel()
    {
        int level = PlayerPrefs.GetInt(KEY, 1);

        return Mathf.Clamp(level, 1, TotalLevels);
    }

    // completedLevelNumber is 1-based:
    // Level 1 = 1, Level 2 = 2, etc.
    public static void UnlockLevel(int completedLevelNumber)
    {
        if (completedLevelNumber < 1 ||
            completedLevelNumber > TotalLevels)
        {
            Debug.LogError(
                "LevelManager: Invalid completed level number: " +
                completedLevelNumber
            );
            return;
        }

        // Completing Level 1 unlocks Level 2.
        int nextLevel = Mathf.Min(
            completedLevelNumber + 1,
            TotalLevels
        );

        int currentlyUnlocked = GetUnlockedLevel();

        if (nextLevel > currentlyUnlocked)
        {
            PlayerPrefs.SetInt(KEY, nextLevel);
            PlayerPrefs.Save();

            Debug.Log("Unlocked Level: " + nextLevel);
        }
    }

    // Resets progression, preserving high scores.
    public static void ResetProgress()
    {
        PlayerPrefs.SetInt(KEY, 1);
        PlayerPrefs.Save();

        Debug.Log("Progress reset: only Level 1 is unlocked.");
    }
    
}
