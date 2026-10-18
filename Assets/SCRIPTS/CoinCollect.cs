using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    public int CoinValue = 1;
    public AudioClip sound;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin Collected!");

            
            if (SFXManager.instance != null)
                SFXManager.instance.PlaySound(sound);
            else
                Debug.LogError("SFXManager is NULL!");

           
            if (ScoreManager.instance != null)
                ScoreManager.instance.AddCoins(CoinValue);
            else
                Debug.LogError("ScoreManager is NULL!");

            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
}