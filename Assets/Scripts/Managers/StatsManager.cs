using System;
using UnityEngine;

//central calculator = every system pushes its numbers here and the ui reads the totals back
public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;

    //ui and other systems subscribe to this so they auto-refresh when anything changes
    public event Action OnStatsChanged;

    //contributions pushed in by other managers as we build them
    public double boatFishPerSecond = 0d; //set by BoatManager
    public int prestigeLevel = 0; //set by PrestigeManager

    //equipped-fossil bonuses, recomputed by FossilManager (small values, float is plenty)
    public float fossilFishMultiplier = 0f; //extra fish % (0.4 = +40% on clicks and boats)
    public float fossilSellBonus = 0f; //extra sell multiplier %
    public float fossilSpawnFactor = 1f; //multiplies the fossil spawn interval (0.7 = 30% faster)
    public float fossilBarFactor = 1f; //multiplies the minigame bar speed (0.7 = 30% slower)
    public float fossilPriceModifier = 0f; //flat $ added to the price per fish

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    //prestige multiplier applies to everything = the global earnings boost
    public double GetGlobalMultiplier()
    {
        return ConfigManager.instance.config.GetPrestigeMultiplier(prestigeLevel);
    }

    //every fish gain is scaled by this = clicks and boats alike
    public double GetFishMultiplier()
    {
        return 1d + fossilFishMultiplier;
    }

    //total fish per second = boats scaled by fossil fish bonuses
    //prestige is deliberately NOT applied here = it lands once at sell, so clicks and boats scale the same
    public double GetFishPerSecond()
    {
        return boatFishPerSecond * GetFishMultiplier();
    }

    //what each fish is worth = fossils and prestige stacked on top of the base price
    public double GetSellMultiplier()
    {
        return (1d + fossilSellBonus) * GetGlobalMultiplier();
    }

    //any system calls this after changing a stat so the ui refreshes just once
    public void RaiseStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }
}
