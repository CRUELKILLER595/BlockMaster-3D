
using UnityEngine;

public class PlayerPrefsResetter : MonoBehaviour
{
    private const string RESET_KEY =
        "BlockMaster_FreshPlaytestReset_v3";

    private void Awake()
    {
        // Do not reset again once the marker exists.
        if (PlayerPrefs.GetInt(RESET_KEY, 0) == 1)
            return;

        Debug.LogWarning(
            "Performing one-time fresh game reset."
        );

        // Remove old high scores, level progression,
        // and other PlayerPrefs.
        PlayerPrefs.DeleteAll();

        // Start fresh with Level 1 unlocked.
        PlayerPrefs.SetInt("UnlockedLevel", 1);

        // Remember that the reset has completed.
        PlayerPrefs.SetInt(RESET_KEY, 1);
        PlayerPrefs.Save();

        Debug.Log(
            "Reset complete. Only Level 1 is unlocked; " +
            "high scores default to zero."
        );
    }
}
