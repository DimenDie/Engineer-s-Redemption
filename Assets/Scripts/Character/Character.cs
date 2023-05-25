using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    [HideInInspector]
    private Rigidbody2D rigidbody;
    private PlayerMovement playerMovement;
    public LivesBar livesBar;
    public GameObject alivePortrait;


    public bool hasGrapple;
    public bool hasDash;
    public bool isDead = false;

    public int health = 12;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        if(Health <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (value < 12)
                health = value;
            livesBar.RefreshCharacter();
        }
    }

    public void ReceiveDamage()
    {
        if(isDead) { return; }
        --Health;
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.up * 10.0f, ForceMode2D.Impulse);
        playerMovement.anim.SetTrigger("hit");
        FindObjectOfType<AudioManager>().Play("PlayerDamage");


        Debug.Log(health);
    }

    public void Die()
    {
        Debug.Log("Dead :(");
        alivePortrait.SetActive(false);
        playerMovement.anim.SetTrigger("die");
        playerMovement.enabled = false;
        
        StartCoroutine(DeathTimer());
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
