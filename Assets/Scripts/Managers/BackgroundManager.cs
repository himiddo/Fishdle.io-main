using UnityEngine;

//swaps the beach background to match the player's real-world time of day
//day 6am-3pm, sunset 3pm-9pm, night 9pm-6am (rolls over midnight)
public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer target; //the background renderer we swap the sprite on
    [SerializeField] private Sprite daySprite;
    [SerializeField] private Sprite sunsetSprite;
    [SerializeField] private Sprite nightSprite;

    private int lastHour = -1; //remember the last hour so we only swap when it actually changes

    void Start()
    {
        Apply(); //show the correct background straight away on load
    }

    void Update()
    {
        //cheap to check the clock each frame, but only bother swapping when the hour ticks over
        if (System.DateTime.Now.Hour != lastHour) Apply();
    }

    //pick the sprite that matches the current hour and drop it in
    private void Apply()
    {
        if (target == null) return;

        int hour = System.DateTime.Now.Hour;
        lastHour = hour;

        Sprite chosen;
        if (hour >= 6 && hour < 15)         //6am - 3pm = day
            chosen = daySprite;
        else if (hour >= 15 && hour < 21)   //3pm - 9pm = sunset
            chosen = sunsetSprite;
        else                                 //9pm - 6am = night
            chosen = nightSprite;

        if (chosen != null) target.sprite = chosen;
    }
}
