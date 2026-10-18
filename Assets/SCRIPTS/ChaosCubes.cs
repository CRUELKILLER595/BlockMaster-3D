using UnityEngine;

public class ChaosCubes : MonoBehaviour
{
    
  
 [Header("Movement Area Limits")]
    public Vector3 minBounds;
    public Vector3 maxBounds;

    [Header("Movement Settings")]
    public float speed = 3f;
    public float waitTime = 1f;

    private Vector3 targetPosition;
    private float waitTimer;

    void Start()
    {
        SetNewTarget();
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                SetNewTarget();
                waitTimer = 0f;
            }
        }
    }

    void SetNewTarget()
    {
        float x = Random.Range(minBounds.x, maxBounds.x);
        float y = Random.Range(minBounds.y, maxBounds.y);
        float z = Random.Range(minBounds.z, maxBounds.z);

        targetPosition = new Vector3(x, y, z);
    }
}

