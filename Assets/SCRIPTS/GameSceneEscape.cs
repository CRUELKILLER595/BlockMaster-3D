
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSceneEscape : MonoBehaviour
{
    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null ||
            !keyboard.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        Debug.Log("Exiting BlockMaster 3D.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
