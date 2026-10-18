using UnityEngine;
using TMPro;
public class RumbleZone : MonoBehaviour
{
    public RumbleBlock[] rumbleBlocks;
    public TextMeshProUGUI rumbleUI;
    private bool active = false;

 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !active)
        {
            active = true;

            Debug.Log("RUMBLE ZONE ENTERED");

            if (rumbleUI != null)
                rumbleUI.gameObject.SetActive(true);

            foreach (RumbleBlock block in rumbleBlocks)
            {
                block.StartRumble();
            }
        }
    }
}

