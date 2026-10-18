using UnityEngine;

public class Inevitable : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public int InevitableTime = 5;

    private bool collected = false;

    void OnTriggerEnter(
        Collider other
    )
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (playerMovement == null)
        {
            playerMovement =
                other.GetComponentInParent<
                    PlayerMovement
                >();
        }

        if (playerMovement == null)
            return;

        collected = true;

        Debug.Log(
            "INEVITABLE POWER-UP COLLECTED"
        );

        playerMovement.EnableInevitable(
            InevitableTime
        );

        if (Analytics.Instance != null)
        {
            Analytics.Instance.PowerupCollected(
                Analytics.Instance.GetCurrentLevel(),
                "Inevitable",
                1
            );
        }

        Destroy(gameObject);
    }
}