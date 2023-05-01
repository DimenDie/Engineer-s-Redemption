using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private MovementCollision movementCollision;

    [Header("Move Settings")]
    public float speed = 10f;
    public float slideSpeed = 3f;
    public float jumpForce = 5f;

    bool wallGrab;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementCollision = GetComponent<MovementCollision>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);

        wallGrab = movementCollision.onWall && Input.GetKey(KeyCode.LeftShift);

        if (wallGrab)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, y * speed);
        }
        else
        {
            rb.gravityScale = 1;
        }


        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if(movementCollision.onWall && !movementCollision.onGround)
        {
            WallSlide();
        }
    }

    private void Walk(Vector2 dir)
    {
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;
    }
    
    private void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }
}
