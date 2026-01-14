using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Refrences")]
    [SerializeField] private EnemyController enemy;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveDirection; // Biến cũ của bạn

    [Header("Check Ground")]
    [SerializeField] private LayerMask platformerLayer;
    [SerializeField] private Transform groundCheckPos;

    [Header("Settings")]
    public bool isCrawler = false; // --- MỚI: Tích vào đây nếu là con sâu ---
    private float rotationSpeed = 15f; // Tốc độ xoay của sâu

    // Các biến cũ (Giữ nguyên giá trị mặc định của bạn)
    private float groundCheckDistance = 1.0f;
    private float wallCheckDistance = 0.5f;

    // --- GIỮ NGUYÊN BIẾN NÀY ĐỂ TRÁNH LỖI (Dù sai chính tả) ---
    public bool canMoveContinous;

    public bool canAirMoving = false;

    // --- GIỮ NGUYÊN PROPERTY NÀY VÀ CẬP NHẬT LOGIC ---
    public bool canMoveContinuous
    {
        get
        {
            // Logic cũ của bạn, nhưng hàm CheckGround/CheckWall bên dưới
            // đã được nâng cấp để hiểu cả con sâu và người thường
            bool hasGroundAhead = CheckGroundAhead();
            bool hasWallAhead = CheckWallAhead();

            if (canAirMoving)
                return !hasWallAhead;
            else
                return hasGroundAhead && !hasWallAhead;
        }
    }

    private void Awake()
    {
        // Giữ nguyên logic cũ
        if (this.enemy == null) this.enemy = GetComponent<EnemyController>();
        if (this.rb == null) this.rb = enemy.myRigidbody;

        // --- Setup thêm cho Sâu ---
        if (isCrawler)
        {
            rb.gravityScale = 0; // Sâu không chịu trọng lực để bám tường
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    // --- SỬA HÀM MOVE ĐỂ HỖ TRỢ CẢ 2 ---
    public void Move(Vector2 direction, float speed)
    {
        moveDirection = direction.normalized;

        // Fix lỗi Animator (nếu animator có parameter isMoving)
        if (enemy.animator != null)
        {
            // Dùng try-catch hoặc check trước để tránh lỗi nếu quên thêm param
            // enemy.animator.SetBool("isMoving", moveDirection.magnitude > 0);
        }

        if (moveDirection != Vector2.zero)
        {
            if (isCrawler)
            {
                // Logic đi của Sâu (Mới)
                HandleCrawlerMovement(speed, moveDirection.x);
            }
            else
            {
                // Logic đi bộ cũ (Giữ nguyên)
                enemy.CheckFacingDirection(moveDirection);
                rb.linearVelocity = new Vector2(moveDirection.x * speed, rb.linearVelocity.y);
            }
        }
        else
        {
            StopMove();
        }
    }

    // --- HÀM RIÊNG CHO SÂU (MỚI) ---
    private void HandleCrawlerMovement(float speed, float dirX)
    {
        // 1. Xác định hướng
        float facingDir = dirX > 0 ? 1 : -1;
        enemy.CheckFacingDirection(new Vector2(facingDir, 0));

        // 2. Bắn raycast từ bụng xuống (-transform.up) thay vì Vector2.down
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, -transform.up, groundCheckDistance, platformerLayer);

        if (hit.collider != null)
        {
            // Bám tường: Xoay theo pháp tuyến bề mặt
            transform.up = Vector3.Slerp(transform.up, hit.normal, rotationSpeed * Time.deltaTime);

            // Di chuyển: Đi theo hướng bên phải của chính nó (transform.right)
            rb.linearVelocity = transform.right * speed * enemy.facingDirection.x;
        }
        else
        {
            // Mất dấu đất: Hơi chúi đầu xuống hoặc rơi nhẹ để tìm đường
            transform.Rotate(0, 0, -5f * enemy.facingDirection.x);
            rb.linearVelocity = -transform.up * 2f;
        }
    }

    public void RunToTarget(Vector2 target, float speed)
    {
        Vector2 direction = new Vector2(target.x - enemy.transform.position.x, 0).normalized;
        Move(direction, speed);
        // Lưu ý: Nếu Animator không có isMoving, dòng dưới sẽ báo Warning vàng
        if (enemy.animator != null) enemy.animator.SetBool("isMoving", true);
    }

    public void StopMove()
    {
        rb.linearVelocity = isCrawler ? Vector2.zero : new Vector2(0, rb.linearVelocity.y);
        if (enemy.animator != null) enemy.animator.SetBool("isMoving", false);
    }

    // --- NÂNG CẤP HÀM CHECK (KHÔNG ĐỔI TÊN HÀM) ---
    public bool CheckGroundAhead()
    {
        // Nếu là Sâu: Check hướng chân (-transform.up)
        // Nếu là Người: Check hướng xuống (Vector2.down) - Logic cũ
        Vector2 direction = isCrawler ? -transform.up : Vector2.down;

        // Tăng khoảng cách check một chút nếu là sâu để bám góc tốt hơn
        float dist = isCrawler ? groundCheckDistance * 1.5f : groundCheckDistance;

        RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, direction, dist, platformerLayer);
        return hit.collider != null;
    }

    public bool CheckWallAhead()
    {
        // Nếu là Sâu: Check hướng mặt (transform.right)
        // Nếu là Người: Check hướng ngang (Vector2.right * facing) - Logic cũ tương đương transform.right nếu không xoay Z

        Vector2 dir = isCrawler ? transform.right : Vector2.right;

        // Lấy hướng nhìn từ EnemyController
        float facing = (enemy != null) ? enemy.facingDirection.x : 1;

        // Nếu là sâu thì dùng transform.right đã bao gồm hướng xoay, chỉ cần nhân facing local
        Vector2 finalDir = isCrawler ? (Vector2)transform.right * facing : Vector2.right * facing;

        RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, finalDir, wallCheckDistance, platformerLayer);
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (groundCheckPos == null) return;

        Gizmos.color = Color.green;
        Vector3 downDir = isCrawler ? -transform.up : Vector3.down;
        float gDist = isCrawler ? groundCheckDistance * 1.5f : groundCheckDistance;
        Gizmos.DrawLine(groundCheckPos.position, groundCheckPos.position + downDir * gDist);

        Gizmos.color = Color.blue;
        float facing = (enemy != null) ? enemy.facingDirection.x : 1;
        Vector3 rightDir = isCrawler ? (Vector3)(transform.right * facing) : (Vector3)(Vector2.right * facing);
        Gizmos.DrawLine(groundCheckPos.position, groundCheckPos.position + rightDir * wallCheckDistance);
    }
}