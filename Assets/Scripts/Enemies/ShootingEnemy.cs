using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ShootingEnemy : MovingEnemy
{
    [Space]
    [Header("Attack")]
    public float attackRate;
    private float nextAttackTime;
    Projectile projectile;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        projectile = Resources.Load<Projectile>("Projectile");
        startSpeed = speed;
        nextAttackTime = 0;
        direction = Vector2.right;
        anim.SetTrigger("move");
    }

    void FixedUpdate()
    {
        if (Time.time >= nextAttackTime)
        {
            CheckAttack();
        }

        Move();
    }
    void CheckAttack()
    {
        RaycastHit2D leftAttack = Physics2D.Raycast(transform.position, Vector2.left, 7, gameObject.layer);
        RaycastHit2D rightAttack = Physics2D.Raycast(transform.position, Vector2.right, 7, gameObject.layer);

        if (leftAttack && leftAttack.transform.GetComponent<Character>())
        {
            direction = Vector2.left;
            Attack();
        }
        if (rightAttack && rightAttack.transform.GetComponent<Character>())
        {
            direction = Vector2.right;
            Attack();
        }
    }

    IEnumerator Chill()
    {
        print("bin chilin");
        anim.SetTrigger("chill");
        speed = 0.0f;
        yield return new WaitForSeconds(5);
        anim.SetTrigger("move");
        speed = startSpeed;
        yield return null;
    }
    void Attack()
    {
        speed = 0.0f;
        anim.SetTrigger("attack");
        Projectile newProjectile = Instantiate(projectile, transform.position, projectile.transform.rotation) as Projectile;

        newProjectile.Parent = gameObject;
        newProjectile.Direction = direction;

        nextAttackTime = Time.time + 1f / attackRate;
    }
    protected override void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.5F + transform.right * direction.x * 1.5F, 0.1F);

        if (colliders.Length > 0 && colliders.All(x => !x.GetComponent<Character>()))
        {
            foreach (var item in colliders)
            {
                print(item.name);
            }
            StartCoroutine(Chill()); //Stops instead of moving into enemy
            spriteRenderer.flipX = !spriteRenderer.flipX;
            direction *= -1.0F;
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }
}
