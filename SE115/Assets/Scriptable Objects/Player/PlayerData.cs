using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength = -52.0f; //Increase if want jump higher
    [HideInInspector] public float gravityScale; //Player rigidbody gravity scale

    [Space(5)]

    public float fallGravityScaleMulti;  //multiple gravity scale when falling
    public float maxFallSpeed; 

    [Space(5)]

    [Header("Slide")]
    public float slideGravityScaleMulti;
    public float maxSlideSpeed;

    [Header("Run")]

    public float runMaxSpeed;      
    public float runAcceleration;   
    [HideInInspector] public float runAccelAmount = 50; //The actual force (multiplied with speedDiff) applied to the player.
    public float runDecceleration;      // Accel = Deccel to stop instant
    [HideInInspector] public float runDeccelAmount = 50; //Actual force (multiplied with speedDiff) applied to the player 

    [Space(5)]

    [Range(0f, 1)] public float accelInAir;       //Accel multiple when on airbound
    [Range(0f, 1)] public float deccelInAir;

    [Space(5)]

    [Header("Jump")]
    public float jumpHeight; 
    public float jumpTimeToApex;    //Time to reach jump height
    [HideInInspector] public float jumpForce;

    [Space(5)]

    [Header("Advanced Jumps")]
    public float jumpHangTimeThreshold; //Reduce thehe player's velocity.y closest to 0 at the jump's highest point

    [Space(5)]

    [Header("Assists")]
    [Range(0.0f, 1.0f)] public float coyoteTime; 
    [Range(0.0f, 1.0f)] public float jumpInputBufferTime;

    [Space(20)]

    [Header("Dash")]
    public int dashCountAmount;
    [Space(5)]
    public float dashSpeed;
    public float dashTime;
    [Space(5)]
    public float dashEndTime;
    [Space(5)]
    public float dashCooldownTime;
    [Space(5)]
    [Range(0.01f, 0.5f)] public float dashInputBufferTime;

    private void OnValidate()
    {
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);    // g = 2s/t^2, s = jumpheight, t = jumptime to apex, cong thuc tinh gia toc chuyen dong roi tu do, chieu di tu duoi len
        
        gravityScale = gravityStrength / Physics2D.gravity.y;

        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;    //v = gt

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
    }
}
