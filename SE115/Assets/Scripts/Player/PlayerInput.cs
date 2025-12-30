using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;
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

        [Header("Others")]
        public KeyCode interact = KeyCode.F;
        public KeyCode close = KeyCode.Escape;
    }

    public KeyConfig keys;

    public Vector2 moveInput { get; private set; }

    public bool isJumpPressed { get; private set; } = false;
    public bool isAttackPressed { get; private set; } = false;
    public bool isDashPressed { get; private set; } = false;
    public bool isInteractPressed { get; private set; } = false;
    public bool[] isSkillPressed { get; private set; }

    [Tooltip("Reset input after buffer time")]
    public float skillInputBufferTime = 0.2f;

    private float[] skillInputTimer;

    private void Awake()
    {
        Instance = this;

        isSkillPressed = new bool[keys.skillsKey.Length];
        skillInputTimer = new float[keys.skillsKey.Length];
    }
    private void Update()
    {
        if (ScrollMessenger.Instance != null && ScrollMessenger.Instance.IsShowingMessage())
        {
            if (Input.GetKeyDown(keys.close))
            {
                ScrollMessenger.Instance.CloseMessage();
            }

            moveInput = Vector2.zero;

            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y);

        if (Input.GetKeyDown(keys.jump))
        {
            isJumpPressed = true;
        }

        if (Input.GetKeyDown(keys.attack))
        {
            isAttackPressed = true;
        }

        if (Input.GetKeyDown(keys.dash))
        {
            isDashPressed = true;
        }
        if(Input.GetKeyDown(keys.interact))
        {
            isInteractPressed = true;
        }
        for (int i = 0; i < keys.skillsKey.Length; i++)
        {
            if (Input.GetKeyDown(keys.skillsKey[i]))
            {
                isSkillPressed[i] = true;
                skillInputTimer[i] = 0.0f;
            }
            else
            {
                skillInputTimer[i] += Time.deltaTime;

                if (skillInputTimer[i] > skillInputBufferTime)
                    isSkillPressed[i] = false;
            }
        }
    }

    public int CheckSkillPressed()
    {
        for (int i = 0; i < keys.skillsKey.Length; i++)
            if (isSkillPressed[i]) return i;
        return -1;
    }
    public void ResetJumpPressed() => isJumpPressed = false;
    public void ResetAttackPressed() => isAttackPressed = false;
    public void ResetDashPressed() => isDashPressed = false;
    public void ResetInteractPressed() => isInteractPressed = false;
    public void ResetSkillPressed(int index) => isSkillPressed[index] = false;

    #region Helper function for UI and event
    public KeyCode GetKeyForSkill(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < keys.skillsKey.Length)
            return keys.skillsKey[skillIndex];
        return KeyCode.None;
    }
    #endregion
}