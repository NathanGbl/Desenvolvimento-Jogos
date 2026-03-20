using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 13f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float dashDuration = 0.16f;
    [SerializeField] private float dashCooldown = 0.7f;

    [Header("Detecção de Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool jumpPressed;
    private bool isGrounded;
    private bool canDoubleJump;

    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownTimer;
    private float dashDirection;

    public bool IsFacingRight { get; private set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash();
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        UpdateFacing();
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        if (isDashing)
        {
            UpdateDash();
            return;
        }

        Move();
        HandleJump();
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (!jumpPressed)
        {
            return;
        }

        if (isGrounded)
        {
            Jump();
            canDoubleJump = true;
        }
        else
        {
            PlayerProgress progress = PlayerProgress.Instance;
            bool hasDoubleJump = progress != null && progress.HasAbility(AbilityType.DoubleJump);
            if (hasDoubleJump && canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
            }
        }

        jumpPressed = false;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = Mathf.Abs(horizontalInput) > 0.01f ? Mathf.Sign(horizontalInput) : (IsFacingRight ? 1f : -1f);
    }

    private void UpdateDash()
    {
        dashTimeLeft -= Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        if (dashTimeLeft <= 0f)
        {
            isDashing = false;
        }
    }

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            return;
        }

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            canDoubleJump = true;
        }
    }

    private void UpdateFacing()
    {
        if (horizontalInput > 0.01f && !IsFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < -0.01f && IsFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    public Vector2 DashDirection()
    {
        return new Vector2(dashDirection, 0f);
    }
}
