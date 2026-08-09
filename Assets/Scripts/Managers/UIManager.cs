using UnityEngine;
using TMPro;

//keeps the on-screen readouts in sync = money, fish count, and price per fish
public class UIManager : MonoBehaviour
{
    [Header("Displays")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text fishCountText;
    [SerializeField] private TMP_Text priceText;

    //cache the last shown values so we only rebuild the text when a number actually changes
    private double lastMoney = double.NaN;
    private double lastFishCount = double.NaN;
    private double lastPrice = double.NaN;

    void OnEnable()
    {
        //refresh the moment any system reports a change
        if (StatsManager.instance != null)
            StatsManager.instance.OnStatsChanged += Refresh;
    }

    void OnDisable()
    {
        if (StatsManager.instance != null)
            StatsManager.instance.OnStatsChanged -= Refresh;
    }

    void Start()
    {
        Refresh(); //show correct values on the very first frame
    }

    void Update()
    {
        Refresh(); //catches values that tick up continuously, like fish from boats
    }

    //only rewrites a label when its number moved = no wasted garbage each frame
    private void Refresh()
    {
        double money = SaveManager.instance != null ? SaveManager.instance.data.money : 0d;
        double fishCount = SaveManager.instance != null ? SaveManager.instance.data.fishCount : 0d;
        double price = PriceManager.instance != null ? PriceManager.instance.EffectivePrice : 0d;

        if (money != lastMoney)
        {
            lastMoney = money;
            if (moneyText != null) moneyText.text = NumberFormatter.FormatMoney(money);
        }

        if (fishCount != lastFishCount)
        {
            lastFishCount = fishCount;
            //floor it so the counter reads clean whole fish, not "12.34 fish"
            if (fishCountText != null) fishCountText.text = NumberFormatter.Format(System.Math.Floor(fishCount)) + " fish";
        }

        if (price != lastPrice)
        {
            lastPrice = price;
            if (priceText != null) priceText.text = NumberFormatter.FormatMoney(price) + "/fish";
        }
    }
}
