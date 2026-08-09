using System.Collections.Generic;
using UnityEngine;

//spawns fossils on the beach, rolls a random one on collect, and applies equipped buffs
public class FossilManager : MonoBehaviour
{
    public static FossilManager instance;

    [Header("Spawning")]
    [SerializeField] private GameObject fossilPrefab;
    [SerializeField] private Sprite[] fossilSprites; //visual variety, one picked at random per spawn
    [SerializeField] private Vector2 sandMin = new Vector2(-9f, -4.5f);  //beach spawn corner
    [SerializeField] private Vector2 sandMax = new Vector2(-5.3f, 4.5f); //beach spawn corner
    [SerializeField] private float baseSpawnInterval = 45f;
    [SerializeField] private int maxActiveFossils = 3;

    private readonly List<GameObject> active = new List<GameObject>();
    private float timer;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    void Start()
    {
        RecalculateEquippedBonuses(); //apply whatever was equipped in the save
    }

    void Update()
    {
        if (!FossilsUnlocked()) return; //nothing spawns until you've prestiged enough

        timer += Time.deltaTime;
        if (timer >= CurrentSpawnInterval())
        {
            timer = 0f;
            TrySpawnFossil();
        }
    }

    //fossils stay out of the early game until the configured prestige level
    public bool FossilsUnlocked()
    {
        if (SaveManager.instance == null || ConfigManager.instance == null) return false;
        return SaveManager.instance.data.prestigeLevel >= ConfigManager.instance.config.fossilUnlockPrestige;
    }

    //spawn interval shrinks with equipped spawn-rate fossils, down to a 5s floor
    private float CurrentSpawnInterval()
    {
        float factor = StatsManager.instance != null ? StatsManager.instance.fossilSpawnFactor : 1f;
        return Mathf.Max(5f, baseSpawnInterval * factor); //compounds, so no fossil is ever wasted
    }

    private void TrySpawnFossil()
    {
        if (fossilPrefab == null) return;
        active.RemoveAll(g => g == null); //drop collected/destroyed ones
        if (active.Count >= maxActiveFossils) return;

        Vector3 pos = new Vector3(Random.Range(sandMin.x, sandMax.x), Random.Range(sandMin.y, sandMax.y), 0f);
        GameObject go = Instantiate(fossilPrefab, pos, Quaternion.identity);
        if (fossilSprites != null && fossilSprites.Length > 0)
        {
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = fossilSprites[Random.Range(0, fossilSprites.Length)]; //random look
        }
        active.Add(go);
    }

    //called by the minigame on success = roll a fresh fossil into the inventory
    public void CollectRandomFossil()
    {
        SaveManager.instance.data.fossils.Add(RollRandomFossil());
        StatsManager.instance.RaiseStatsChanged();
    }

    //random buff type + a value inside that stat's range
    public static FossilData RollRandomFossil()
    {
        FossilData f = new FossilData();
        f.stat = (FossilStat)Random.Range(0, 5);
        switch (f.stat)
        {
            case FossilStat.FishMultiplier:  f.value = Random.Range(0.02f, 0.40f); break; //+2..40% fish
            case FossilStat.MoneyMultiplier: f.value = Random.Range(0.02f, 0.40f); break; //+2..40%
            case FossilStat.SpawnRate:       f.value = Random.Range(0.05f, 0.30f); break; //5..30% faster spawns
            case FossilStat.BarSlowdown:     f.value = Random.Range(0.05f, 0.25f); break; //5..25% slower bar
            case FossilStat.PriceModifier:   f.value = Random.Range(0.2f, 1.5f); break;  //+$0.2..1.5
        }
        return f;
    }

    //how many fossils are currently equipped
    public int EquippedCount()
    {
        int n = 0;
        foreach (FossilData f in SaveManager.instance.data.fossils) if (f.equipped) n++;
        return n;
    }

    //equip a fossil = blocked if the 5 slots are full
    public bool TryEquip(FossilData f)
    {
        if (f.equipped) return true;
        if (EquippedCount() >= ConfigManager.instance.config.maxEquippedFossils) return false;
        f.equipped = true;
        RecalculateEquippedBonuses();
        return true;
    }

    public void Unequip(FossilData f)
    {
        if (!f.equipped) return;
        f.equipped = false;
        RecalculateEquippedBonuses();
    }

    //sum up every equipped fossil's buff and push the totals into the stats hub
    public void RecalculateEquippedBonuses()
    {
        StatsManager s = StatsManager.instance;
        s.fossilFishMultiplier = 0f;
        s.fossilSellBonus = 0f;
        s.fossilSpawnFactor = 1f; //multiplied down once per equipped fossil
        s.fossilBarFactor = 1f;
        s.fossilPriceModifier = 0f;

        foreach (FossilData f in SaveManager.instance.data.fossils)
        {
            if (!f.equipped) continue;
            switch (f.stat)
            {
                case FossilStat.FishMultiplier:  s.fossilFishMultiplier += f.value; break;
                case FossilStat.MoneyMultiplier: s.fossilSellBonus += f.value; break;
                case FossilStat.SpawnRate:       s.fossilSpawnFactor *= (1f - f.value); break; //compounds
                case FossilStat.BarSlowdown:     s.fossilBarFactor *= (1f - f.value); break;
                case FossilStat.PriceModifier:   s.fossilPriceModifier += f.value; break;
            }
        }
        s.RaiseStatsChanged();
    }
}
