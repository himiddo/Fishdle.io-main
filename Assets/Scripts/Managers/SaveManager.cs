using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//everything we need to remember between sessions lives in here
//grows as we add systems = boats, fossils, unlocked slots, etc.
//one owned boat = its tier, upgrade level, and where its visual sits in the water
[Serializable]
public class BoatData
{
    public int tier;
    public int level;
    public float posX;
    public float posY;
}

//the five fossil buff types
public enum FossilStat { FishMultiplier, MoneyMultiplier, SpawnRate, BarSlowdown, PriceModifier }

//one collected fossil = which buff it gives, how strong, and whether it's equipped
[Serializable]
public class FossilData
{
    public FossilStat stat;
    public float value;
    public bool equipped;
}

[Serializable]
public class SaveData
{
    //doubles, not floats = float dies at ~16.7 million (fishCount += 1 stops doing anything)
    public double money = 0d;
    public double fishCount = 0d; //fish caught but not sold yet = what the counter shows
    public double totalFishCaught = 0d; //lifetime total, kept for stats
    public int prestigeLevel = 0;
    public int clickLevel = 0; //tap upgrades = permanent, deliberately not reset by prestige
    public List<BoatData> boats = new List<BoatData>(); //every boat you currently own
    public List<FossilData> fossils = new List<FossilData>(); //every fossil you've collected
    public long lastSaveTicks = 0; //timestamp = used later for idle/offline earnings
}

//handles writing and reading the save file = SaveManager.instance.Save()
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public SaveData data = new SaveData();

    //full path to the save file on disk (per-user, survives updates)
    private string SavePath => Path.Combine(Application.persistentDataPath, "fishdle_save.json");

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Load(); //pull existing progress the moment the game starts
    }

    //serialize current data to json and write it out
    public void Save()
    {
        data.lastSaveTicks = DateTime.UtcNow.Ticks;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    //read the json back if a save exists, otherwise start fresh
    public void Load()
    {
        if (!File.Exists(SavePath)) { data = new SaveData(); return; }
        string json = File.ReadAllText(SavePath);
        data = JsonUtility.FromJson<SaveData>(json);
    }

    //wipe progress = for a hard reset or while testing
    public void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        data = new SaveData();
    }

    //auto-save so progress isn't lost when the app is backgrounded or closed
    void OnApplicationPause(bool paused) { if (paused) Save(); }
    void OnApplicationQuit() { Save(); }
}
