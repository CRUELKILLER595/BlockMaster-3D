
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LeapPoint : MonoBehaviour
{
    [Header("Cooldown")]
    public float cooldownTime = 1f;

    [Header("Audio")]
    public AudioClip leapSound;

    private float nextActivationTime;

    private void Reset()
    {
        Collider trigger = GetComponent<Collider>();

        if (trigger != null)
            trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Diagnostic log to verify that the trigger is firing.
        Debug.Log(
            $"LeapPoint: Trigger entered by {other.name}",
            this
        );

        // Find PlayerMovement even if the collider is on a child object.
        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player == null)
        {
            Debug.Log(
                "LeapPoint: No PlayerMovement found on this collider's parents.",
                this
            );

            return;
        }

        // Prevent repeated activation during the cooldown.
        if (Time.time < nextActivationTime)
            return;

        nextActivationTime =
            Time.time + Mathf.Max(0f, cooldownTime);

        player.LaunchFromLeapPoint();

        if (leapSound != null)
        {
            AudioSource.PlayClipAtPoint(
                leapSound,
                transform.position
            );
        }
    }
}
