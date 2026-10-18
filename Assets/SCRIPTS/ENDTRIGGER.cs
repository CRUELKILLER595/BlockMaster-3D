using UnityEngine;

public class ENDTRIGGER : MonoBehaviour
{
    public LevelEnd levelEnd;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (levelEnd != null)
        {
            levelEnd.CompleteLevel();
        }
    }
}