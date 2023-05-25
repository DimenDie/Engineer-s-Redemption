using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingEnemy : Enemy
{
    [Space]
    [Header("Movement")]
    [SerializeField] protected Vector3 direction;
    public float speed = 2.0f;
    protected float startSpeed;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        direction = Vector2.right;
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        print(gameObject.name + " moving");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.5F + transform.right * direction.x * 1.5F, 0.1F);

        if (colliders.Length > 0 && colliders.All(x => !x.GetComponent<Character>()))
        {
            foreach (var item in colliders)
            {
                print(item.name);
            }
            spriteRenderer.flipX = !spriteRenderer.flipX;
            direction *= -1.0F;
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }
}
