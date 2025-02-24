using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Player")]
    public float moveSpeed = 2.5f;

    [Header("Physics")]
    public float gravity = -9.81f;

    [Header("Components")]
    public LayerMask groundLayer;

    private Vector2 boxCastSize = new Vector2(0.5f, 0.05f);
    private float boxCastMaxDistance = 0.5f;

    private float moveInput;
    private Vector2 velocity;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out sprite);
        TryGetComponent(out animator);
    }

    void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        velocity.x = moveInput * moveSpeed;

        if (moveInput > 0)
        {
            sprite.flipX = false;

            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }
        else if(moveInput < 0)
        {
            sprite.flipX = true;

            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }
        else
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }

        if (!IsGrounded())
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground"));
        return (raycastHit.collider != null);
    }
}
