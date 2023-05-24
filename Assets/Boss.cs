using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Animator anim;
    private int health = 12;
    public LivesBar bossLivesBar;
    private bool isDead;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (value < 12)
                health = value;
            bossLivesBar.RefreshBoss();
        }
    }
    private void FixedUpdate()
    {
        if (Health <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }

        if(Input.GetKeyDown(KeyCode.X))
            ReceiveDamage();
    }

    public void Die()
    {
        anim.SetTrigger("death");
        StartCoroutine(DeathTimer());
    }

    public void ReceiveDamage()
    {
        if (isDead) { return; }
        --Health;
        anim.SetTrigger("hit");

        Debug.Log(health);
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        GameObject.Destroy(gameObject);
    }
}
