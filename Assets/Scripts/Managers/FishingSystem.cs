using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

//click the ocean to catch a fish
public class FishingSystem : MonoBehaviour
{
    public static FishingSystem instance;

    private Camera cam;
    private int oceanMask;

    void Awake()
    {
        instance = this;
        cam = Camera.main;
        oceanMask = LayerMask.GetMask("Ocean"); //only clicks on water should count
    }

    //fish gained per tap = base + tap upgrades, then fossil fish bonuses
    public double GetFishPerTap()
    {
        GameConfig cfg = ConfigManager.instance.config;
        double raw = cfg.fishPerClick + SaveManager.instance.data.clickLevel * cfg.clickUpgradeBonus;
        return raw * StatsManager.instance.GetFishMultiplier();
    }

    //buy the next tap upgrade = capped, unlike boats
    public bool TryUpgradeTap()
    {
        SaveData data = SaveManager.instance.data;
        GameConfig cfg = ConfigManager.instance.config;
        if (data.clickLevel >= cfg.maxClickLevel) return false;

        double cost = cfg.GetClickUpgradeCost(data.clickLevel);
        if (!MoneyManager.instance.TrySpend(cost)) return false;

        data.clickLevel += 1;
        StatsManager.instance.RaiseStatsChanged();
        return true;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || cam == null) return;

        //don't fish while the fossil minigame is running (that click is for the bar)
        if (FossilMinigame.instance != null && FossilMinigame.instance.IsActive) return;

        //only act the frame the left button goes down
        if (!mouse.leftButton.wasPressedThisFrame) return;

        //ignore clicks that land on ui = the counters, buttons, etc.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 worldPoint = cam.ScreenToWorldPoint(mouse.position.ReadValue());
        if (Physics2D.OverlapPoint(worldPoint, oceanMask) != null)
            CatchFish();
    }

    //add fish to the stockpile and refresh the ui
    private void CatchFish()
    {
        SaveData data = SaveManager.instance.data;
        double caught = GetFishPerTap();
        data.fishCount += caught;
        data.totalFishCaught += caught;
        StatsManager.instance.RaiseStatsChanged();
        //TODO: little "+1" popup + catch sound for feedback
    }
}
