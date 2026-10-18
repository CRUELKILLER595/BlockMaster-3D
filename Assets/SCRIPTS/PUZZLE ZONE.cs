using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PuzzleZone : MonoBehaviour
{
    public float abilityTime;

    private float timer;
    private bool active;

    public GameObject puzzleUI;
    public TMP_Text timertext;

    private PlayerMovement player;
    private PuzzleMovement puzzleMovement;

    void Start()
    {
        puzzleUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !active)
        {
            player = other.GetComponentInParent<PlayerMovement>();

if (player == null)
    return;

Rigidbody playerRb = player.GetComponent<Rigidbody>();

if (playerRb == null)
    return;

// Apply backward impulse.
puzzleMovement = player.GetComponent<PuzzleMovement>();

if (puzzleMovement == null)
{
    Debug.LogError("PuzzleMovement is missing from the Player.");
    return;
}

player.puzzleMode = true;
player.canMoveBackward = true;
puzzleMovement.enabled = true;

// Apply backward impulse only after puzzle mode is enabled.
player.ApplyInstantForce(Vector3.back, 20f);

timer = abilityTime;
active = true;

if (puzzleUI != null)
    puzzleUI.SetActive(true);

if (timertext != null)
{
    timertext.enabled = true;
    timertext.text = "Time Remaining: " + Mathf.CeilToInt(timer);
}
StartCoroutine(ImpulseThenPause(player, playerRb));
        }
    }
    void Update()
    {
        if (!active) return;

        timer -=Time.deltaTime*0.8f;
        timertext.text="Time Remaining:"+Mathf.CeilToInt(timer).ToString();


        if (timer <= 0)
        {
            DisableAbility();
        }
    }

    void DisableAbility()
{
    active = false;

    if (player != null)
    {
        player.canMoveBackward = false;
        player.puzzleMode = false;
    }

    if (puzzleMovement != null)
        puzzleMovement.enabled = false;

    if (puzzleUI != null)
        puzzleUI.SetActive(false);

    if (timertext != null)
        timertext.enabled = false;
}
   private IEnumerator ImpulseThenPause(
    PlayerMovement player,
    Rigidbody playerRb)
{
    // Let the backward impulse move the player.
    yield return new WaitForSeconds(1f);

    if (player == null || playerRb == null)
        yield break;

    // Remember the original Rigidbody constraints.
    RigidbodyConstraints originalConstraints =
        playerRb.constraints;

    // Stop the player for one second.
    playerRb.linearVelocity = Vector3.zero;
    playerRb.angularVelocity = Vector3.zero;

    playerRb.constraints =
        originalConstraints |
        RigidbodyConstraints.FreezePositionX |
        RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezePositionZ;

    yield return new WaitForSeconds(1f);

    if (player == null || playerRb == null)
        yield break;

    // Restore Rigidbody movement.
    playerRb.constraints = originalConstraints;

    // IMPORTANT: allow keyboard movement in PuzzleMovement.
    if (puzzleMovement != null)
        puzzleMovement.SetControlsPaused(false);
}
}

