using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject parent;
    public GameObject Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    private float speed = 10.0f;
    private Vector3 direction;
    public Vector3 Direction
    {
        set { direction = value; }
    }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Destroy(gameObject, 1.4f);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != parent)
        {
            print(collision.gameObject.name);
            if (collision.gameObject.GetComponent<Character>())
            {
                collision.gameObject.GetComponent<Character>().ReceiveDamage();
            }
            Destroy(gameObject);
        }
    }
}
