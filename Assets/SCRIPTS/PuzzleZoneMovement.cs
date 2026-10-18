
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PuzzleMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement runnerMovement;

    [Header("Puzzle Movement")]
    public float moveSpeed = 6f;

    [Tooltip("Tag assigned to the Puzzle Zone trigger.")]
    public string puzzleZoneTag = "PuzzleZone";

    private Rigidbody rb;
    private Collider playerCollider;

    private bool inPuzzleZone;
    private bool controlsPaused;

    private float minimumZ;
    private float maximumZ;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (runnerMovement == null)
            runnerMovement = GetComponent<PlayerMovement>();

        playerCollider = GetComponent<Collider>();

        if (playerCollider == null)
            playerCollider = GetComponentInChildren<Collider>();

        if (runnerMovement == null)
            Debug.LogError(
                "PuzzleMovement: PlayerMovement is missing."
            );

        if (playerCollider == null)
            Debug.LogWarning(
                "PuzzleMovement: Player collider not found."
            );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(puzzleZoneTag))
            return;

        if (inPuzzleZone || runnerMovement == null)
            return;

        Bounds zoneBounds = other.bounds;

        // Calculate boundaries using the player's collider.
        float colliderOffsetZ = 0f;
        float playerExtentZ = 0f;

        if (playerCollider != null)
        {
            colliderOffsetZ =
                playerCollider.bounds.center.z - rb.position.z;

            playerExtentZ =
                playerCollider.bounds.extents.z;
        }

        // The player cannot move backward past the entrance.
        minimumZ =
            zoneBounds.min.z + playerExtentZ - colliderOffsetZ;

        // The player returns to runner movement at this boundary.
        maximumZ =
            zoneBounds.max.z - playerExtentZ - colliderOffsetZ;

        if (maximumZ < minimumZ)
            maximumZ = minimumZ;

        inPuzzleZone = true;
        controlsPaused = true;

        // Hand movement control over to this script.
        runnerMovement.puzzleMode = true;
        runnerMovement.canMoveBackward = true;

        // Remove sideways momentum without cancelling
        // the backward impulse applied by PuzzleZone.
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;
        rb.linearVelocity = velocity;

        Debug.Log(
            "Entered Puzzle Zone. Waiting for the pause sequence."
        );
    }

    private void FixedUpdate()
    {
        if (!inPuzzleZone)
            return;

        Vector3 position = rb.position;
        Vector3 velocity = rb.linearVelocity;

        // Keep the rear boundary and fall check active
        // even during the entry impulse and pause sequence.
        if (controlsPaused)
        {
            if (position.z < minimumZ)
            {
                position.z = minimumZ;

                if (velocity.z < 0f)
                    velocity.z = 0f;
            }

            if (position != rb.position)
                rb.position = position;

            rb.linearVelocity = velocity;

            CheckForFall();
            return;
        }

        Keyboard keyboard = Keyboard.current;

        float horizontal = 0f;
        float longitudinal = 0f;

        if (keyboard != null)
        {
            bool left =
                keyboard.aKey.isPressed ||
                keyboard.leftArrowKey.isPressed;

            bool right =
                keyboard.dKey.isPressed ||
                keyboard.rightArrowKey.isPressed;

            bool forward =
                keyboard.wKey.isPressed ||
                keyboard.upArrowKey.isPressed;

            bool backward =
                keyboard.sKey.isPressed ||
                keyboard.downArrowKey.isPressed;

            if (left != right)
                horizontal = right ? 1f : -1f;

            if (forward != backward)
                longitudinal = forward ? 1f : -1f;
        }

        // Manual movement. Vertical velocity is preserved
        // so jumping and falling continue to work.
        velocity.x = horizontal * moveSpeed;
        velocity.z = longitudinal * moveSpeed;

        // Prevent the player from leaving the normal X limits.
        if (runnerMovement != null)
        {
            float clampedX = Mathf.Clamp(
                position.x,
                runnerMovement.startX,
                runnerMovement.endX
            );

            if (!Mathf.Approximately(clampedX, position.x))
            {
                position.x = clampedX;
                velocity.x = 0f;
            }
        }

        // Prevent backward escape through the rear entrance.
        if (position.z <= minimumZ && velocity.z < 0f)
            velocity.z = 0f;

        // Correct any physics movement beyond the rear boundary.
        if (position.z < minimumZ)
        {
            position.z = minimumZ;

            if (velocity.z < 0f)
                velocity.z = 0f;
        }

        // Detect reaching the forward end of the zone.
        bool reachedForwardExit = position.z >= maximumZ;

        if (reachedForwardExit)
        {
            position.z = maximumZ;
            velocity.x = 0f;
        }

        if (position != rb.position)
            rb.position = position;

        rb.linearVelocity = velocity;

        CheckForFall();

        // Restore normal runner movement after reaching
        // the forward boundary.
        if (reachedForwardExit)
        {
            ExitPuzzleZone();
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(puzzleZoneTag))
            return;

        // A side or rear exit must not restore auto-running.
        if (rb.position.z < maximumZ)
            return;

        ExitPuzzleZone();
    }

    private void CheckForFall()
    {
        if (rb.position.y < -1f &&
            runnerMovement != null &&
            runnerMovement.gm != null)
        {
            runnerMovement.gm.EndGame();
        }
    }

    // Called by PuzzleZone to pause or resume manual controls.
    public void SetControlsPaused(bool paused)
    {
        controlsPaused = paused;
    }

    // Called by PuzzleZone when the timer expires.
    public void ForceExitPuzzleZone()
    {
        ExitPuzzleZone();
    }

    private void ExitPuzzleZone()
    {
        if (!inPuzzleZone)
            return;

        inPuzzleZone = false;
        controlsPaused = false;

        if (runnerMovement != null)
        {
            runnerMovement.puzzleMode = false;
            runnerMovement.canMoveBackward = false;
        }

        // Resume normal forward-running movement.
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;

        if (runnerMovement != null)
            velocity.z = runnerMovement.autoRunSpeed;

        rb.linearVelocity = velocity;

        Debug.Log(
            "Exited Puzzle Zone. Auto-run restored."
        );
    }
}

