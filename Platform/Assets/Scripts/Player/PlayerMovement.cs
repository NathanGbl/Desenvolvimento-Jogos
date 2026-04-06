using UnityEngine;
using OCaminhoDoPeregrino.Core;

namespace OCaminhoDoPeregrino.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float airControlMultiplier = 0.8f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpBufferTime = 0.1f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.18f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 16f;
        [SerializeField] private float dashDuration = 0.18f;
        [SerializeField] private float dashCooldown = 0.7f;

        [Header("Penalty")]
        [SerializeField] private int maxHealth = 3;

        private Rigidbody2D rb;
        private float moveInput;
        private bool isGrounded;
        private float coyoteCounter;
        private float jumpBufferCounter;
        private bool isDashing;
        private float dashTimer;
        private float dashCooldownTimer;
        private Vector2 dashDirection;
        private Vector3 checkpointPosition;
        private int currentHealth;
        private float originalGravityScale;

        public int CurrentHealth => currentHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            originalGravityScale = rb.gravityScale;
            checkpointPosition = transform.position;
            currentHealth = maxHealth;
        }

        private void Update()
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0f)
            {
                StartDash();
            }
        }

        private void FixedUpdate()
        {
            if (groundCheck == null)
            {
                return;
            }

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (isGrounded)
            {
                coyoteCounter = coyoteTime;
            }
            else
            {
                coyoteCounter -= Time.fixedDeltaTime;
            }

            if (isDashing)
            {
                UpdateDash();
                return;
            }

            ApplyHorizontalMovement();
            TryJump();
        }

        private void ApplyHorizontalMovement()
        {
            float control = isGrounded ? 1f : airControlMultiplier;
            rb.linearVelocity = new Vector2(moveInput * moveSpeed * control, rb.linearVelocity.y);
        }

        private void TryJump()
        {
            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
            }
        }

        private void StartDash()
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            float horizontalDirection = Mathf.Abs(moveInput) > 0.01f ? Mathf.Sign(moveInput) : transform.localScale.x >= 0f ? 1f : -1f;
            dashDirection = new Vector2(horizontalDirection, 0f);
            rb.gravityScale = 0f;
            rb.linearVelocity = dashDirection * dashSpeed;
        }

        private void UpdateDash()
        {
            dashTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = dashDirection * dashSpeed;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = originalGravityScale;
            }
        }

        public void ApplySinPenalty(int damage, bool forceCheckpointRespawn)
        {
            if (damage > 0)
            {
                currentHealth -= damage;
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                if (GameFlowManager.Instance != null)
                {
                    GameFlowManager.Instance.LoadDefeat();
                }
                else
                {
                    RespawnAtCheckpoint();
                }

                return;
            }

            if (forceCheckpointRespawn)
            {
                RespawnAtCheckpoint();
            }
        }

        public void SetCheckpoint(Vector3 checkpoint)
        {
            checkpointPosition = checkpoint;
        }

        public void RespawnAtCheckpoint()
        {
            transform.position = checkpointPosition;
            rb.linearVelocity = Vector2.zero;
            currentHealth = maxHealth;
            isDashing = false;
            rb.gravityScale = originalGravityScale;
        }
    }
}
