using UnityEngine;

//handles prestige = reset for a permanent multiplier and an extra boat slot
public class PrestigeManager : MonoBehaviour
{
    public static PrestigeManager instance;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    void Start()
    {
        //push the saved prestige level into the stats hub so the multiplier is right from the start
        StatsManager.instance.prestigeLevel = SaveManager.instance.data.prestigeLevel;
        StatsManager.instance.RaiseStatsChanged();
    }

    //what the next prestige costs
    public double GetNextPrestigeCost()
    {
        int level = SaveManager.instance.data.prestigeLevel;
        return ConfigManager.instance.config.GetPrestigeCost(level + 1);
    }

    //the multiplier you'd have after the next prestige
    public double GetNextMultiplier()
    {
        int level = SaveManager.instance.data.prestigeLevel;
        return ConfigManager.instance.config.GetPrestigeMultiplier(level + 1);
    }

    public bool CanPrestige() => SaveManager.instance.data.money >= GetNextPrestigeCost();

    //do the prestige = wipe money/fish, strip boat upgrades, bump the tier, unlock a slot
    //tap upgrades survive on purpose
    public bool TryPrestige()
    {
        if (!CanPrestige()) return false;

        SaveData data = SaveManager.instance.data;
        data.prestigeLevel += 1;
        data.money = 0d;
        data.fishCount = 0d;

        //boats stay owned but lose their upgrade levels
        foreach (BoatData b in data.boats)
            b.level = 0;

        //push the new level = updates the global multiplier and MaxSlots (prestigeLevel + 1)
        StatsManager.instance.prestigeLevel = data.prestigeLevel;
        StatsManager.instance.RaiseStatsChanged();
        return true;
    }
}
