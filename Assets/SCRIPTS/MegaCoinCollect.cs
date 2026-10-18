using UnityEngine;

public class MegaCoinCollect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public int CoinValue=5;
    public AudioClip sound;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
             Debug.Log("Coin Collected!");
             SFXManager.instance.PlaySound(sound);
             ScoreManager.instance.AddCoins(CoinValue);
                 Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,100*Time.deltaTime,0);
    }
}


    // Update is called once per frame
   

