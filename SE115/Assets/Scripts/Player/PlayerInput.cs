using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [System.Serializable]
    public class KeyConfig
    {
        [Header("Movement")]
        public KeyCode jump = KeyCode.Z;
        public KeyCode dash = KeyCode.C;

        [Header("Combat")]
        public KeyCode attack = KeyCode.X;

        [Header("Skills")]
        public KeyCode[] skillsKey;
    }

    public KeyConfig keys;

    public Vector2 moveInput { get; private set; }

    public bool isJumpPressed { get; private set; } = false;
    public bool isJumpPressUp { get; private set; } = false;
    public bool isAttackPressed { get; private set; } = false;
    public bool isDashPressed { get; private set; } = false;
    public bool[] isSkillPressed { get; private set;}
    private void Awake()
    {
        isSkillPressed = new bool[keys.skillsKey.Length];
    }
    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y);

        if (Input.GetKeyDown(keys.jump))
        {
            isJumpPressed = true;
        }
        if (Input.GetKeyUp(keys.jump))
        {
            isJumpPressUp = true;
        }

        if (Input.GetKeyDown(keys.attack))
        {
            isAttackPressed = true;
        }

        if (Input.GetKeyDown(keys.dash))
        {
            isDashPressed = true;
        }

        for (int i = 0; i < keys.skillsKey.Length; i++)
        {
            if (Input.GetKeyDown(keys.skillsKey[i]))
                isSkillPressed[i] = true;
        }
    }

    public int CheckSkillPressed()
    {
        for (int i = 0; i < keys.skillsKey.Length; i++)
            if (isSkillPressed[i])  return i;
        return -1;
    }
    public void ResetJumpPressed() => isJumpPressed = false;
    public void ResetJumpPressUp() => isJumpPressUp = false;
    public void ResetAttackPressed() => isAttackPressed = false;
    public void ResetDashPressed() => isDashPressed = false;
    public void ResetSkillPressed(int index) => isSkillPressed[index] = false;
}