using UnityEngine;
using System.Collections.Generic;
public class CoinSpawnManager : MonoBehaviour
{
  private List<Bounds> obstacleBounds = new List<Bounds>();
    [Header("Playable Boundaries")]
    public float minX;
    public float maxX;

    [Header("Spawn Region")]
    public float minZ;
    public float maxZ;
private GameObject[] obstacles;
    [Header("Spawn Settings")]
    public float obstacleClearance = 1.5f;
    private void Start()
    {
      FindObstacles();

    }
void FindObstacles()
{
    obstacles = GameObject.FindGameObjectsWithTag("OBSTACLE");

    obstacleBounds.Clear();

    foreach (GameObject obstacle in obstacles)
    {
        BoxCollider box = obstacle.GetComponent<BoxCollider>();

        if (box != null)
        {
            obstacleBounds.Add(box.bounds);
        }
    }

    Debug.Log("Total obstacle bounds stored: " + obstacleBounds.Count);
}
bool IsValidCoinPosition(Vector3 position)
{
    foreach (Bounds bounds in obstacleBounds)
    {
        bool insideX =
            position.x >= bounds.min.x - obstacleClearance &&
            position.x <= bounds.max.x + obstacleClearance;

        bool insideZ =
            position.z >= bounds.min.z - obstacleClearance &&
            position.z <= bounds.max.z + obstacleClearance;

        if (insideX && insideZ)
        {
            return false;
        }
    }

    return true;
}
}
