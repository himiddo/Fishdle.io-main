using UnityEngine;
using UnityEngine.UI;
using TMPro;

//tap upgrade panel = shows fish per tap and buys the next level (capped, unlike boats)
public class TapUpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;   //fish per tap + level
    [SerializeField] private TMP_Text buttonText;   //cost or "Maxed"
    [SerializeField] private Button upgradeButton;

    void OnEnable()
    {
        if (upgradeButton != null) upgradeButton.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        if (upgradeButton != null) upgradeButton.onClick.RemoveListener(OnClick);
    }

    void Update()
    {
        Refresh(); //money ticks continuously, keep affordability live
    }

    private void OnClick()
    {
        if (FishingSystem.instance != null) FishingSystem.instance.TryUpgradeTap();
    }

    private void Refresh()
    {
        if (FishingSystem.instance == null || ConfigManager.instance == null || SaveManager.instance == null) return;

        GameConfig cfg = ConfigManager.instance.config;
        int level = SaveManager.instance.data.clickLevel;
        double money = SaveManager.instance.data.money;

        if (statusText != null)
            statusText.text = "Fish per tap: " + NumberFormatter.Format(FishingSystem.instance.GetFishPerTap())
                            + "\nLevel " + level + " / " + cfg.maxClickLevel;

        //fully upgraded = the system retires itself
        if (level >= cfg.maxClickLevel)
        {
            if (buttonText != null) buttonText.text = "Maxed";
            if (upgradeButton != null) upgradeButton.interactable = false;
            return;
        }

        double cost = cfg.GetClickUpgradeCost(level);
        if (buttonText != null) buttonText.text = "Upgrade  " + NumberFormatter.FormatMoney(cost);
        if (upgradeButton != null) upgradeButton.interactable = money >= cost;
    }
}
