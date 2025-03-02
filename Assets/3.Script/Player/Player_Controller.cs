using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float jumpCharge = 0f;
    [SerializeField] private float minJumpCharge = 2f;
    [SerializeField] private float maxJumpCharge = 14f;
    [SerializeField] private Vector2 boxCastSize = new Vector2(0.6f, 0.7f);
    [SerializeField] private float boxCastMaxDistance = 0.1f;

    [Header("Physics")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float currentSlideSpeed = 0f;
    [SerializeField] private float accelerationFactor = 2f;
    [SerializeField] private float gravitySlopeMultiplier = 1.2f;

    [Header("Components")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject windPlatform;

    RaycastHit2D groundHit;
    RaycastHit2D slopeHit;

    private float moveInput;
    private Vector2 velocity;

    private float chargedDir = 0f;
    private bool isChargingJump = false;
    private bool isBounce = false;
    public bool isWind = false;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;
    private Wind_Controller wind;


    private void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out sprite);
        TryGetComponent(out animator);
        TryGetComponent(out wind);
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        CheckWindPlatform();
        CheckWindOff();
        Move();
    }

    private void Move()
    {
        SlopeMove();
        NormalMove();
        FallingMove(); 

        float deltaX = velocity.x * Time.fixedDeltaTime;
        float deltaY = velocity.y * Time.fixedDeltaTime;
        Vector2 newPos = rb.position + new Vector2(deltaX, deltaY);

        if (IsGrounded() && !IsSlope() && moveInput != 0)
        {
            Vector2 direction = new Vector2(Mathf.Sign(moveInput), 0);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, direction, Mathf.Abs(deltaX), groundLayer);

            if (hit.collider != null)
            {
                deltaX = (hit.distance - 0.05f) * Mathf.Sign(deltaX);
            }
        }

        if (!IsGrounded() && Mathf.Abs(velocity.y) > 0)
        {
            Vector2 direction = new Vector2(Mathf.Sign(velocity.x), 0);
            RaycastHit2D wallHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, direction, Mathf.Abs(deltaX), groundLayer);
            RaycastHit2D headHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.up, Mathf.Abs(deltaY), groundLayer);

            if (wallHit.collider != null)
            {
                isBounce = true;

                Vector2 reflected = Vector2.Reflect(velocity, wallHit.normal);
                reflected = new Vector2(reflected.x * 0.5f, reflected.y);
                velocity = reflected;

                animator.Play("Bounce");

                deltaX = velocity.x * Time.fixedDeltaTime;
                deltaY = velocity.y * Time.fixedDeltaTime;
            }

            if (headHit.collider != null)
            {
                isBounce = true;

                Vector2 reflected = Vector2.Reflect(velocity, headHit.normal);
                reflected = new Vector2(reflected.x * 0.6f, reflected.y * 0.1f);
                velocity = reflected;

                animator.Play("Jump_Down");

                deltaX = velocity.x * Time.fixedDeltaTime;
                deltaY = velocity.y * Time.fixedDeltaTime;
            }
        }

        if (IsSlope())
        {
            RaycastHit2D snapHit = Physics2D.BoxCast(newPos, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, groundLayer);
            if (snapHit.collider != null)
            {
                float snapOffset = boxCastSize.y * 0.6f;
                newPos.y = snapHit.point.y + snapOffset;
            }
        }

        newPos = rb.position + new Vector2(deltaX, deltaY);

        // if (isWind)
        // {
        //     newPos += wind.WindForce;
        // }

        rb.MovePosition(newPos);
    }

    private void SlopeMove()
    {
        if (IsSlope())
        {
            animator.Play("Bounce");

            Vector2 slopeNormal = slopeHit.normal.normalized;
            Vector2 slopeTangent = new Vector2(-slopeNormal.y, slopeNormal.x);

            if (Vector2.Dot(slopeTangent, Vector2.down) < 0)
            {
                slopeTangent = -slopeTangent;
            }

            Vector2 gravityVec = new Vector2(0, gravity);
            float gravityProjection = Mathf.Abs(Vector2.Dot(gravityVec, slopeTangent));
            float targetSlideSpeed = gravityProjection * gravitySlopeMultiplier;

            currentSlideSpeed = Mathf.Lerp(currentSlideSpeed, targetSlideSpeed, accelerationFactor * Time.fixedDeltaTime);

            velocity = slopeTangent.normalized * currentSlideSpeed;
        }
        else
        {
            currentSlideSpeed = 0f;
        }
    }

    private void NormalMove()
    {
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
    }

    private void FallingMove()
    {
        if (!IsGrounded() && !IsSlope())
        {
            velocity.y += gravity * Time.fixedDeltaTime;

            if (velocity.y < 0 && !isBounce)
            {
                animator.Play("Jump_Down");
            }
        }
        else if (!IsSlope() && !isChargingJump && velocity.y < 0)
        {
            if (IsGrounded() && velocity.y <= -16)
            {
                velocity.y = 0f;
                isBounce = false;

                animator.Play("Splat");
            }
            else
            {
                velocity.y = 0f;
                isBounce = false;

                animator.Play("Idle");
            }
        }
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
            float slopeAngle = Vector2.Angle(slopeHit.normal, Vector2.up);

            if (slopeAngle > 10f && slopeAngle < 60f)
            {
                return true;
            }
        }

        return false;
    }

    private void CheckWindPlatform()
    {
        if (isWind)
        {
            return;
        }

        Vector2 footPos = (Vector2)transform.position - new Vector2(0, boxCastSize.y / 2 + boxCastMaxDistance);
        Collider2D[] hits = Physics2D.OverlapCircleAll(footPos, 0.1f);

        foreach (Collider2D col in hits)
        {
            if (col.gameObject == windPlatform)
            {
                isWind = true;
                break;
            }
        }
    }

    private void CheckWindOff()
    {
        if (isWind && windPlatform != null)
        {
            if (transform.position.y < windPlatform.transform.position.y)
            {
                isWind = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 box = transform.position + Vector3.down * boxCastMaxDistance;
        Gizmos.DrawWireCube(box, boxCastSize);

        Vector2 footPos = (Vector2)transform.position - new Vector2(0, boxCastSize.y / 2 + boxCastMaxDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(footPos, 0.1f);

        if (windPlatform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(windPlatform.transform.position, 0.2f);
        }
    }
}
