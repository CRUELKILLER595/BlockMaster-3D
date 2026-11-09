
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsSceneController : MonoBehaviour
{
    public const string SettingsSceneName = "SETTINGS";

    private const string MusicVolumeKey = "BM_MusicVolume";
    private const string SFXVolumeKey = "BM_SFXVolume";
    private const string BrightnessKey = "BM_Brightness";
    private const string MusicEnabledKey = "BM_MusicEnabled";
    private const string SFXEnabledKey = "BM_SFXEnabled";

    [Header("Volume Sliders")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Brightness Slider")]
    public Slider brightnessSlider;

    [Header("Audio Toggles")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("Brightness Volume")]
    public Volume brightnessVolume;

    [Header("Brightness Range")]
    public float minExposure = -2f;
    public float maxExposure = 2f;

    private MusicManager musicManager;
    private SFXManager sfxManager;
    private ColorAdjustments colorAdjustments;

    // State retained while SETTINGS is loaded.
    private static Scene previousScene;

    private static GameObject[] previousRootObjects;
    private static bool[] previousRootActiveStates;

    private static bool transitionInProgress;

    // The brightness Volume survives Settings scene unloading.
    private static Volume persistentBrightnessVolume;

    // ==================================================
    // OPEN SETTINGS
    // ==================================================

    public static void OpenSettings()
    {
        if (transitionInProgress)
            return;

        Scene existingSettings =
            SceneManager.GetSceneByName(SettingsSceneName);

        if (existingSettings.isLoaded)
            return;

        Scene sourceScene = SceneManager.GetActiveScene();

        if (!sourceScene.IsValid() ||
            sourceScene.name == SettingsSceneName)
        {
            Debug.LogError("Settings: Invalid source scene.");
            return;
        }

        previousScene = sourceScene;

        previousRootObjects = sourceScene.GetRootGameObjects();

        previousRootActiveStates =
            new bool[previousRootObjects.Length];

        // Capture the original active states.
        for (int i = 0; i < previousRootObjects.Length; i++)
        {
            previousRootActiveStates[i] =
                previousRootObjects[i].activeSelf;
        }

        // Completely suspend the source scene.
        // DontDestroyOnLoad managers are not included here.
        for (int i = 0; i < previousRootObjects.Length; i++)
        {
            if (previousRootObjects[i] != null)
                previousRootObjects[i].SetActive(false);
        }

        transitionInProgress = true;

        try
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(
                SettingsSceneName,
                LoadSceneMode.Additive
            );

            if (operation == null)
            {
                Debug.LogError("Settings: Loading failed.");

                RestorePreviousScene();
                transitionInProgress = false;
                return;
            }

            operation.completed += _ =>
            {
                Scene settingsScene =
                    SceneManager.GetSceneByName(SettingsSceneName);

                if (settingsScene.IsValid() &&
                    settingsScene.isLoaded)
                {
                    SceneManager.SetActiveScene(settingsScene);

                    Debug.Log(
                        "Settings loaded. Previous scene suspended."
                    );
                }
                else
                {
                    Debug.LogError("Settings scene failed to load.");
                    RestorePreviousScene();
                }

                transitionInProgress = false;
            };
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);

            RestorePreviousScene();
            transitionInProgress = false;
        }
    }

    // ==================================================
    // INITIALIZATION
    // ==================================================

    private void Awake()
    {
        musicManager = MusicManager.Instance;

        if (musicManager == null)
            musicManager = FindFirstObjectByType<MusicManager>();

        sfxManager = SFXManager.instance;

        if (sfxManager == null)
            sfxManager = FindFirstObjectByType<SFXManager>();

        InitializeBrightnessVolume();
    }

    private void Start()
    {
        InitializeMusicControls();
        InitializeSFXControls();
        InitializeBrightnessControl();
    }

    // ==================================================
    // MUSIC SETTINGS
    // ==================================================

    private void InitializeMusicControls()
    {
        float volume = PlayerPrefs.GetFloat(
            MusicVolumeKey,
            musicManager != null ? musicManager.CurrentVolume : 1f
        );

        bool enabled =
            PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;

            musicVolumeSlider.SetValueWithoutNotify(volume);

            musicVolumeSlider.onValueChanged.AddListener(
                OnMusicVolumeChanged
            );
        }

        if (musicToggle != null)
        {
            musicToggle.SetIsOnWithoutNotify(enabled);

            musicToggle.onValueChanged.AddListener(
                OnMusicToggleChanged
            );
        }

        if (musicManager != null)
        {
            musicManager.SetMusicVolume(volume);
            musicManager.SetMusicEnabled(enabled);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (musicManager != null)
            musicManager.SetMusicVolume(value);
    }

    private void OnMusicToggleChanged(bool enabled)
    {
        if (musicManager != null)
            musicManager.SetMusicEnabled(enabled);
    }

    // ==================================================
    // SFX SETTINGS
    // ==================================================

    private void InitializeSFXControls()
    {
        float volume = PlayerPrefs.GetFloat(
            SFXVolumeKey,
            sfxManager != null ? sfxManager.CurrentVolume : 1f
        );

        bool enabled =
            PlayerPrefs.GetInt(SFXEnabledKey, 1) == 1;

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;

            sfxVolumeSlider.SetValueWithoutNotify(volume);

            sfxVolumeSlider.onValueChanged.AddListener(
                OnSFXVolumeChanged
            );
        }

        if (sfxToggle != null)
        {
            sfxToggle.SetIsOnWithoutNotify(enabled);

            sfxToggle.onValueChanged.AddListener(
                OnSFXToggleChanged
            );
        }

        if (sfxManager != null)
        {
            sfxManager.SetSFXVolume(volume);
            sfxManager.SetSFXEnabled(enabled);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (sfxManager != null)
            sfxManager.SetSFXVolume(value);
    }

    private void OnSFXToggleChanged(bool enabled)
    {
        if (sfxManager != null)
            sfxManager.SetSFXEnabled(enabled);
    }

    // ==================================================
    // BRIGHTNESS SETTINGS
    // ==================================================

    private void InitializeBrightnessVolume()
    {
        if (persistentBrightnessVolume != null)
        {
            // Reuse the Volume that survived the previous Settings visit.
            if (brightnessVolume != null &&
                brightnessVolume != persistentBrightnessVolume)
            {
                Destroy(brightnessVolume.gameObject);
            }

            brightnessVolume = persistentBrightnessVolume;
        }
        else
        {
            if (brightnessVolume == null)
            {
                Debug.LogError(
                    "Settings: Brightness Volume is not assigned."
                );

                return;
            }

            if (brightnessVolume.transform.parent != null)
            {
                Debug.LogError(
                    "Settings: Brightness Volume must be a root object."
                );

                return;
            }

            persistentBrightnessVolume = brightnessVolume;

            DontDestroyOnLoad(brightnessVolume.gameObject);
        }

        brightnessVolume.isGlobal = true;
        brightnessVolume.priority = 1000f;
        brightnessVolume.weight = 1f;

        VolumeProfile runtimeProfile = brightnessVolume.profile;

        if (!runtimeProfile.TryGet(out colorAdjustments))
        {
            colorAdjustments =
                runtimeProfile.Add<ColorAdjustments>(true);
        }

        colorAdjustments.active = true;
        colorAdjustments.postExposure.overrideState = true;
    }

    private void InitializeBrightnessControl()
    {
        float brightness = PlayerPrefs.GetFloat(
            BrightnessKey,
            0.5f
        );

        if (brightnessSlider != null)
        {
            brightnessSlider.minValue = 0f;
            brightnessSlider.maxValue = 1f;

            brightnessSlider.SetValueWithoutNotify(brightness);

            brightnessSlider.onValueChanged.AddListener(
                OnBrightnessChanged
            );
        }

        ApplyBrightness(brightness);
    }

    private void OnBrightnessChanged(float value)
    {
        ApplyBrightness(value);

        PlayerPrefs.SetFloat(BrightnessKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyBrightness(float value)
    {
        if (colorAdjustments == null)
            return;

        float exposure = Mathf.Lerp(
            minExposure,
            maxExposure,
            Mathf.Clamp01(value)
        );

        colorAdjustments.postExposure.value = exposure;
    }

    // ==================================================
    // BACK BUTTON
    // ==================================================

    public void Back()
    {
        if (transitionInProgress)
            return;

        Scene settingsScene =
            SceneManager.GetSceneByName(SettingsSceneName);

        if (!settingsScene.isLoaded)
            return;

        transitionInProgress = true;

        AsyncOperation operation =
            SceneManager.UnloadSceneAsync(settingsScene);

        if (operation == null)
        {
            transitionInProgress = false;

            Debug.LogError("Settings: Could not unload scene.");
            return;
        }

        operation.completed += _ =>
        {
            RestorePreviousScene();

            transitionInProgress = false;

            Debug.Log("Previous scene restored.");
        };
    }

    // ==================================================
    // RESTORE PREVIOUS SCENE
    // ==================================================

    private static void RestorePreviousScene()
    {
        if (previousScene.IsValid() && previousScene.isLoaded)
        {
            SceneManager.SetActiveScene(previousScene);

            if (previousRootObjects != null &&
                previousRootActiveStates != null)
            {
                int count = Mathf.Min(
                    previousRootObjects.Length,
                    previousRootActiveStates.Length
                );

                for (int i = 0; i < count; i++)
                {
                    if (previousRootObjects[i] != null)
                    {
                        previousRootObjects[i].SetActive(
                            previousRootActiveStates[i]
                        );
                    }
                }
            }
        }
        else
        {
            Debug.LogError(
                "Settings: Previous scene is no longer loaded."
            );
        }

        previousScene = default;

        previousRootObjects = null;
        previousRootActiveStates = null;
    }

    // ==================================================
    // CLEANUP
    // ==================================================

    private void OnDestroy()
    {
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(
                OnMusicVolumeChanged
            );

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(
                OnSFXVolumeChanged
            );

        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.RemoveListener(
                OnBrightnessChanged
            );

        if (musicToggle != null)
            musicToggle.onValueChanged.RemoveListener(
                OnMusicToggleChanged
            );

        if (sfxToggle != null)
            sfxToggle.onValueChanged.RemoveListener(
                OnSFXToggleChanged
            );
    }
}
