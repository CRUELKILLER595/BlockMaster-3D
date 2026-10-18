using UnityEngine;

public class Reset : MonoBehaviour
{
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log(
            "ALL PLAYER DATA RESET"
        );
    }
}