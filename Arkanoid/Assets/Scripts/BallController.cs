using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    public ArkanoidGameManager manager;
    public PaddleController paddle;

    private Rigidbody2D rb;
    private float speed = 7f;
    private bool attachedToPaddle = true;

    private Coroutine boostCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (attachedToPaddle)
        {
            FollowPaddle();

            bool launchPressed = Keyboard.current != null &&
                                (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);

            if (launchPressed)
            {
                Launch();
            }
        }
        else
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = Mathf.Max(4f, newSpeed);
    }

    public void BoostSpeed(float multiplier, float duration)
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
        }

        boostCoroutine = StartCoroutine(BoostRoutine(multiplier, duration));
    }

    public void AttachToPaddle()
    {
        attachedToPaddle = true;
        rb.linearVelocity = Vector2.zero;
        FollowPaddle();
    }

    private void FollowPaddle()
    {
        if (paddle == null)
        {
            return;
        }

        transform.position = paddle.transform.position + new Vector3(0f, 0.42f, 0f);
    }

    private void Launch()
    {
        attachedToPaddle = false;

        float randomX = Random.Range(-0.65f, 0.65f);
        Vector2 direction = new Vector2(randomX, 1f).normalized;
        rb.linearVelocity = direction * speed;

        manager?.OnBallLaunched();
    }

    private IEnumerator BoostRoutine(float multiplier, float duration)
    {
        float originalSpeed = speed;
        speed *= Mathf.Max(1f, multiplier);

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
        boostCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Brick")
        {
            var brick = collision.gameObject.GetComponent<Brick>();
            if (brick != null)
            {
                brick.Break();
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Paddle"))
        {
            float distanceFromCenter = transform.position.x - collision.transform.position.x;
            float normalizedDistance = Mathf.Clamp(distanceFromCenter / 1.3f, -1f, 1f);

            Vector2 newDirection = new Vector2(normalizedDistance, 1f).normalized;
            rb.linearVelocity = newDirection * speed;
        }
    }
}
