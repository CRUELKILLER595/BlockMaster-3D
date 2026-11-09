
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    [Header("Button Reference")]
    public Button pauseButton;

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Gameplay UI to Hide")]
    public GameObject[] Texts;

    private bool[] previousTextStates;
    private bool isPaused;

    private MusicManager musicManager;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        musicManager = MusicManager.Instance;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveListener(TogglePause);
            pauseButton.onClick.AddListener(TogglePause);
        }
        else
        {
            Debug.LogError("PauseButton: Pause Button is not assigned.");
        }

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        else
            Debug.LogError("PauseButton: Pause Menu is not assigned.");
    }

    private void Update()
    {
        if (SceneManager.GetSceneByName(
            SettingsSceneController.SettingsSceneName).isLoaded)
        {
            return;
        }

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

        HideGameplayTexts();

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        if (musicManager == null)
            musicManager = MusicManager.Instance;

        if (musicManager != null)
            musicManager.PauseMusic();

        Debug.Log("Game paused.");
    }

    public void ResumeLevel()
    {
        if (!isPaused)
            return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        RestoreGameplayTexts();

        if (musicManager == null)
            musicManager = MusicManager.Instance;

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

            previousTextStates[i] = Texts[i].activeSelf;
            Texts[i].SetActive(false);
        }
    }

    private void RestoreGameplayTexts()
    {
        if (Texts == null || previousTextStates == null)
            return;

        for (int i = 0; i < Texts.Length; i++)
        {
            if (Texts[i] != null)
                Texts[i].SetActive(previousTextStates[i]);
        }

        previousTextStates = null;
    }

    public void RestartLevel()
    {
        RestoreGameplayTexts();

        isPaused = false;
        Time.timeScale = 1f;

        Scene activeScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(
            activeScene.buildIndex,
            LoadSceneMode.Single
        );
    }

    // Connect the Pause Menu Settings button to this method.
    public void SettingScene()
    {
        if (!isPaused)
            PauseLevel();

        SettingsSceneController.OpenSettings();
    }

    private void OnDestroy()
    {
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(TogglePause);

        // The source scene is deactivated, not destroyed, when
        // entering Settings. Therefore this does not reset its pause state.
        Time.timeScale = 1f;
    }
}
