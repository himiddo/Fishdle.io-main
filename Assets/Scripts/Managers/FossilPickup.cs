using UnityEngine;
using UnityEngine.InputSystem;

//sits on a spawned fossil = walk up to it, then click to start the minigame
public class FossilPickup : MonoBehaviour
{
    private bool triggered;
    private bool inRange;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) inRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) inRange = false;
    }

    void Update()
    {
        if (triggered || !inRange) return;                                                //must be standing on it
        if (FossilMinigame.instance == null || FossilMinigame.instance.IsActive) return;  //one minigame at a time

        Mouse m = Mouse.current;
        if (m == null || !m.leftButton.wasPressedThisFrame) return;                        //wait for a click

        triggered = true;
        FossilMinigame.instance.Begin(success =>
        {
            if (success) FossilManager.instance.CollectRandomFossil();
            Destroy(gameObject); //consumed whether you win or lose
        });
    }
}
