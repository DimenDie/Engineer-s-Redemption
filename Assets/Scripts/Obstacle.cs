using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.name);

        if (collision.GetComponent<Character>())
        {
            collision.GetComponent<Character>().ReceiveDamage();
        }
    }
}
