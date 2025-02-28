using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Player")]
    public float moveSpeed = 2.5f;
    public float jumpCharge = 0f;
    public float minJumpCharge = 2f;
    public float maxJumpCharge = 14f;

    [Header("Physics")]
    public float gravity = -9.81f;

    [Header("Components")]
    public LayerMask groundLayer;
    public LayerMask slopeLayer;

    RaycastHit2D groundHit;
    RaycastHit2D slopeHit;

    public Vector2 boxCastSize= new Vector2(0.7f, 0.75f);
    public float boxCastMaxDistance= 0.08f;

    private float moveInput;
    private Vector2 velocity;

    private bool isChargingJump = false;
    private float chargedDir = 0f;
    private bool bounce = false;
    public bool isGround;
    public bool isSlope;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out sprite);
        TryGetComponent(out animator);
    }

    private void Update()
    {
        Jump();

        isGround = IsGrounded();
        isSlope = IsSlope();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float deltaX = velocity.x * Time.fixedDeltaTime;
        float deltaY = velocity.y * Time.fixedDeltaTime;

        if (IsSlope())
        {
            Vector2 slopeNormal = slopeHit.normal.normalized;
            Vector2 slopeTangent = new Vector2(-slopeNormal.y, slopeNormal.x);

            if (Vector2.Dot(slopeTangent, Vector2.down) < 0)
            {
                slopeTangent = -slopeTangent;
            }

            float velocityAlongSlope = Vector2.Dot(velocity, slopeTangent);

            if (velocityAlongSlope < 0 && velocity.y < -1)
            {
                float reductionFactor = 0.8f;
                velocity += slopeTangent * (Mathf.Abs(velocityAlongSlope) * reductionFactor);
            }

            float slopeAngle = Mathf.Acos(slopeNormal.y);
            float effectiveAcceleration = Mathf.Abs(gravity) * Mathf.Sin(slopeAngle);

            velocity += slopeTangent * effectiveAcceleration * Time.fixedDeltaTime;
        }


        if (IsGrounded() && !IsSlope() && !isChargingJump)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            velocity.x = moveInput * moveSpeed;
        }

        if (moveInput > 0)
        {
            sprite.flipX = false;

            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }
        else if (moveInput < 0)
        {
            sprite.flipX = true;

            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }
        else
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }

        if (!IsGrounded() && !IsSlope())
        {
            velocity.y += gravity * Time.fixedDeltaTime;

            if(velocity.y < 0 && !bounce)
            {
                animator.Play("Jump_Down");
            }
        }
        else if (!IsSlope() && !isChargingJump && velocity.y < 0)
        {   
            if (IsGrounded() && velocity.y <= -16)
            {
                velocity.y = 0f;
                bounce = false;

                animator.Play("Splat");
            }
            else
            {
                velocity.y = 0f;
                bounce = false;

                animator.Play("Idle");
            }
        }  

        if (IsGrounded() && moveInput != 0)
        {
            Vector2 direction = new Vector2(Mathf.Sign(moveInput), 0);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, direction, Mathf.Abs(deltaX), groundLayer);

            if (hit.collider != null)
            {
                deltaX = (hit.distance - 0.05f) * Mathf.Sign(deltaX);
            }
        }

        if (!IsGrounded() && !IsSlope() && Mathf.Abs(velocity.y) > 0)
        {
            Vector2 direction = new Vector2(Mathf.Sign(velocity.x), 0);
            RaycastHit2D wallHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, direction, Mathf.Abs(deltaX), groundLayer);
            RaycastHit2D headHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.up, Mathf.Abs(deltaY), groundLayer);

            if (wallHit.collider != null)
            {
                bounce = true;

                Vector2 reflected = Vector2.Reflect(velocity, wallHit.normal);
                reflected = new Vector2(reflected.x * 0.5f, reflected.y);
                velocity = reflected;

                animator.Play("Bounce");

                deltaX = velocity.x * Time.fixedDeltaTime;
                deltaY = velocity.y * Time.fixedDeltaTime;
            }

            if (headHit.collider != null)
            {
                bounce = true;

                Vector2 reflected = Vector2.Reflect(velocity, headHit.normal);
                reflected = new Vector2(reflected.x * 0.6f, reflected.y * 0.1f);
                velocity = reflected;

                animator.Play("Jump_Down");

                deltaX = velocity.x * Time.fixedDeltaTime;
                deltaY = velocity.y * Time.fixedDeltaTime;
            }
        }

        Vector2 newPos = rb.position + new Vector2(deltaX, deltaY);

        if (isSlope)  // 또는 if (IsSlope()) 대신, 이미 업데이트한 isSlope 변수를 사용
        {
            RaycastHit2D snapHit = Physics2D.BoxCast(newPos, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, groundLayer);
            if (snapHit.collider != null)
            {
                float snapOffset = boxCastSize.y * 0.5f; // 필요에 따라 조절
                newPos.y = snapHit.point.y + snapOffset;
            }
        }

        rb.MovePosition(newPos);
    }

    private void Jump()
    {
        if (isChargingJump)
        {
            float inputDir = Input.GetAxisRaw("Horizontal");

            if (inputDir > 0)
            {
                sprite.flipX = false;

                chargedDir = 2.3f;
            }
            else if (inputDir < 0)
            {
                sprite.flipX = true;
                
                chargedDir = -2.3f;
            }
            else
            {
                chargedDir = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isChargingJump)
        {
            isChargingJump = true;
            jumpCharge = minJumpCharge;

            velocity.x = 0;
            rb.velocity = new Vector2(0, rb.velocity.y);

            animator.Play("Charge");
        }

        if (Input.GetKey("space") && IsGrounded())
        {
            jumpCharge += 20.5f * Time.deltaTime;
        }

        if (jumpCharge >= maxJumpCharge && IsGrounded())
        {
            animator.Play("Jump_Up");

            velocity = new Vector2(chargedDir * moveSpeed, maxJumpCharge);
            rb.MovePosition(rb.position + velocity * Time.deltaTime);

            Invoke("ResetJump", 0.1f);
        }

        if (Input.GetKeyUp(KeyCode.Space) && IsGrounded())
        {
            animator.Play("Jump_Up");

            velocity = new Vector2(chargedDir * moveSpeed, jumpCharge);
            rb.MovePosition(rb.position + velocity * Time.deltaTime);

            Invoke("ResetJump", 0.1f);         
        }
    }

    private void ResetJump()
    {
        isChargingJump = false;
        jumpCharge = 0f;
    }

    private bool IsGrounded()
    {
        groundHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, groundLayer);

        if (groundHit.collider != null)
        {
            Vector2 normal = groundHit.normal.normalized;

            if(normal.y >= 0.99f)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsSlope()
    {
        slopeHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, groundLayer);
        
        if (slopeHit.collider != null)
        {
            Vector2 normal = slopeHit.normal.normalized;

            if (normal.y < 0.99f)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 box = transform.position + Vector3.down * boxCastMaxDistance;

        Gizmos.DrawWireCube(box, boxCastSize);
    }
}
