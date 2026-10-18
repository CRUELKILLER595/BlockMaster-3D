using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private AudioSource audioSource;
    private string currentSceneName;
    private bool initialized;

    public AudioClip menuMusic;
    public AudioClip[] levelMusic;

    [Header("Menu Scenes")]
    [SerializeField]
    private string[] menuSceneNames =
    {
        "game",
        "MainMenu",
        "LEVELS"
    };

    void Awake()
    {
        if (
            instance != null &&
            instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        audioSource =
            GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }

    void Start()
    {
        OnSceneLoaded(
            SceneManager.GetActiveScene(),
            LoadSceneMode.Single
        );
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }

    void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        if (audioSource == null)
        {
            audioSource =
                GetComponent<AudioSource>();
        }

        // =====================================================
        // MENU
        // =====================================================

        if (IsMenuScene(scene.name))
        {
            bool sameMenuMusic =
                initialized &&
                audioSource.clip == menuMusic &&
                audioSource.isPlaying;

            if (!sameMenuMusic)
            {
                PlayMusic(
                    menuMusic,
                    false
                );
            }
        }
        else
        {
            // =================================================
            // LEVEL
            // =================================================

            int levelIndex =
                ExtractLevelIndex(scene);

            if (
                levelIndex >= 0 &&
                levelIndex < levelMusic.Length
            )
            {
                // FORCE restart.
                PlayMusic(
                    levelMusic[levelIndex],
                    true
                );
            }
            else
            {
                Debug.LogWarning(
                    "MusicManager: No music for scene " +
                    scene.name
                );
            }
        }

        currentSceneName =
            scene.name;

        initialized = true;
    }

    bool IsMenuScene(
        string sceneName
    )
    {
        if (
            string.IsNullOrEmpty(
                sceneName
            )
        )
        {
            return false;
        }

        return menuSceneNames.Contains(
            sceneName
        );
    }

    void PlayMusic(
        AudioClip clip,
        bool forceRestart
    )
    {
        if (clip == null)
        {
            Debug.LogWarning(
                "MusicManager: Missing music clip."
            );

            return;
        }

        // Don't restart menu music unnecessarily.
        if (
            !forceRestart &&
            audioSource.clip == clip &&
            audioSource.isPlaying
        )
        {
            return;
        }

        audioSource.Stop();

        audioSource.clip = clip;
        audioSource.time = 0f;

        audioSource.Play();
    }

    int ExtractLevelIndex(
        Scene scene
    )
    {
        // Example:
        // Level 1  -> 0
        // Level 5  -> 4
        // SCENE 10 -> 9

        string digits =
            new string(
                scene.name
                    .Where(
                        char.IsDigit
                    )
                    .ToArray()
            );

        if (
            int.TryParse(
                digits,
                out int levelNumber
            )
        )
        {
            return levelNumber - 1;
        }

        // Build index fallback.
        return scene.buildIndex - 2;
    }

    public void PauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }
}