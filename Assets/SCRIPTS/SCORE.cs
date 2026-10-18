using UnityEngine;

public class SCORE : MonoBehaviour
{
    public Transform player;

    public static SCORE instance;

    public int DistanceScore;
    void Start()
{
    if (player == null)
    {
        PlayerMovement pm =
            FindFirstObjectByType<PlayerMovement>();

        if (pm != null)
            player = pm.transform;
    }
}

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        DistanceScore =
            Mathf.FloorToInt(
                player.position.z + 56f
            );
    }
}