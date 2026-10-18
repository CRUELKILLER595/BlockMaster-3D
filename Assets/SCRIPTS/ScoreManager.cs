using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int coins = 0;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddCoins(int value)
    {
        coins += value;
        Debug.Log("Coins: " + coins);
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if(scoreText != null)
            scoreText.text = "Coins: " + coins.ToString();
        else
            Debug.LogError("Score Text not assigned!");
    }
}