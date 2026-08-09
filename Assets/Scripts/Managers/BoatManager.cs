using System.Collections.Generic;
using UnityEngine;

//owns the boats = buying, upgrading, generating fish, and spawning their visuals
public class BoatManager : MonoBehaviour
{
    public static BoatManager instance;

    [Header("Boat Visual")]
    [SerializeField] private GameObject boatPrefab;                      //boat sprite to drop in the water
    [SerializeField] private Vector2 oceanMin = new Vector2(1f, -3.5f);  //spawn area corner
    [SerializeField] private Vector2 oceanMax = new Vector2(8.5f, 3.5f); //spawn area corner

    private readonly List<GameObject> spawnedVisuals = new List<GameObject>();

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    void Start()
    {
        //show a boat for everything we already own from the save
        foreach (BoatData b in SaveManager.instance.data.boats)
            SpawnBoatVisual(b);
    }

    //unlocked boat slots = one more per prestige tier, no ceiling
    public int MaxSlots => StatsManager.instance.prestigeLevel + 1;

    //find an owned boat by tier, or null if we don't have it
    public BoatData GetBoat(int tier)
    {
        foreach (BoatData b in SaveManager.instance.data.boats)
            if (b.tier == tier) return b;
        return null;
    }

    public bool OwnsBoat(int tier) => GetBoat(tier) != null;

    //buy a boat = must be unowned, within an unlocked slot, and affordable
    public bool TryBuyBoat(int tier)
    {
        SaveData data = SaveManager.instance.data;
        if (OwnsBoat(tier)) return false;
        if (tier > MaxSlots) return false; //this tier's slot isn't unlocked yet

        double cost = ConfigManager.instance.config.GetBoatCost(tier);
        if (!MoneyManager.instance.TrySpend(cost)) return false;

        //pick a spot in the water and remember it so it stays put across reloads
        BoatData boat = new BoatData
        {
            tier = tier,
            level = 0,
            posX = Random.Range(oceanMin.x, oceanMax.x),
            posY = Random.Range(oceanMin.y, oceanMax.y)
        };
        data.boats.Add(boat);
        SpawnBoatVisual(boat);
        StatsManager.instance.RaiseStatsChanged();
        return true;
    }

    //upgrade a boat = +1 level for a cost that scales with the level
    public bool TryUpgradeBoat(int tier)
    {
        BoatData boat = GetBoat(tier);
        if (boat == null) return false;

        double cost = ConfigManager.instance.config.GetUpgradeCost(tier, boat.level);
        if (!MoneyManager.instance.TrySpend(cost)) return false;

        boat.level += 1;
        StatsManager.instance.RaiseStatsChanged();
        return true;
    }

    //total fish/sec from every boat, before fossil and prestige boosts
    public double GetRawBoatFishPerSecond()
    {
        GameConfig cfg = ConfigManager.instance.config;
        double total = 0d;
        foreach (BoatData b in SaveManager.instance.data.boats)
            total += cfg.GetBoatBaseGeneration(b.tier) * (1d + b.level * cfg.upgradeBonus); //upgrades scale with the boat
        return total;
    }

    void Update()
    {
        //feed the stats hub, then pour fish into the stockpile
        StatsManager.instance.boatFishPerSecond = GetRawBoatFishPerSecond();
        double rate = StatsManager.instance.GetFishPerSecond();
        if (rate > 0d)
            SaveManager.instance.data.fishCount += rate * Time.deltaTime;
    }

    //drop a boat sprite into the water at its saved position
    private void SpawnBoatVisual(BoatData boat)
    {
        if (boatPrefab == null) return;

        //older saves have no stored position = give it one now
        if (boat.posX == 0f && boat.posY == 0f)
        {
            boat.posX = Random.Range(oceanMin.x, oceanMax.x);
            boat.posY = Random.Range(oceanMin.y, oceanMax.y);
        }

        GameObject vis = Instantiate(boatPrefab, new Vector3(boat.posX, boat.posY, 0f), Quaternion.identity);
        vis.name = "Boat " + boat.tier + " (visual)";
        spawnedVisuals.Add(vis);
    }
}
