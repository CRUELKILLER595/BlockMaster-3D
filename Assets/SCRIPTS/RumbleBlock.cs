using UnityEngine;

public class RumbleBlock : MonoBehaviour
{
    private Vector3 startPos;

    private bool rumbling = false;

    public float height = 0.5f;
    public float speed = 6f;

    private GAMEMANAGER gm;

    private Collider blockCollider;
    private Rigidbody blockRb;

    void Start()
    {
        startPos =
            transform.position;

        gm =
            FindFirstObjectByType<
                GAMEMANAGER
            >();

        blockCollider =
            GetComponent<Collider>();

        blockRb =
            GetComponent<Rigidbody>();

        // =====================================================
        // PREVENT PHYSICS PUSHING
        // =====================================================

        if (blockCollider != null)
        {
            blockCollider.isTrigger = true;
        }

        if (blockRb != null)
        {
            blockRb.isKinematic = true;
        }
    }

    void Update()
    {
        if (!rumbling)
            return;

        float y =
            Mathf.Sin(
                Time.time * speed
            ) * height;

        Vector3 target =
            startPos +
            new Vector3(
                0f,
                y,
                0f
            );

        if (blockRb != null)
        {
            blockRb.MovePosition(
                target
            );
        }
        else
        {
            transform.position =
                target;
        }
    }

    public void StartRumble()
    {
        rumbling = true;
    }

    // =========================================================
    // TRIGGER
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
            "PLAYER HIT RUMBLE BLOCK"
        );

        if (gm != null)
        {
            gm.EndGame();
        }
    }
}