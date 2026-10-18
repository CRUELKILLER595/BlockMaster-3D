using UnityEngine;

public class FOLLOWPLAYER : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Camera Offset")]
    public Vector3 offset =
        new Vector3(
            0f,
            8f,
            -10f
        );

    [Header("Follow")]
    public float smoothSpeed = 10f;

    [Header("Camera Rotation")]
    public Vector3 cameraRotation =
        new Vector3(
            35f,
            0f,
            0f
        );

    private Camera cam;

    void Awake()
    {
        cam =
            GetComponent<Camera>();

        if (cam != null)
        {
            cam.enabled = true;
        }

        FindPlayer();
    }

    void Start()
    {
        FindPlayer();

        if (player != null)
        {
            
            transform.position =
                player.position + offset;

            transform.rotation =
                Quaternion.Euler(
                    cameraRotation
                );
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        Vector3 targetPosition =
            player.position + offset;

        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed *
                Time.deltaTime
            );

        transform.rotation =
            Quaternion.Euler(
                cameraRotation
            );
    }

    void FindPlayer()
    {
        if (player != null)
            return;

        PlayerMovement playerMovement =
            FindFirstObjectByType<
                PlayerMovement
            >();

        if (playerMovement != null)
        {
            player =
                playerMovement.transform;
        }

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag(
                    "Player"
                );

            if (playerObject != null)
            {
                player =
                    playerObject.transform;
            }
        }
    }
}