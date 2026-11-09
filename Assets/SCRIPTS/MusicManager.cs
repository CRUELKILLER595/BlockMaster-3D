using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    public static MusicManager Instance => instance;

    private AudioSource audioSource;
    private bool initialized;

    public AudioClip menuMusic;
    public AudioClip[] levelMusic;

    private const string SettingsSceneName = "SETTINGS";
    private const string MusicVolumeKey = "BM_MusicVolume";
    private const string MusicEnabledKey = "BM_MusicEnabled";

    [Header("Menu Scenes")]
    [SerializeField]
    private string[] menuSceneNames =
    {
        "game",
        "MainMenu",
        "LEVELS"
    };

    public float CurrentVolume =>
        audioSource != null ? audioSource.volume : 1f;

    public bool IsMusicEnabled =>
        audioSource != null && !audioSource.mute;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        audioSource.volume =
            PlayerPrefs.GetFloat(MusicVolumeKey, audioSource.volume);

        audioSource.mute =
            PlayerPrefs.GetInt(MusicEnabledKey, 1) == 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (!initialized)
        {
            OnSceneLoaded(
                SceneManager.GetActiveScene(),
                LoadSceneMode.Single
            );
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Settings must never restart existing music.
        if (scene.name == SettingsSceneName)
            return;

        if (IsMenuScene(scene.name))
        {
            bool sameMenuMusic =
                initialized &&
                audioSource.clip == menuMusic &&
                audioSource.isPlaying;

            if (!sameMenuMusic)
                PlayMusic(menuMusic, false);
        }
        else
        {
            int levelIndex = ExtractLevelIndex(scene);

            if (levelMusic != null &&
                levelIndex >= 0 &&
                levelIndex < levelMusic.Length)
            {
                PlayMusic(levelMusic[levelIndex], true);
            }
            else
            {
                Debug.LogWarning(
                    "MusicManager: No music assigned for " + scene.name
                );
            }
        }

        initialized = true;
    }

    private bool IsMenuScene(string sceneName)
    {
        return !string.IsNullOrEmpty(sceneName) &&
               menuSceneNames.Contains(sceneName);
    }

    private void PlayMusic(AudioClip clip, bool forceRestart)
    {
        if (clip == null)
        {
            Debug.LogWarning("MusicManager: Missing music clip.");
            return;
        }

        if (!forceRestart &&
            audioSource.clip == clip &&
            audioSource.isPlaying)
        {
            return;
        }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.time = 0f;
        audioSource.Play();
    }

    private int ExtractLevelIndex(Scene scene)
    {
        string digits = new string(
            scene.name.Where(char.IsDigit).ToArray()
        );

        if (int.TryParse(digits, out int levelNumber))
            return levelNumber - 1;

        return scene.buildIndex - 2;
    }

    public void PauseMusic()
    {
        if (audioSource != null)
            audioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (audioSource != null)
            audioSource.UnPause();
    }

    public void SetMusicVolume(float volume)
    {
        if (audioSource == null)
            return;

        audioSource.volume = Mathf.Clamp01(volume);

        PlayerPrefs.SetFloat(MusicVolumeKey, audioSource.volume);
        PlayerPrefs.Save();
    }

    public void SetMusicEnabled(bool enabled)
    {
        if (audioSource == null)
            return;

        audioSource.mute = !enabled;

        PlayerPrefs.SetInt(MusicEnabledKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
