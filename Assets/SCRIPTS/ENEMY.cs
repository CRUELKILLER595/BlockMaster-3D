using UnityEngine;

public class ENEMY : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GAMEMANAGER gm;
    
    public AudioClip strikeCollisionSound;
   

    private Collider[] enemyColliders;
    private Collider[] playerColliders;

   
    void Awake()
    {
       

        if (playerMovement == null)
        {
            playerMovement =
                FindFirstObjectByType<PlayerMovement>();
        }

        if (gm == null)
        {
            gm =
                FindFirstObjectByType<GAMEMANAGER>();
        }

        enemyColliders =
            GetComponentsInChildren<Collider>();

        if (playerMovement != null)
        {
            playerColliders =
                playerMovement
                    .GetComponentsInChildren<Collider>();
        }
    }

   

    void OnEnable()
    {
        if (playerMovement == null)
        {
            playerMovement =
                FindFirstObjectByType<PlayerMovement>();
        }

        if (
            playerMovement != null &&
            playerMovement.isInevitable
        )
        {
            SetPlayerCollisionIgnored(
                true
            );
        }
    }

   
    
private void OnCollisionEnter(Collision collision)
{
    // Find PlayerMovement even if the player's collider
    // is attached to a child GameObject.
    PlayerMovement hitPlayer =
        collision.collider.GetComponentInParent<PlayerMovement>();

    if (hitPlayer == null)
        return;

    playerMovement = hitPlayer;

  
    if (hitPlayer.isInevitable)
        return;

 
    if (hitPlayer.isStriking)
    {
        Debug.Log("STRIKE HIT!");
        hitPlayer.MaintainStrikeMomentum();

        if (SFXManager.instance != null &&
            strikeCollisionSound != null)
        {
            SFXManager.instance.PlaySound(strikeCollisionSound);
        }

        if (Analytics.Instance != null)
        {
            Analytics.Instance.ObstacleDestroyed(
                Analytics.Instance.GetCurrentLevel(),
                gameObject.name
            );
        }

        Destroy(gameObject);
        return;
    }

    
    Debug.Log("ENEMY HIT THE PLAYER");

    GAMEMANAGER manager =
        gm != null ? gm : hitPlayer.gm;

    if (manager != null)
        manager.EndGame();
}

    public void SetPlayerCollisionIgnored(
        bool ignore
    )
    {
        if (playerMovement == null)
            return;

        if (enemyColliders == null)
        {
            enemyColliders =
                GetComponentsInChildren<Collider>();
        }

        if (playerColliders == null)
        {
            playerColliders =
                playerMovement
                    .GetComponentsInChildren<Collider>();
        }

        foreach (
            Collider enemyCollider
            in enemyColliders
        )
        {
            if (enemyCollider == null)
                continue;

            foreach (
                Collider playerCollider
                in playerColliders
            )
            {
                if (playerCollider == null)
                    continue;

                Physics.IgnoreCollision(
                    enemyCollider,
                    playerCollider,
                    ignore
                );
            }
        }
    }
}