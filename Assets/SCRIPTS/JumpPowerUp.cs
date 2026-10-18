using UnityEngine;

public class JumpPowerUp : MonoBehaviour
{
    public float duration = 10f;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player == null)
            return;

        collected = true;

        Debug.Log("JUMP POWER-UP COLLECTED");

        player.ActivateJumpBoost(duration);

        if (Analytics.Instance != null)
        {
            Analytics.Instance.PowerupCollected(
                Analytics.Instance.GetCurrentLevel(),
                "JumpPowerUp",
                1
            );
        }

        Destroy(gameObject);
    }
}