using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//fossil inventory menu = scrollable list of collected fossils, equip up to 5
public class FossilMenuUI : MonoBehaviour
{
    [SerializeField] private Transform content;    //scroll view content parent
    [SerializeField] private GameObject rowPrefab; //one fossil row template
    [SerializeField] private TMP_Text headerText;  //"Equipped 2/5"

    private readonly List<GameObject> rows = new List<GameObject>();

    void OnEnable()
    {
        Refresh(); //rebuild the list every time the menu opens
    }

    //redraw a row for each collected fossil
    public void Refresh()
    {
        foreach (GameObject r in rows) Destroy(r);
        rows.Clear();

        if (SaveManager.instance == null || rowPrefab == null || content == null) return;

        foreach (FossilData f in SaveManager.instance.data.fossils)
        {
            GameObject row = Instantiate(rowPrefab, content);
            row.SetActive(true);

            Transform labelT = row.transform.Find("Label");
            if (labelT != null) labelT.GetComponent<TMP_Text>().text = Describe(f);

            Button btn = row.transform.Find("EquipButton").GetComponent<Button>();
            Transform btnTextT = btn.transform.Find("Text");
            if (btnTextT != null) btnTextT.GetComponent<TMP_Text>().text = f.equipped ? "Unequip" : "Equip";

            FossilData captured = f; //capture this fossil for the click
            btn.onClick.AddListener(() => Toggle(captured));

            rows.Add(row);
        }
        UpdateHeader();
    }

    //equip if there's room, unequip if it's already on
    private void Toggle(FossilData f)
    {
        if (f.equipped) FossilManager.instance.Unequip(f);
        else FossilManager.instance.TryEquip(f); //silently no-ops when 5 are already equipped
        Refresh();
    }

    private void UpdateHeader()
    {
        if (headerText == null) return;
        int max = ConfigManager.instance.config.maxEquippedFossils;
        headerText.text = "Equipped " + FossilManager.instance.EquippedCount() + "/" + max;
    }

    //readable one-line summary of a fossil's buff
    private string Describe(FossilData f)
    {
        string tag = f.equipped ? "[E]  " : "";
        switch (f.stat)
        {
            case FossilStat.FishMultiplier:  return tag + "+" + (f.value * 100f).ToString("0") + "% fish";
            case FossilStat.MoneyMultiplier: return tag + "+" + (f.value * 100f).ToString("0") + "% money";
            case FossilStat.SpawnRate:       return tag + "-" + (f.value * 100f).ToString("0") + "% fossil timer";
            case FossilStat.BarSlowdown:     return tag + "-" + (f.value * 100f).ToString("0") + "% bar speed";
            case FossilStat.PriceModifier:   return tag + "+$" + f.value.ToString("0.00") + " per fish";
        }
        return tag + "?";
    }
}
