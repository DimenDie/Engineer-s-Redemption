using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Die()
    {
        anim.SetTrigger("death");
        StartCoroutine(DeathTimer());
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>())
        {
            collision.GetComponent<Character>().ReceiveDamage();
        }
    }
}
