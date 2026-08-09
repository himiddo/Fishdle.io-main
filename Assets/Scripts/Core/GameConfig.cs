using UnityEngine;

//all balance numbers live in one place so nothing is hard-coded around the project
//made a ScriptableObject = you can create a GameConfig asset and tweak values in the inspector
//formulas return double so late-prestige costs stay exact instead of losing precision
[CreateAssetMenu(fileName = "GameConfig", menuName = "Fishdle/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Boat Progression")]
    public float boat1BaseCost = 100f;
    public float boatCostMultiplier = 2f; //boat N cost = boat1 * this^(N-1)
    public float boat1BaseGeneration = 1f; //fish per second for boat 1
    public float boatGenerationMultiplier = 2f; //matches the cost multiplier so every boat is equal value
    public float upgradeBonus = 0.5f; //each upgrade adds this share of the boat's own base gen
    public float upgradeCostMultiplier = 1.25f;

    [Header("Clicking")]
    public float fishPerClick = 3f; //base fish per tap = the early-game engine, irrelevant once boats scale
    public float clickUpgradeBonus = 1f; //each tap upgrade adds this many fish per tap
    public float clickUpgradeBaseCost = 50f;
    public float clickUpgradeCostMultiplier = 1.6f;
    public int maxClickLevel = 15; //finite on purpose, unlike boats and prestige

    [Header("Prestige")]
    public float prestigeMultiplier = 1.5f; //earnings boost gained per prestige tier
    public float firstPrestigeCost = 2000f; //this is the dial for how long a run is
    public float prestigeCostMultiplier = 3f; //keep equal to boatGenerationMultiplier * prestigeMultiplier for constant runs

    [Header("Fossils")]
    public int maxEquippedFossils = 5;
    public int fossilUnlockPrestige = 2; //fossils stay hidden until you've prestiged this many times

    [Header("Economy")]
    public float minPrice = 0.5f;
    public float maxPrice = 2.5f;
    public float priceChangeInterval = 60f; //seconds between price changes

    //boat cost = 100, 200, 400, 800...
    public double GetBoatCost(int boatTier)
    {
        return boat1BaseCost * System.Math.Pow(boatCostMultiplier, boatTier - 1);
    }

    //base fish/sec = 1, 2, 4, 8...
    public double GetBoatBaseGeneration(int boatTier)
    {
        return boat1BaseGeneration * System.Math.Pow(boatGenerationMultiplier, boatTier - 1);
    }

    //upgrade cost scales by tier and by how many levels you already bought
    public double GetUpgradeCost(int boatTier, int currentLevel)
    {
        double baseCost = boat1BaseCost * boatTier * 1.5;
        return baseCost * System.Math.Pow(upgradeCostMultiplier, currentLevel);
    }

    //tap upgrade cost = 50, 80, 128, 205... steep because it's capped
    public double GetClickUpgradeCost(int level)
    {
        return clickUpgradeBaseCost * System.Math.Pow(clickUpgradeCostMultiplier, level);
    }

    //prestige cost = 2k, 6k, 18k...
    public double GetPrestigeCost(int prestigeLevel)
    {
        return firstPrestigeCost * System.Math.Pow(prestigeCostMultiplier, prestigeLevel - 1);
    }

    //prestige 0 = 1x, prestige 1 = 1.5x, prestige 2 = 2.25x...
    public double GetPrestigeMultiplier(int prestigeLevel)
    {
        return System.Math.Pow(prestigeMultiplier, prestigeLevel);
    }
}
