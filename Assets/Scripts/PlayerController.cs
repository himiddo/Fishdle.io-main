using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    //base movement

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

   //every frame updated = reads input
    void Update()
    {
        //freeze while the fossil minigame is running = can't wander into other fossils
        if (FossilMinigame.instance != null && FossilMinigame.instance.IsActive)
        {
            moveInput = Vector2.zero;
            return;
        }

        Keyboard kb = Keyboard.current;
        if (kb == null) { moveInput = Vector2.zero; return; }

        float x = 0f;
        float y = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y += 1f;

        moveInput = new Vector2(x, y);
        if (moveInput.sqrMagnitude > 1f)      
            moveInput = moveInput.normalized;
        //statement that wont allow diagonal moving to befaster
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
    //applies physics and balanced movement without being dependant on client
}