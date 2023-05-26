using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputMaster controls;

    private MovementCollision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    public AnimationScript anim;

    private Character character;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
    public int side = 1;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;
    private bool hasDashed;
    private bool groundTouch;

    [Space]
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    private Vector2 attackPos;
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    [Space]
    [Header("Bullshit")]
    private float x;
    private float y;
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.Attack.performed += _ => Attack();
        controls.Player.Movement.performed += ctx => Walk(ctx.ReadValue<Vector2>());
        controls.Player.Movement.performed += ctx => AxisLoad(ctx.ReadValue<Vector2>());
        controls.Player.Movement.canceled += ctx => Walk(ctx.ReadValue<Vector2>());
        controls.Player.Movement.canceled += ctx => AxisLoad(ctx.ReadValue<Vector2>());

        controls.Player.Jump.performed += jmp => JumpCheck();

        controls.Player.Grab.performed += grb => WallGrabCheck();
        controls.Player.Grab.performed += grb => WallGrab();

        controls.Player.Dash.performed += dsh => DashCheck(); 

    }

    void Start()
    {
        anim = GetComponentInChildren<AnimationScript>();
        coll = GetComponent<MovementCollision>();
        rb = GetComponent<Rigidbody2D>();
        character = GetComponent<Character>();

        attackPos = attackPoint.localPosition;
    }

    void Update()
    {
        if (x != 0)
            FindObjectOfType<AudioManager>().Play("PlayerStep");
        else
            FindObjectOfType<AudioManager>().Stop("PlayerStep");

        Vector2 dir = new Vector2(x, y);

        anim.SetHorizontalMovement(x, y, rb.velocity.y);
        Walk(dir);

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<Jumping>().enabled = true;
        }
        WallGrabCheck();
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }

        else
        {
            rb.gravityScale = 3;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        attackPoint.localPosition = attackPos * side;

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (wallGrab || wallSlide || !canMove)
            return;

        if (x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);

            side = anim.sr.flipX ? -1 : 1;
        }


    }

    void WallGrab()
    {
        if (!coll.onWall || !canMove) return;

        if (side != coll.wallSide)
            anim.Flip(side * -1);
        wallGrab = true;
        wallSlide = false;
    }
    void DashCheck()
    {
        if (hasDashed) return;
            if (x != 0 || y != 0)
            {
                anim.SetTrigger("dash");
                Dash(x, y);
            }
    }
    void WallGrabCheck()
    {
        if (!coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }
    }
    void JumpCheck()
    {
        anim.SetTrigger("jump");
        FindObjectOfType<AudioManager>().Play("PlayerJump");
        if (coll.onGround)
            Jump(Vector2.up, false);
        if (coll.onWall && !coll.onGround)
            WallJump();
    }
    void AxisLoad(Vector2 xyAxis)
    {
        x = xyAxis.x;
        y = xyAxis.y;
    }
    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
    }

    private void Dash(float x, float y)
    {
        hasDashed = true;


        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    private void Attack()
    {
        if (Time.time < nextAttackTime) return;
        anim.SetTrigger("attack");
        FindObjectOfType<AudioManager>().Play("PlayerAttack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (var enemy in hitEnemies)
        {
            print(enemy.name);
            if(enemy.GetComponent<Enemy>())
                enemy.GetComponent<Enemy>().Die();
            if(enemy.GetComponentInParent<Boss>())
                enemy.GetComponentInParent<Boss>().ReceiveDamage();
        }
        nextAttackTime = Time.time + 1f / attackRate;
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());

        rb.gravityScale = 0;
        GetComponent<Jumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        rb.gravityScale = 3;
        GetComponent<Jumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }

    private void WallSlide()
    {

        if (!canMove)
            return;

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;
        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }
}