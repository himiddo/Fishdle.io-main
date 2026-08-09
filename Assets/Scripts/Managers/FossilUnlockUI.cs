using UnityEngine;

//keeps the fossils button hidden until fossils actually unlock
//lives on the canvas so it stays running while the button itself is switched off
public class FossilUnlockUI : MonoBehaviour
{
    [SerializeField] private GameObject fossilsButton;

    void Update()
    {
        if (fossilsButton == null || FossilManager.instance == null) return;

        bool unlocked = FossilManager.instance.FossilsUnlocked();
        if (fossilsButton.activeSelf != unlocked) fossilsButton.SetActive(unlocked);
    }
}
