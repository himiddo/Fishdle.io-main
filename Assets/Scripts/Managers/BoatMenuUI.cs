using UnityEngine;
using UnityEngine.UI;
using TMPro;

//boat menu row = buy the boat, then upgrade it, with live labels
public class BoatMenuUI : MonoBehaviour
{
    [SerializeField] private int tier = 1;
    [SerializeField] private TMP_Text statusText;   //name + level + fish/sec
    [SerializeField] private TMP_Text buttonText;   //buy/upgrade label + cost
    [SerializeField] private Button actionButton;

    //the list builder assigns which boat tier this row shows
    public void SetTier(int t) { tier = t; }

    void OnEnable()
    {
        if (actionButton != null) actionButton.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        if (actionButton != null) actionButton.onClick.RemoveListener(OnClick);
    }

    void Update()
    {
        Refresh(); //money ticks continuously, keep labels + affordability live
    }

    //buy if we don't own it yet, otherwise upgrade it
    private void OnClick()
    {
        if (BoatManager.instance == null) return;
        if (BoatManager.instance.OwnsBoat(tier))
            BoatManager.instance.TryUpgradeBoat(tier);
        else
            BoatManager.instance.TryBuyBoat(tier);
    }

    private void Refresh()
    {
        if (BoatManager.instance == null || ConfigManager.instance == null || SaveManager.instance == null) return;

        GameConfig cfg = ConfigManager.instance.config;
        BoatData boat = BoatManager.instance.GetBoat(tier);
        double money = SaveManager.instance.data.money;

        //this tier's slot isn't unlocked yet = locked behind prestige
        if (boat == null && tier > BoatManager.instance.MaxSlots)
        {
            if (statusText != null) statusText.text = "Boat " + tier + " - unlock by prestiging";
            if (buttonText != null) buttonText.text = "Locked";
            if (actionButton != null) actionButton.interactable = false;
            return;
        }

        if (boat == null) //not bought yet
        {
            double cost = cfg.GetBoatCost(tier);
            if (statusText != null) statusText.text = "Boat " + tier;
            if (buttonText != null) buttonText.text = "Buy  " + NumberFormatter.FormatMoney(cost);
            if (actionButton != null) actionButton.interactable = money >= cost;
        }
        else //owned = show upgrade
        {
            double gen = cfg.GetBoatBaseGeneration(tier) * (1d + boat.level * cfg.upgradeBonus);
            double upCost = cfg.GetUpgradeCost(tier, boat.level);
            if (statusText != null) statusText.text = "Boat " + tier + "   Lv " + boat.level + "   " + NumberFormatter.Format(gen) + "/s";
            if (buttonText != null) buttonText.text = "Upgrade  " + NumberFormatter.FormatMoney(upCost);
            if (actionButton != null) actionButton.interactable = money >= upCost;
        }
    }
}
