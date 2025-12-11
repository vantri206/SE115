using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Component")]
    public PlayerController player;
    public new Rigidbody2D rigidbody;
    private PlayerData data;

    private PlayerInput input;
    private void Awake()
    {
        this.player = GetComponent<PlayerController>();
        this.rigidbody = player.myRigidbody;

        this.input = player.input;
        this.data = player.data;
    }
    private void Start()
    {

    }
    private void Update()
    {
        if (player.isAttacking)
        {
            player.SetGravityScale(0.0f);
            return;
        }
        else if(player.isDashing)
        {
            player.SetGravityScale(0.0f);
            return;
        }

        if ((player.isJumping || player.IsFalling()) && Mathf.Abs(rigidbody.linearVelocity.y) < data.jumpHangTimeThreshold)
        {
            player.SetGravityScale(data.gravityScale);
        }
        else if(player.isSliding)
        {
            player.SetGravityScale(data.slideGravityScaleMulti);
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, Mathf.Max(rigidbody.linearVelocity.y, -data.maxSlideSpeed));
        }
        else if (rigidbody.linearVelocity.y < 0)
        {
            player.SetGravityScale(data.gravityScale * data.fallGravityScaleMulti);
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, Mathf.Max(rigidbody.linearVelocity.y, -data.maxFallSpeed));
        }
        else
        {
            player.SetGravityScale(data.gravityScale);
        }
    }
    public void HorizonMoving(float moveLerf)
    {
        float maxSpeed = input.moveInput.x * data.runMaxSpeed;
        maxSpeed = Mathf.Lerp(rigidbody.linearVelocityX, maxSpeed, moveLerf);

        float accelScale;

        accelScale = (Mathf.Abs(maxSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;

        float speedDiff = maxSpeed - rigidbody.linearVelocity.x;

        float movement = speedDiff * accelScale;

        rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }
    public void Jump()
    {
        float force = data.jumpForce;

        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, 0.0f);

        rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        player.effect.PlayJumpEffect();
    }
    public void WallJump()
    {
        Vector2 direction = new Vector2((-1) * player.facingDirection.x, 1.0f);

        Vector2 force = new Vector2(0.25f * data.jumpForce, data.jumpForce);

        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, 0.0f);

        rigidbody.AddForce(direction * force, ForceMode2D.Impulse);

        player.CheckFacingDirection(direction);

        player.effect.PlayJumpEffect();
    }
    public void Dash()
    {
        player.myRigidbody.linearVelocity = new Vector2(player.facingDirection.x * data.dashSpeed, 0.0f);
    }
    public void StopMovingHorzion()
    {
        player.myRigidbody.linearVelocity = new Vector2(0.0f, player.myRigidbody.linearVelocity.y);
    }
    public void StopMoving()
    {
        player.myRigidbody.linearVelocity = Vector2.zero;
    }
    public void StopGravity()
    {
        player.SetGravityScale(0.0f);
    }
}
