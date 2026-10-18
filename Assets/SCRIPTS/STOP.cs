using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class StopTimer : MonoBehaviour
{
    public float counter = 5f;
    public float decreaseRate = 1f;

    public TextMeshProUGUI counterText;

    public Transform player;
    private bool restartTriggered = false;
    public Vector3 offset =
        new Vector3(0, 2, 0);

    public float smoothSpeed = 15f;

    public GAMEMANAGER gm;

    private Rigidbody rb;
    private PlayerMovement playerMovement;

    private bool startedMonitoring = false;
    void Start()
    {
        if (player != null)
        {
            rb =
                player.GetComponent<Rigidbody>();

            playerMovement =
                player.GetComponent<
                    PlayerMovement
                >();
        }

        if (gm == null)
        {
            gm =
                FindFirstObjectByType<
                    GAMEMANAGER
                >();
        }
    }

    void Update()
    {
        if (counterText != null)
        {
            counterText.text =
                counter.ToString("F1");
        }

        // Don't monitor before the run has started.
        if (
            playerMovement != null &&
            !playerMovement.HasStartedRunning
        )
        {
            return;
        }

        if (!startedMonitoring)
        {
            startedMonitoring = true;
        }

        bool playerStopped =
            rb != null &&
            rb.IsSleeping();

        Keyboard keyboard =
            Keyboard.current;

        bool pressedS =
            keyboard != null &&
            keyboard.sKey.isPressed;

        if (
            pressedS ||
            playerStopped
        )
        {
            counter -=
                decreaseRate *
                Time.deltaTime *
                0.5f;
        }

        if (
            counter <= 0f &&
            !restartTriggered
        )
        {
            counter = 0f;

            restartTriggered = true;

            if (gm != null)
            {
                gm.EndGame();
            }
        }
    }

    void LateUpdate()
    {
        if (
            counterText == null ||
            player == null ||
            Camera.main == null
        )
        {
            return;
        }

        Vector3 target =
            Camera.main.WorldToScreenPoint(
                player.position + offset
            );

        counterText.transform.position =
            Vector3.Lerp(
                counterText.transform.position,
                target,
                Time.deltaTime *
                smoothSpeed
            );
    }
}