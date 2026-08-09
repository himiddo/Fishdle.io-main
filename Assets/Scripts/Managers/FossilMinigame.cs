using System;
using UnityEngine;
using UnityEngine.InputSystem;

//timing bar minigame = a line sweeps across the bar, click anywhere to stop it on the green
public class FossilMinigame : MonoBehaviour
{
    public static FossilMinigame instance;

    [Header("UI")]
    [SerializeField] private GameObject panel;        //whole minigame ui, hidden until needed
    [SerializeField] private RectTransform bar;       //the track the line travels along
    [SerializeField] private RectTransform greenZone; //success zone (child of the bar)
    [SerializeField] private RectTransform indicator; //the moving line (child of the bar)
    [SerializeField] private float greenWidthFraction = 0.18f;

    private bool active;
    private float pos;   //0..1 along the bar
    private int dir = 1;
    private float speed;
    private float greenMin, greenMax;
    private Action<bool> onDone;
    private int startFrame; //frame the minigame opened, so the opening click isn't read as the stop

    public bool IsActive => active;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        if (panel != null) panel.SetActive(false);
    }

    //start a fresh attempt = the result comes back through the callback
    public void Begin(Action<bool> callback)
    {
        onDone = callback;
        active = true;
        startFrame = Time.frameCount;
        pos = 0f; dir = 1;

        //random speed, eased by equipped bar-slowdown fossils (compounds, never reaches zero)
        float baseSpeed = UnityEngine.Random.Range(0.5f, 1.1f);
        float factor = StatsManager.instance != null ? StatsManager.instance.fossilBarFactor : 1f;
        speed = baseSpeed * factor;

        //random green zone position
        float center = UnityEngine.Random.Range(greenWidthFraction, 1f - greenWidthFraction);
        greenMin = center - greenWidthFraction * 0.5f;
        greenMax = center + greenWidthFraction * 0.5f;
        PlaceGreenZone();

        if (panel != null) panel.SetActive(true);
    }

    void Update()
    {
        if (!active) return;

        //bounce the line back and forth
        pos += dir * speed * Time.deltaTime;
        if (pos >= 1f) { pos = 1f; dir = -1; }
        else if (pos <= 0f) { pos = 0f; dir = 1; }
        PlaceIndicator();

        //click anywhere to stop
        Mouse m = Mouse.current;
        if (m != null && m.leftButton.wasPressedThisFrame && Time.frameCount > startFrame)
            Stop();
    }

    private void Stop()
    {
        active = false;
        if (panel != null) panel.SetActive(false);
        bool success = pos >= greenMin && pos <= greenMax;
        onDone?.Invoke(success);
    }

    //green zone and line are children of the bar, positioned across its width from the center
    private void PlaceGreenZone()
    {
        if (bar == null || greenZone == null) return;
        float w = bar.rect.width;
        greenZone.anchoredPosition = new Vector2((greenMin + greenMax) * 0.5f * w - w * 0.5f, 0f);
        greenZone.sizeDelta = new Vector2((greenMax - greenMin) * w, greenZone.sizeDelta.y);
    }

    private void PlaceIndicator()
    {
        if (bar == null || indicator == null) return;
        float w = bar.rect.width;
        indicator.anchoredPosition = new Vector2(pos * w - w * 0.5f, 0f);
    }
}
