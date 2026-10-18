using UnityEngine;
using TMPro;

public class TIMER : MonoBehaviour
{
    public float LevelTime;
    public TMP_Text timertext;
    public GAMEMANAGER gm;

    private bool timerEnabled = false;

    void Start()
    {
        timerEnabled = true;

        if (gm == null)
        {
            gm =
                FindFirstObjectByType<
                    GAMEMANAGER
                >();
        }

        UpdateTimerUI();
    }

    void Update()
    {
        if (!timerEnabled)
            return;

        LevelTime -=
            Time.deltaTime;

        if (
            LevelTime <= 10f &&
            LevelTime > 0f
        )
        {
            if (timertext != null)
            {
                timertext.color =
                    Color.red;
            }
        }

        if (LevelTime <= 0f)
        {
            LevelTime = 0f;

            timerEnabled = false;

            UpdateTimerUI();

            if (gm != null)
            {
                gm.EndGame();
            }

            return;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timertext != null)
        {
            timertext.text =
                Mathf.CeilToInt(
                    LevelTime
                ).ToString();
        }
    }

    public void StopTimer()
    {
        timerEnabled = false;
    }
}