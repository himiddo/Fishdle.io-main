using UnityEngine;

//handles money = earning from sales now, spending on boats later
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    //sell the whole fish stockpile at the current price plus any sell bonuses
    public void SellAllFish()
    {
        SaveData data = SaveManager.instance.data;
        if (data.fishCount <= 0d) return; //nothing to sell

        double earnings = data.fishCount * PriceManager.instance.EffectivePrice * StatsManager.instance.GetSellMultiplier();
        data.money += earnings;
        data.fishCount = 0d; //stockpile emptied
        StatsManager.instance.RaiseStatsChanged();
    }

    //add money from any source
    public void AddMoney(double amount)
    {
        SaveManager.instance.data.money += amount;
        StatsManager.instance.RaiseStatsChanged();
    }

    //try to spend = returns false if you can't afford it
    public bool TrySpend(double amount)
    {
        SaveData data = SaveManager.instance.data;
        if (data.money < amount) return false;
        data.money -= amount;
        StatsManager.instance.RaiseStatsChanged();
        return true;
    }
}
