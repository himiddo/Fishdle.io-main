using UnityEngine;
using UnityEngine.UI;
using TMPro;

//sell window contents = live price, a countdown to the next price change, and a sell button
public class SellWindowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Button sellButton;

    void OnEnable()
    {
        if (sellButton != null) sellButton.onClick.AddListener(OnSell);
    }

    void OnDisable()
    {
        if (sellButton != null) sellButton.onClick.RemoveListener(OnSell);
    }

    void Update()
    {
        Refresh(); //price and timer both tick live
    }

    private void OnSell()
    {
        if (MoneyManager.instance != null) MoneyManager.instance.SellAllFish();
    }

    private void Refresh()
    {
        if (PriceManager.instance == null || SaveManager.instance == null) return;

        if (priceText != null)
            priceText.text = "Price: " + NumberFormatter.FormatMoney(PriceManager.instance.EffectivePrice) + "/fish";
        if (countdownText != null)
            countdownText.text = "Next change in " + Mathf.CeilToInt(PriceManager.instance.TimeUntilNextPriceChange) + "s";
        if (sellButton != null)
            sellButton.interactable = SaveManager.instance.data.fishCount > 0d; //nothing to sell = greyed
    }
}
