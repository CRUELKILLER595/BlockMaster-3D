
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelsSceneEscape : MonoBehaviour
{
    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null ||
            !keyboard.escapeKey.wasPressedThisFrame)
            return;

        Time.timeScale = 1f;

        SceneManager.LoadScene("game");
    }
}
