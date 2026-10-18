using UnityEngine;

public class StrikeThrough : MonoBehaviour
{
    public int strikeAmount = 1;

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

        Debug.Log("STRIKE POWER-UP COLLECTED");

        player.AddStrike(strikeAmount);

        if (Analytics.Instance != null)
        {
            Analytics.Instance.PowerupCollected(
                Analytics.Instance.GetCurrentLevel(),
                "StrikeThrough",
                strikeAmount
            );
        }

        Destroy(gameObject);
    }
}