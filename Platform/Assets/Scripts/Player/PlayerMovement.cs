using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 13f;
    [SerializeField] private float airControlMultiplier = 0.85f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float dashDuration = 0.16f;
    [SerializeField] private float dashCooldown = 0.6f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Camera")]
    [SerializeField] private bool autoFollowMainCamera = true;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0.8f, -10f);

    [Header("Fall Boundary")]
    [SerializeField] private float deathYThreshold = -10f;

    private Rigidbody2D rb;
    private Camera mainCam;
    private Collider2D[] selfColliders;

    private float horizontal;
    private bool jumpBuffered;
    private bool grounded;
    private int airJumpCount;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private float dashDirection;

    public bool IsFacingRight { get; private set; } = true;
    public bool IsDashing => isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.freezeRotation = true;

        selfColliders = GetComponentsInChildren<Collider2D>();
        mainCam = Camera.main;
    }

    private void Start()
    {
        if (autoFollowMainCamera)
        {
            SnapCameraToPlayer();
        }
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpBuffered = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash();
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckFallBoundary();

        if (isDashing)
        {
            UpdateDash();
            return;
        }

        HandleMove();
        HandleJump();
    }

    private void LateUpdate()
    {
        if (!autoFollowMainCamera)
        {
            return;
        }

        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (mainCam != null)
        {
            Vector3 targetPos = transform.position + cameraOffset;
            targetPos.z = cameraOffset.z;
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPos, 10f * Time.deltaTime);
        }
    }

    private void SnapCameraToPlayer()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (mainCam != null)
        {
            Vector3 targetPos = transform.position + cameraOffset;
            targetPos.z = cameraOffset.z;
            mainCam.transform.position = targetPos;
        }
    }

    private void HandleMove()
    {
        float speedMultiplier = grounded ? 1f : airControlMultiplier;
        rb.linearVelocity = new Vector2(horizontal * moveSpeed * speedMultiplier, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (!jumpBuffered)
        {
            return;
        }

        if (grounded)
        {
            Jump();
            airJumpCount = 0;
        }
        else if (airJumpCount < 1)
        {
            Jump();
            airJumpCount++;
        }

        jumpBuffered = false;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = Mathf.Abs(horizontal) > 0.01f ? Mathf.Sign(horizontal) : (IsFacingRight ? 1f : -1f);
    }

    private void UpdateDash()
    {
        dashTimer -= Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            grounded = false;
            return;
        }

        bool wasGrounded = grounded;
        grounded = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundMask);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null)
            {
                continue;
            }

            bool isSelf = false;
            for (int j = 0; j < selfColliders.Length; j++)
            {
                if (hit == selfColliders[j])
                {
                    isSelf = true;
                    break;
                }
            }

            if (!isSelf)
            {
                grounded = true;
                break;
            }
        }

        if (!wasGrounded && grounded)
        {
            airJumpCount = 0;
        }
    }

    private void CheckFallBoundary()
    {
        if (transform.position.y < deathYThreshold)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private void UpdateFacingDirection()
    {
        if (horizontal > 0.01f && !IsFacingRight)
        {
            Flip();
        }
        else if (horizontal < -0.01f && IsFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    public float FacingSign()
    {
        return IsFacingRight ? 1f : -1f;
    }
}
