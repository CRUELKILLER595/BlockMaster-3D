using UnityEngine;
using TMPro;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject leftPanel;
    public GameObject rightPanel;
public GameObject Button;
public TextMeshProUGUI ButtonText;

    bool showingLeft = true;


    public void SwitchPanels()
    {
        showingLeft = !showingLeft;

        leftPanel.SetActive(showingLeft);
        rightPanel.SetActive(!showingLeft);
        if (ButtonText != null&& showingLeft)
        {
            ButtonText.text = ">";
        }
        else if (ButtonText != null)
        {
            ButtonText.text = "<";
        }
    }
}