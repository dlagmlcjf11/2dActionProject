using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    Player player;

    public Transform pos;
    public int damage = 2;
    public GameObject box;
    public GameObject enemy;
    public float coolTime;
    public float currentTime;
    Animator anim;

    void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Attack();
    }

    void Attack()
    {
        Collider2D[] collider = Physics2D.OverlapBoxAll(pos.position, new Vector2(1f, 1f), 1);

        if(collider != null)
        {
            for(int i=0; i<collider.Length; i++)
            {
                if(currentTime <= 0)
                {
                    if (collider[i].tag == "Player")
                    {
                        anim.SetTrigger("doAttack");
                        StartCoroutine(enBox());
                        currentTime = coolTime;
                        StartCoroutine(deBox());
                    }
                }
            }
        }
        currentTime -= Time.deltaTime;
    }

    IEnumerator enBox()
    {
        yield return new WaitForSeconds(0.3f);
        box.SetActive(true);
    }

    IEnumerator deBox()
    {
        yield return new WaitForSeconds(0.8f);
        box.SetActive(false);
    }

    public void OnDamaged()
    {
        anim.SetTrigger("doTakeHit");
        health -= player.damage;
        if(health <= 0) {
            anim.SetTrigger("doDeath");
            StartCoroutine(enemyDeath());
        }
    }

    IEnumerator enemyDeath()
    {
        yield return new WaitForSeconds(2f);
        enemy.SetActive(false);
    }

    
}
