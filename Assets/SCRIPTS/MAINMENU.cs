using UnityEngine;
using UnityEngine.SceneManagement;

public class MAINMENU : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    


    public static void PlayGame()
    {
        Time.timeScale = 1f;
        // Load next scene (make sure build settings are correct)
        SceneManager.LoadScene("LEVELS");
    }

    public static void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public static void OpenSettings()
    {
        Debug.Log("Settings Menu Clicked");
        // Enable settings panel if you create one
    }
}
