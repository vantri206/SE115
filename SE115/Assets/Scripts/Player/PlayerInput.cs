using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    /* Move Input */
    public Vector2 moveInput { get; set; }

    /* Jump Input */
    public bool isJumpPressed { get; private set; } = false;
    public bool isJumpPressUp { get; private set; } = false;

    /* Attack Input */
    public bool isAttackPressed { get; private set; } = false;

    /* Dash input */
    public bool isDashPressed { get; private set; } = false;
    private void Awake()
    {

    }
    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            isJumpPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            isJumpPressUp = true;
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            isAttackPressed = true;
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            isDashPressed = true;
        }
    }
    public void ResetJumpPressed()
    {
        isJumpPressed = false;
    }
    public void ResetJumpPressUp()
    {
        isJumpPressUp = false;
    }
    public void ResetAttackPressed()
    {
        isAttackPressed = false;
    }
    public void ResetDashPressed()
    {
        isDashPressed = false;
    }
}
