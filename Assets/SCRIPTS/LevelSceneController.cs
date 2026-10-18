
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelSceneController : MonoBehaviour
{
    [Tooltip("Exact gameplay scene name to load when Escape is pressed.")]
    [SerializeField] private string gameSceneName = "Level 1";

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null ||
            !keyboard.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("Gameplay scene name is not configured.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.LogError(
                "Cannot load scene '" + gameSceneName +
                "'. Check its name and Build Profiles."
            );
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }
}
