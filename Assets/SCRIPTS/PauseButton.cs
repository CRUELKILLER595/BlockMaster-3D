
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseButton : MonoBehaviour
{
    [Header("Button Reference")]
    public Button pauseButton;

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Gameplay UI to Hide")]
    public GameObject[] Texts;

    private bool[] previousTextStates;
    private bool isPaused = false;
    private MusicManager musicManager;

    private void Awake()
    {
        musicManager = FindFirstObjectByType<MusicManager>();
    }

    private void Start()
    {
        // Ensure the scene does not inherit a paused time scale.
        Time.timeScale = 1f;

        if (pauseButton == null)
        {
            Debug.LogError(
                "PauseButton: Assign the Pause Button in the Inspector."
            );
        }
        else
        {
            pauseButton.onClick.RemoveListener(TogglePause);
            pauseButton.onClick.AddListener(TogglePause);
        }

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        else
            Debug.LogError("PauseButton: Pause Menu is not assigned.");
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null &&
            keyboard.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeLevel();
        else
            PauseLevel();
    }

    public void PauseLevel()
    {
        if (isPaused)
            return;

        // Save visibility and hide gameplay UI.
        HideGameplayTexts();

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        if (musicManager != null)
            musicManager.PauseMusic();

        Debug.Log("Game paused.");
    }

    public void ResumeLevel()
    {
        if (!isPaused)
            return;

        isPaused = false;

        // Restore time before resuming gameplay.
        Time.timeScale = 1f;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        // Restore each object to its previous active state.
        RestoreGameplayTexts();

        if (musicManager != null)
            musicManager.ResumeMusic();

        Debug.Log("Game resumed.");
    }

    private void HideGameplayTexts()
    {
        if (Texts == null)
            return;

        previousTextStates = new bool[Texts.Length];

        for (int i = 0; i < Texts.Length; i++)
        {
            if (Texts[i] == null)
                continue;

            previousTextStates[i] =
                Texts[i].activeSelf;

            Texts[i].SetActive(false);
        }
    }

    private void RestoreGameplayTexts()
    {
        if (Texts == null || previousTextStates == null)
            return;

        for (int i = 0; i < Texts.Length; i++)
        {
            if (Texts[i] == null)
                continue;

            Texts[i].SetActive(previousTextStates[i]);
        }

        previousTextStates = null;
    }

    private void OnDestroy()
    {
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(TogglePause);

        // Prevent a paused time scale from leaking into another scene.
        Time.timeScale = 1f;
    }
    public void RestartLevel()
    {
        RestoreGameplayTexts(); // Restore UI before restarting
        isPaused = false; // Reset pause state
       Time.timeScale = 1f; // Ensure time scale is reset before restarting
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
