
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsBackButton : MonoBehaviour
{
    public void GoBackToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("game");
    }
}
