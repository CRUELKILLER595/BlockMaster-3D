using UnityEngine;
using UnityEngine.SceneManagement;

public class NEWLEVEL : MonoBehaviour
{
    public static void LoadLevel()
    {
        Time.timeScale = 1f;

        int nextScene =
            SceneManager.GetActiveScene()
                .buildIndex + 1;

        if (
            nextScene >=
            SceneManager.sceneCountInBuildSettings
        )
        {
            Debug.Log(
                "There is no next level."
            );

            return;
        }

        SceneManager.LoadScene(
            nextScene
        );
    }
}