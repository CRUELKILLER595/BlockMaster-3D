
using UnityEngine;
using System.Collections.Generic;

public class CoinSpawnManager : MonoBehaviour
{
    [Header("Coin Prefab")]
    public GameObject coinPrefab;

    [Header("Playable Boundaries")]
    public float minX = -8f;
    public float maxX = 8f;

    public float minZ = -10f;
    public float maxZ = 100f;

    [Header("Coin Height")]
    public float floorY = 1f;

    [Header("Optional Ground Detection")]
    public bool useGroundRaycast = false;

    public LayerMask groundLayers = ~0;

    public float raycastStartY = 100f;
    public float raycastDistance = 200f;
    public float groundOffset = 1f;

    [Header("Obstacle Detection")]
    public string obstacleTag = "OBSTACLE";

    public float obstacleClearance = 1.5f;
    public float boundaryClearance = 0.75f;

    [Header("Coin Spawning")]
    public int coinsToSpawn = 15;
    public int candidateSamplesPerCoin = 40;

    public float minCoinSpacing = 2f;

    public bool spawnOnStart = true;

    private readonly List<Bounds> obstacleBounds =
        new List<Bounds>();

    private readonly List<Vector3> spawnedPositions =
        new List<Vector3>();

    private readonly List<GameObject> spawnedCoins =
        new List<GameObject>();

    public int SpawnedCoinCount => spawnedCoins.Count;

    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        if (spawnOnStart)
            SpawnCoins();
    }

    // ==================================================
    // MAIN SPAWNING METHOD
    // ==================================================

    public void SpawnCoins()
    {
        if (coinPrefab == null)
        {
            Debug.LogError(
                "CoinSpawnManager: Assign the Coin Prefab."
            );

            return;
        }

        if (minX >= maxX || minZ >= maxZ)
        {
            Debug.LogError(
                "CoinSpawnManager: Invalid boundaries."
            );

            return;
        }

        FindObstacles();

        ClearSpawnedCoins();

        int successfulSpawns = 0;

        for (int i = 0; i < coinsToSpawn; i++)
        {
            if (!TryFindBestPosition(out Vector3 bestPosition))
            {
                Debug.LogWarning(
                    "CoinSpawnManager: No valid position for coin " +
                    (i + 1)
                );

                continue;
            }

            GameObject coin = Instantiate(
                coinPrefab,
                bestPosition,
                coinPrefab.transform.rotation,
                transform
            );

            coin.name = "SpawnedCoin_" + (i + 1);

            spawnedCoins.Add(coin);
            spawnedPositions.Add(bestPosition);

            successfulSpawns++;
        }

        Debug.Log(
            "CoinSpawnManager: Spawned " +
            successfulSpawns + " / " + coinsToSpawn + " coins."
        );
    }

    // ==================================================
    // DETECT OBSTACLES
    // ==================================================

    private void FindObstacles()
    {
        obstacleBounds.Clear();

        GameObject[] obstacles;

        try
        {
            obstacles = GameObject.FindGameObjectsWithTag(obstacleTag);
        }
        catch (UnityException exception)
        {
            Debug.LogError(
                "CoinSpawnManager: Invalid tag. " +
                exception.Message
            );

            return;
        }

        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle == null)
                continue;

            BoxCollider[] colliders =
                obstacle.GetComponentsInChildren<BoxCollider>();

            foreach (BoxCollider box in colliders)
            {
                if (box == null ||
                    !box.enabled ||
                    !box.gameObject.activeInHierarchy)
                {
                    continue;
                }

                obstacleBounds.Add(box.bounds);
            }
        }

        Debug.Log(
            "CoinSpawnManager: Stored " +
            obstacleBounds.Count + " obstacle collider bounds."
        );
    }

    // ==================================================
    // GENERATE RANDOM CANDIDATE
    // ==================================================

    private bool GenerateRandomPosition(out Vector3 position)
    {
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        float randomY = floorY;

        if (useGroundRaycast)
        {
            Vector3 rayOrigin = new Vector3(
                randomX,
                raycastStartY,
                randomZ
            );

            if (!Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit hit,
                raycastDistance,
                groundLayers,
                QueryTriggerInteraction.Ignore
            ))
            {
                position = Vector3.zero;
                return false;
            }

            randomY = hit.point.y + groundOffset;
        }

        position = new Vector3(
            randomX,
            randomY,
            randomZ
        );

        return true;
    }

    // ==================================================
    // VALIDATE POSITION
    // ==================================================

    public bool IsValidCoinPosition(Vector3 position)
    {
        // Level boundaries.
        if (position.x < minX + boundaryClearance ||
            position.x > maxX - boundaryClearance ||
            position.z < minZ + boundaryClearance ||
            position.z > maxZ - boundaryClearance)
        {
            return false;
        }

        // Obstacles and enemies.
        foreach (Bounds bounds in obstacleBounds)
        {
            float distance = DistanceToBoundsXZ(
                position,
                bounds
            );

            if (distance < obstacleClearance)
                return false;
        }

        // Already spawned coins.
        float minimumSpacingSquared =
            minCoinSpacing * minCoinSpacing;

        foreach (Vector3 existingPosition in spawnedPositions)
        {
            if (HorizontalDistanceSquared(
                position,
                existingPosition
            ) < minimumSpacingSquared)
            {
                return false;
            }
        }

        return true;
    }

    // ==================================================
    // FIND BEST POSITION FROM RANDOM CANDIDATES
    // ==================================================

    private bool TryFindBestPosition(out Vector3 bestPosition)
    {
        bestPosition = Vector3.zero;

        bool foundPosition = false;
        float bestScore = -1f;

        int samples = Mathf.Max(1, candidateSamplesPerCoin);

        for (int i = 0; i < samples; i++)
        {
            if (!GenerateRandomPosition(out Vector3 candidate))
                continue;

            if (!IsValidCoinPosition(candidate))
                continue;

            float score = CalculateClearanceScore(candidate);

            if (!foundPosition || score > bestScore)
            {
                foundPosition = true;
                bestScore = score;
                bestPosition = candidate;
            }
        }

        return foundPosition;
    }

    // ==================================================
    // CLEARANCE SCORING
    // ==================================================

    private float CalculateClearanceScore(Vector3 position)
    {
        float score = Mathf.Min(
            position.x - minX,
            maxX - position.x,
            position.z - minZ,
            maxZ - position.z
        );

        foreach (Bounds bounds in obstacleBounds)
        {
            float distance = DistanceToBoundsXZ(
                position,
                bounds
            );

            score = Mathf.Min(score, distance);
        }

        foreach (Vector3 existingPosition in spawnedPositions)
        {
            float distance = Mathf.Sqrt(
                HorizontalDistanceSquared(
                    position,
                    existingPosition
                )
            );

            score = Mathf.Min(score, distance);
        }

        return score;
    }

    // ==================================================
    // DISTANCE HELPERS
    // ==================================================

    private float DistanceToBoundsXZ(
        Vector3 position,
        Bounds bounds)
    {
        float dx = Mathf.Max(
            bounds.min.x - position.x,
            0f,
            position.x - bounds.max.x
        );

        float dz = Mathf.Max(
            bounds.min.z - position.z,
            0f,
            position.z - bounds.max.z
        );

        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    private float HorizontalDistanceSquared(
        Vector3 a,
        Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;

        return dx * dx + dz * dz;
    }

    // ==================================================
    // CLEAR PREVIOUSLY SPAWNED COINS
    // ==================================================

    public void ClearSpawnedCoins()
    {
        foreach (GameObject coin in spawnedCoins)
        {
            if (coin != null)
                Destroy(coin);
        }

        spawnedCoins.Clear();
        spawnedPositions.Clear();
    }
}
