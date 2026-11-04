
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public GAMEMANAGER gm;
    private Rigidbody rb;
    public AudioClip jumpSound;

    [Header("Movement Mode")]
    public bool puzzleMode = false;

    // =========================================================
    // UI
    // =========================================================
    [Header("Leap Point")]
public float leapPointVerticalForce = 5f;
public float leapPointForwardSpeed = 3f;

private bool leapPointLaunchActive = false;

    [Header("UI")]
    public TMPro.TextMeshProUGUI InevitableText;
    public TMPro.TextMeshProUGUI TutorialText;
    public TMPro.TextMeshProUGUI strikeText;

    // =========================================================
    // MOVEMENT
    // =========================================================

    [Header("Movement")]
    public float speed = 10f;
    public float sideForce = 7f;
    public float backwardForce = -7f;
    public float maxSpeed = 15f;

    [Header("Auto Run")]
    public float autoRunSpeed = 10f;
    public float maxSideSpeed = 10f;

    [Header("Lateral Movement")]
    public float horizontalDeceleration = 15f;

   

    // =========================================================
    // MOVEMENT BOUNDS
    // =========================================================

    [Header("Movement Bounds")]
    public float startX = -5f;
    public float endX = 5f;

    // =========================================================
    // RUN STATE
    // =========================================================

    private bool hasStartedRunning = false;

    public bool HasStartedRunning
    {
        get { return hasStartedRunning; }
    }

    // =========================================================
    // INEVITABLE
    // =========================================================

    [Header("Inevitable")]
    public bool isInevitable = false;

    private Coroutine inevitableRoutine;

    // =========================================================
    // STRIKE
    // =========================================================

    [Header("Strike")]
    public int strikeCount = 0;
    public bool isStriking = false;

    public float strikeDuration = 0.5f;
    public float strikeForce = 12f;
    public float strikeSpeed = 18f;

    private Coroutine strikeRoutine;

    // =========================================================
    // JUMP
    // =========================================================

    [Header("Jump")]
    public float jumpForce = 7f;

    private bool canJump = false;
    private bool isGrounded = false;

    private Coroutine jumpRoutine;

    // =========================================================
    // ABILITIES
    // =========================================================

    [Header("Abilities")]
    public bool canMoveBackward = false;

    public JumpPowerUpUI jumpUI;

    public static PlayerMovement instance;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();

        if (gm == null)
        {
            gm = FindFirstObjectByType<GAMEMANAGER>();
        }
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        hasStartedRunning = false;

        UpdateStrikeUI();

        if (rb != null)
        {
            Vector3 velocity = rb.linearVelocity;

            velocity.x = 0f;
            velocity.z = 0f;

            rb.linearVelocity = velocity;
            rb.WakeUp();
        }

        StartCoroutine(RecordLevelStartedWhenReady());
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        // START RUN
        if (!hasStartedRunning)
        {
            if (
                keyboard.wKey.wasPressedThisFrame ||
                keyboard.upArrowKey.wasPressedThisFrame ||
                keyboard.aKey.wasPressedThisFrame ||
                keyboard.leftArrowKey.wasPressedThisFrame ||
                keyboard.dKey.wasPressedThisFrame ||
                keyboard.rightArrowKey.wasPressedThisFrame
            )
            {
                hasStartedRunning = true;

                if (rb != null)
                    rb.WakeUp();

                Debug.Log("PLAYER STARTED RUNNING");
            }
            else
            {
                return;
            }
        }

        // STRIKE INPUT
        if (
            strikeCount > 0 &&
            !isStriking &&
            keyboard.xKey.wasPressedThisFrame
        )
        {
            ActivateStrike();
        }

        // JUMP INPUT
        if (
            keyboard.spaceKey.wasPressedThisFrame &&
            canJump &&
            isGrounded
        )
        {
            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );
            SFXManager.instance.PlaySound(jumpSound);
        }
    }

    // =========================================================
    // FIXED UPDATE
    // =========================================================

    private void FixedUpdate()
    {
        // PuzzleMovement takes over horizontal movement.
        if (puzzleMode)
            return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        // WAIT FOR PLAYER TO START
        if (!hasStartedRunning)
        {
            Vector3 stoppedVelocity = rb.linearVelocity;

            stoppedVelocity.x = 0f;
            stoppedVelocity.z = 0f;

            rb.linearVelocity = stoppedVelocity;
            return;
        }

        bool backwardPressed =
            keyboard.sKey.isPressed ||
            keyboard.downArrowKey.isPressed;

        bool leftPressed =
            keyboard.aKey.isPressed ||
            keyboard.leftArrowKey.isPressed;

        bool rightPressed =
            keyboard.dKey.isPressed ||
            keyboard.rightArrowKey.isPressed;

        // CONTINUOUS FORWARD MOVEMENT
        if(!leapPointLaunchActive)
        {
            rb.AddForce(
                Vector3.forward * autoRunSpeed,
                ForceMode.Acceleration
            );
        }

        // BACKWARD MOVEMENT
        if (
            backwardPressed &&
            canMoveBackward &&
            !isStriking
        )
        {
            rb.AddForce(
                Vector3.forward * backwardForce,
                ForceMode.Acceleration
            );
        }

        // LEFT MOVEMENT
        if (leftPressed)
        {
            rb.AddForce(
                Vector3.left * sideForce,
                ForceMode.Acceleration
            );
        }

        // RIGHT MOVEMENT
        if (rightPressed)
        {
            rb.AddForce(
                Vector3.right * sideForce,
                ForceMode.Acceleration
            );
        }
       
        // VELOCITY CONTROL
        Vector3 velocity = rb.linearVelocity;
        if (!leftPressed && !rightPressed)
{
   

    velocity.x = Mathf.MoveTowards(
        velocity.x,
        0f,
        horizontalDeceleration * Time.fixedDeltaTime
    );

    
}

        if (isStriking)
        {
            velocity.z = Mathf.Max(
                velocity.z,
                strikeSpeed
            );
        }
        else if (backwardPressed && canMoveBackward)
        {
            velocity.z = Mathf.Min(
                velocity.z,
                0f
            );
        }
        else
        {
            velocity.z = Mathf.Max(
                velocity.z,
                autoRunSpeed
            );
        }

        // SIDE SPEED LIMIT
        velocity.x = Mathf.Clamp(
            velocity.x,
            -maxSideSpeed,
            maxSideSpeed
        );

        // NORMAL SPEED LIMIT
        if (!isStriking)
        {
            Vector3 horizontalVelocity = new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

            horizontalVelocity = Vector3.ClampMagnitude(
                horizontalVelocity,
                maxSpeed
            );

            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;
        }

        rb.linearVelocity = velocity;

        // X BOUNDARY FAILURE CHECK
        if (
            transform.position.x < startX ||
            transform.position.x > endX
        )
        {
            Debug.Log("PLAYER LEFT THE PLAY AREA");

            if (gm != null)
                gm.EndGame();

            return;
        }

        // FALL FAILURE CHECK
        if (transform.position.y < -1f)
        {
            if (gm != null)
                gm.EndGame();
        }
    }

    // =========================================================
    // LEAP POINT LAUNCH
    // =========================================================

   public void LaunchFromLeapPoint()
{
    if (rb == null)
        rb = GetComponent<Rigidbody>();

    if (rb == null)
        return;

    // Prevent stacking the previous vertical momentum.
    Vector3 velocity = rb.linearVelocity;
    velocity.y = 0f;

    // Reduce existing forward momentum.
    velocity.z = Mathf.Max(0f, leapPointForwardSpeed);

    rb.linearVelocity = velocity;

    // Enter temporary launch mode.
    leapPointLaunchActive = true;

    // Apply vertical force only.
    rb.AddForce(
        Vector3.up * leapPointVerticalForce,
        ForceMode.Impulse
    );

    Debug.Log("LeapPoint: Vertical launch activated.");
}
    // =========================================================
    // STRIKE
    // =========================================================

    private void ActivateStrike()
    {
        isStriking = true;

        Vector3 velocity = rb.linearVelocity;

        velocity.z = Mathf.Max(
            velocity.z,
            strikeSpeed
        );

        rb.linearVelocity = velocity;

        rb.AddForce(
            transform.forward * strikeForce,
            ForceMode.Impulse
        );

        strikeCount--;

        UpdateStrikeUI();

        if (Analytics.Instance != null)
        {
            int level =
                Analytics.Instance.GetCurrentLevel();

            Analytics.Instance.PowerupUsed(
                level,
                "StrikeThrough"
            );

            Analytics.Instance.StrikeActivated(
                level,
                strikeCount
            );
        }

        if (strikeRoutine != null)
            StopCoroutine(strikeRoutine);

        strikeRoutine = StartCoroutine(StrikeWindow());
    }

    private IEnumerator StrikeWindow()
    {
        float timer = strikeDuration;

        while (timer > 0f)
        {
            MaintainStrikeMomentum();

            timer -= Time.deltaTime;
            yield return null;
        }

        isStriking = false;
        strikeRoutine = null;
    }

    public void MaintainStrikeMomentum()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.z = Mathf.Max(
            velocity.z,
            strikeSpeed
        );

        rb.linearVelocity = velocity;
    }

    // =========================================================
    // STRIKE COUNT
    // =========================================================

    public void AddStrike(int amount)
    {
        strikeCount += amount;
        UpdateStrikeUI();
    }

    private void UpdateStrikeUI()
    {
        if (strikeText == null)
            return;

        if (strikeCount > 0)
        {
            strikeText.gameObject.SetActive(true);

            if (TutorialText != null)
            {
                TutorialText.gameObject.SetActive(true);
                TutorialText.text = "Press X to Strike!";
            }

            strikeText.text = "Strike: " + strikeCount;
        }
        else
        {
            strikeText.gameObject.SetActive(false);

            if (TutorialText != null)
                TutorialText.gameObject.SetActive(false);
        }
    }

    // =========================================================
    // GROUND AND ENEMY COLLISIONS
    // =========================================================

    private void OnCollisionEnter(Collision collision)
    {
        if (
            collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("GROUND")
        )
        {
            isGrounded = true;
        }

        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (
            collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("GROUND")
        )
        {
            isGrounded = false;
        }
    }

    // =========================================================
    // JUMP BOOST
    // =========================================================

    public void ActivateJumpBoost(float duration)
    {
        if (Analytics.Instance != null)
        {
            Analytics.Instance.PowerupUsed(
                Analytics.Instance.GetCurrentLevel(),
                "JumpPowerUp"
            );
        }

        if (jumpRoutine != null)
            StopCoroutine(jumpRoutine);

        if (Analytics.Instance != null)
        {
            Analytics.Instance.PowerupUsed(
                Analytics.Instance.GetCurrentLevel(),
                "JumpBoost"
            );
        }

        jumpRoutine = StartCoroutine(JumpBoost(duration));
    }

    private IEnumerator JumpBoost(float duration)
    {
        canJump = true;

        float timer = duration;

        while (timer > 0f)
        {
            if (jumpUI != null)
                jumpUI.ShowTimer(timer);

            timer -= Time.deltaTime;
            yield return null;
        }

        canJump = false;

        if (jumpUI != null)
            jumpUI.HideTimer();

        jumpRoutine = null;
    }

    // =========================================================
    // INEVITABLE
    // =========================================================

    public void EnableInevitable(float duration)
    {
        if (Analytics.Instance != null)
        {
            Analytics.Instance.PowerupUsed(
                Analytics.Instance.GetCurrentLevel(),
                "Inevitable"
            );
        }

        if (inevitableRoutine != null)
            StopCoroutine(inevitableRoutine);

        isInevitable = true;

        SetEnemyCollisionsIgnored(true);

        if (InevitableText != null)
            InevitableText.gameObject.SetActive(true);

        if (Analytics.Instance != null)
        {
            Analytics.Instance.PowerupUsed(
                Analytics.Instance.GetCurrentLevel(),
                "Inevitable"
            );
        }

        inevitableRoutine =
            StartCoroutine(InevitableTimer(duration));
    }

    private IEnumerator InevitableTimer(float duration)
    {
        float timer = duration;

        while (timer > 0f)
        {
            if (InevitableText != null)
            {
                InevitableText.text =
                    "Inevitable: " +
                    Mathf.CeilToInt(timer);
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        isInevitable = false;

        SetEnemyCollisionsIgnored(false);

        if (InevitableText != null)
            InevitableText.gameObject.SetActive(false);

        inevitableRoutine = null;
    }

    private void SetEnemyCollisionsIgnored(bool ignore)
    {
        ENEMY[] enemies =
            FindObjectsByType<ENEMY>(
                FindObjectsSortMode.None
            );

        foreach (ENEMY enemy in enemies)
        {
            if (enemy != null)
                enemy.SetPlayerCollisionIgnored(ignore);
        }
    }

    // =========================================================
    // ANALYTICS
    // =========================================================

    private IEnumerator RecordLevelStartedWhenReady()
    {
        float timeout = 5f;

        while (timeout > 0f)
        {
            if (
                Analytics.Instance != null &&
                Analytics.Instance.IsAnalyticsReady()
            )
            {
                Analytics.Instance.LevelStarted(
                    Analytics.Instance.GetCurrentLevel()
                );

                yield break;
            }

            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // =========================================================
    // INSTANT FORCE
    // =========================================================

    public void ApplyInstantForce(
        Vector3 direction,
        float force
    )
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null)
            return;

        rb.AddForce(
            direction.normalized * force,
            ForceMode.Impulse
        );
    }
}
