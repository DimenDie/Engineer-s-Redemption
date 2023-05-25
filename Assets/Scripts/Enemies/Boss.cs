using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Animator anim;
    public Vector2 direction = Vector2.left;
    private int health = 12;
    public LivesBar bossLivesBar;
    private bool isDead;
    Projectile projectile;

    public float attackRate;
    private float nextAttackTime;

    private float animStart;

    private void Awake()
    {
        nextAttackTime = 0;
        anim = GetComponentInChildren<Animator>();
        projectile = Resources.Load<Projectile>("Projectile");
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

        if (Time.time >= nextAttackTime)
        {
            CheckAttack();
        }
    }

    private void Update()
    {
        if ((animStart + anim.GetCurrentAnimatorStateInfo(0).length <= Time.time) && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle")     //Animation is ended whatever it was
        {
            anim.SetTrigger("idle");
            animStart = Time.time;
        }

        if(Input.GetKeyDown(KeyCode.X)) 
        {
            ReceiveDamage();
        }
    }

    void CheckAttack()
    {
        RaycastHit2D leftAttack = Physics2D.Raycast(transform.position, Vector2.left, 7, gameObject.layer);
        RaycastHit2D rightAttack = Physics2D.Raycast(transform.position, Vector2.right, 7, gameObject.layer);


        int roll = Random.Range(1, 3);
        if (leftAttack && leftAttack.transform.GetComponent<Character>())
        {
            direction = Vector2.left;
            if (roll == 1)
                Attack();
            else if (roll == 2)
                Shoot();
        }
        if (rightAttack && rightAttack.transform.GetComponent<Character>())
        {
            direction = Vector2.right;
            if (roll == 1)
                Attack();
            else if (roll == 2)
                Shoot();
        }
    }
    void Shoot()
    {
        anim.SetTrigger("shoot");
        animStart = Time.time;
        Projectile newProjectile = Instantiate(projectile, transform.position, projectile.transform.rotation) as Projectile;

        newProjectile.Parent = gameObject;
        newProjectile.Direction = direction;

        nextAttackTime = Time.time + 1f / attackRate;
    }
    void Attack()
    {
        anim.SetTrigger("attack");
        animStart = Time.time;
        nextAttackTime = Time.time + 1f / attackRate;
    }


    public void Die()
    {
        FindObjectOfType<AudioManager>().Play("EnemyDeath");
        anim.SetTrigger("death");
        anim.SetBool("isDead", true);
        StartCoroutine(DeathTimer());
    }

    public void ReceiveDamage()
    {
        if (isDead) { return; }
        --Health;
        anim.SetTrigger("hit");
        animStart = Time.time;

        Debug.Log(health);
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 0.7f);
        GameObject.Destroy(gameObject);
    }
}
