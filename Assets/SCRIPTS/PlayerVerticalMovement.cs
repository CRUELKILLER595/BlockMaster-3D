using UnityEngine;

public class EnemyVerticalMovement : MonoBehaviour
{
    [Header("Vertical Movement")]
    public float speed = 5f;
    public float height = 5f;

    private Vector3 startPos;

    private GAMEMANAGER gm;

    private Collider enemyCollider;
    private Rigidbody enemyRb;

    void Start()
    {
        startPos =
            transform.position;

        gm =
            FindFirstObjectByType<
                GAMEMANAGER
            >();

        enemyCollider =
            GetComponent<Collider>();

        enemyRb =
            GetComponent<Rigidbody>();

        // =====================================================
        // PREVENT PLAYER FROM BEING PHYSICALLY PUSHED
        // =====================================================

        if (enemyCollider != null)
        {
            enemyCollider.isTrigger = true;
        }

        if (enemyRb != null)
        {
            enemyRb.isKinematic = true;
        }
    }

    void Update()
    {
        float yOffset =
            Mathf.Sin(
                Time.time * speed
            ) * height;

        Vector3 target =
            startPos +
            new Vector3(
                0f,
                yOffset,
                0f
            );

        if (enemyRb != null)
        {
            enemyRb.MovePosition(
                target
            );
        }
        else
        {
            transform.position =
                target;
        }
    }

    // =========================================================
    // PLAYER HIT
    // =========================================================

    void OnTriggerEnter(
        Collider other
    )
    {
        if (
            !other.CompareTag(
                "Player"
            )
        )
        {
            return;
        }

        Debug.Log(
            "PLAYER HIT VERTICAL ENEMY"
        );

        if (gm == null)
        {
            gm =
                FindFirstObjectByType<
                    GAMEMANAGER
                >();
        }

        if (gm != null)
        {
            gm.EndGame();
        }
    }
}