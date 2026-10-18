using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.UnityConsent;

public class Analytics : MonoBehaviour
{
    public static Analytics Instance { get; private set; }

    [Header("Initialization")]
    [SerializeField] private bool initializeOnAwake = true;

    [Tooltip("Testing only. Keep OFF for release builds.")]
    [SerializeField] private bool grantConsentForTesting = false;

    [Header("Level Mapping")]
    [Tooltip("Build Index of Level 1.")]
    [SerializeField] private int firstLevelBuildIndex = 2;

    private bool initialized;
    private bool analyticsConsentGranted;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (initializeOnAwake)
        {
            await InitializeAnalytics();
        }
    }

    private async System.Threading.Tasks.Task InitializeAnalytics()
    {
        try
        {
            await UnityServices.InitializeAsync();

            initialized = true;

            ConsentState consentState =
                EndUserConsent.GetConsentState();

            analyticsConsentGranted =
                consentState.AnalyticsIntent ==
                ConsentStatus.Granted;

            Debug.Log(
                "Unity Services initialized. Analytics consent: " +
                analyticsConsentGranted
            );

            if (grantConsentForTesting)
            {
                GrantAnalyticsConsent();
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Unity Services initialization failed: " +
                exception
            );
        }
    }

  

    public void GrantAnalyticsConsent()
    {
        ConsentState consentState =
            EndUserConsent.GetConsentState();

        consentState.AnalyticsIntent =
            ConsentStatus.Granted;

        EndUserConsent.SetConsentState(
            consentState
        );

        analyticsConsentGranted = true;

        Debug.Log("Analytics consent granted.");
    }

    public void RevokeAnalyticsConsent()
    {
        ConsentState consentState =
            EndUserConsent.GetConsentState();

        consentState.AnalyticsIntent =
            ConsentStatus.Denied;

        EndUserConsent.SetConsentState(
            consentState
        );

        analyticsConsentGranted = false;

        Debug.Log("Analytics consent revoked.");
    }

    public bool IsAnalyticsReady()
    {
        return initialized &&
               analyticsConsentGranted;
    }

   

    public int GetCurrentLevel()
    {
        int level =
            SceneManager.GetActiveScene().buildIndex -
            firstLevelBuildIndex +
            1;

        return Mathf.Max(1, level);
    }

 

    private void RecordEvent(
        string eventName,
        CustomEvent customEvent
    )
    {
        if (!initialized)
        {
            Debug.LogWarning(
                "Analytics not initialized. Event skipped: " +
                eventName
            );

            return;
        }

        if (!analyticsConsentGranted)
        {
            Debug.Log(
                "Analytics skipped because consent was not granted: " +
                eventName
            );

            return;
        }

        try
        {
            AnalyticsService.Instance.RecordEvent(
                customEvent
            );
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Analytics event failed (" +
                eventName +
                "): " +
                exception
            );
        }
    }


    public void LevelStarted(int level)
    {
        RecordEvent(
            "level_started",
            new CustomEvent("level_started")
            {
                { "level", level }
            }
        );
    }

    public void LevelFailed(
        int level,
        float distance,
        int coins
    )
    {
        RecordEvent(
            "level_failed",
            new CustomEvent("level_failed")
            {
                { "level", level },
                { "distance", distance },
                { "coins", coins }
            }
        );
    }

    public void LevelCompleted(
        int level,
        float distance,
        int coins,
        int score
    )
    {
        RecordEvent(
            "level_completed",
            new CustomEvent("level_completed")
            {
                { "level", level },
                { "distance", distance },
                { "coins", coins },
                { "score", score }
            }
        );
    }

   

    public void PowerupCollected(
        int level,
        string powerup,
        int amount = 1
    )
    {
        RecordEvent(
            "powerup_collected",
            new CustomEvent("powerup_collected")
            {
                { "level", level },
                { "powerup", powerup },
                { "amount", amount }
            }
        );
    }

    public void PowerupUsed(
        int level,
        string powerup
    )
    {
        RecordEvent(
            "powerup_used",
            new CustomEvent("powerup_used")
            {
                { "level", level },
                { "powerup", powerup }
            }
        );
    }

 

    public void StrikeActivated(
        int level,
        int remainingStrikes
    )
    {
        RecordEvent(
            "strike_activated",
            new CustomEvent("strike_activated")
            {
                { "level", level },
                {
                    "remaining_strikes",
                    remainingStrikes
                }
            }
        );
    }

    public void ObstacleDestroyed(
        int level,
        string obstacleType = "Obstacle"
    )
    {
        RecordEvent(
            "obstacle_destroyed",
            new CustomEvent("obstacle_destroyed")
            {
                { "level", level },
                { "obstacle_type", obstacleType }
            }
        );
    }
}