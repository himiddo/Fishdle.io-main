using System.Collections.Generic;
using UnityEngine;

//builds the boat rows = one per unlocked tier, plus a preview of the next locked one
//boats are infinite now, so the list grows with every prestige instead of stopping at 4
public class BoatMenuList : MonoBehaviour
{
    [SerializeField] private Transform content;    //scroll view content parent
    [SerializeField] private GameObject rowPrefab; //boat row template

    private readonly List<GameObject> rows = new List<GameObject>();

    void OnEnable()
    {
        Rebuild(); //refresh every time the window opens
    }

    //one row per unlocked tier plus one locked teaser so you can see what's next
    private void Rebuild()
    {
        foreach (GameObject r in rows) Destroy(r);
        rows.Clear();

        if (BoatManager.instance == null || rowPrefab == null || content == null) return;

        int shown = BoatManager.instance.MaxSlots + 1;
        for (int tier = 1; tier <= shown; tier++)
        {
            GameObject row = Instantiate(rowPrefab, content);
            row.SetActive(true);
            BoatMenuUI menu = row.GetComponent<BoatMenuUI>();
            if (menu != null) menu.SetTier(tier);
            rows.Add(row);
        }
    }
}
