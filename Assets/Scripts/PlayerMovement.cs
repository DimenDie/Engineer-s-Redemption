using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private MovementCollision movementCollision;

    [Header("Move Settings")]
    public float speed = 10f;
    public float grabMoveSpeed = 5f;
    public float slideSpeed = 3f;
    public float jumpForce = 5f;

    public int side = 1; //Animations shit, later

    public bool canMove;
    bool wallGrab;
    bool wallJumped;

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
        print(rb.velocity);
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

            if(movementCollision.onGround)
            {
                Jump(Vector2.up, false);
            }
            else
            {
                WallJump();
            }
        }

        if(movementCollision.onWall && !movementCollision.onGround && !Input.GetKey(KeyCode.LeftShift))
        {
            WallSlide();
        }

        if(x > 0) 
        {
            side = 1;
        }
        if(x < 0)
        {
            side = -1;
        }
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;


        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    private void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = movementCollision.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up * 1.5f + wallDir * 1.5f), true);

        wallJumped = true;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    private void Jump(Vector2 dir, bool wall)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }
    
    private void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }
}
