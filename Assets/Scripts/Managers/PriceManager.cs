using UnityEngine;

//sets the price per fish and reshuffles it every so often
public class PriceManager : MonoBehaviour
{
    public static PriceManager instance;
    public float currentPrice { get; private set; }

    //price after flat fossil price modifiers = what you actually sell at
    public float EffectivePrice
    {
        get
        {
            float mod = StatsManager.instance != null ? StatsManager.instance.fossilPriceModifier : 0f;
            return currentPrice + mod;
        }
    }

    //seconds until the price rerolls = drives the sell window countdown
    public float TimeUntilNextPriceChange
    {
        get
        {
            float interval = ConfigManager.instance != null ? ConfigManager.instance.config.priceChangeInterval : 60f;
            return Mathf.Max(0f, interval - timer);
        }
    }

    private float timer;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    void Start()
    {
        RollNewPrice(); //have a price ready so selling works from the start
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= ConfigManager.instance.config.priceChangeInterval)
        {
            timer = 0f;
            RollNewPrice();
        }
    }

    //pick a fresh random price inside the configured range
    private void RollNewPrice()
    {
        GameConfig cfg = ConfigManager.instance.config;
        currentPrice = Random.Range(cfg.minPrice, cfg.maxPrice);
        StatsManager.instance.RaiseStatsChanged(); //let the ui show the new price
    }
}
