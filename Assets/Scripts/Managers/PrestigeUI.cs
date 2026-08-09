using UnityEngine;
using UnityEngine.UI;
using TMPro;

//prestige window contents = shows cost + what you gain, prestiges on click then closes the window
public class PrestigeUI : MonoBehaviour
{
    [SerializeField] private GameObject window;      //modal root to close after a successful prestige
    [SerializeField] private TMP_Text infoText;      //cost + gain summary
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button prestigeButton;

    void OnEnable()
    {
        if (prestigeButton != null) prestigeButton.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        if (prestigeButton != null) prestigeButton.onClick.RemoveListener(OnClick);
    }

    void Update()
    {
        Refresh(); //money ticks continuously, keep cost/affordability live
    }

    //prestige, and if it actually went through, close the window right away
    private void OnClick()
    {
        if (PrestigeManager.instance != null && PrestigeManager.instance.TryPrestige())
        {
            if (window != null) window.SetActive(false);
        }
    }

    private void Refresh()
    {
        if (PrestigeManager.instance == null || SaveManager.instance == null || ConfigManager.instance == null) return;

        int level = SaveManager.instance.data.prestigeLevel;
        double nextCost = PrestigeManager.instance.GetNextPrestigeCost();
        double nextMult = PrestigeManager.instance.GetNextMultiplier();

        if (infoText != null)
            infoText.text = "Prestige " + level + " -> " + (level + 1)
                          + "\n\nCost: " + NumberFormatter.FormatMoney(nextCost)
                          + "\nGain: x" + nextMult.ToString("0.00") + " earnings";
        if (buttonText != null) buttonText.text = "Prestige";
        if (prestigeButton != null) prestigeButton.interactable = PrestigeManager.instance.CanPrestige();
    }
}
