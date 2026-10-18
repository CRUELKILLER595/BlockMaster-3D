using UnityEngine;
using TMPro;

public class JumpPowerUpUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    void Start()
    {
        if(timerText == null)
            timerText = GetComponent<TextMeshProUGUI>();

        gameObject.SetActive(false); 
    }

    
     public void ShowTimer(float timeLeft)
    {
        gameObject.SetActive(true);
        timerText.text = "Jump Timer: " + Mathf.Ceil(timeLeft);
       
    }

    
    public void HideTimer()
    {
        gameObject.SetActive(false);
    }
}
