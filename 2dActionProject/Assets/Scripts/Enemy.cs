using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    Player player;

    public Transform pos;
    public int damage = 2;
    public BoxCollider2D box;
    public float coolTime;
    float currentTime;
    
    void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
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
                        //animation
                        Debug.Log("퍽");
                        enBox();
                    }
                    currentTime = coolTime;
                    deBox();
                }
            }
        }
        currentTime -= Time.deltaTime;
    }

    void enBox()
    {
        box.enabled = true;
    }

    void deBox()
    {
        box.enabled = false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos.position, new Vector3(1f, 1f, 1f));
    }

    public void OnDamaged()
    {
        health -= player.damage;
    }
}
