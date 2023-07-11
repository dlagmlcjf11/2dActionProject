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
    public float coolTime;
    public float currentTime;

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
                        box.SetActive(true);
                        currentTime = coolTime;
                        StartCoroutine(deBox());
                    }
                }
            }
        }
        currentTime -= Time.deltaTime;
    }

    IEnumerator deBox()
    {
        yield return new WaitForSeconds(3f);
        box.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos.position, new Vector3(1f, 1f, 1f));
    }

    public void OnDamaged()
    {
        Debug.Log("Damaged");
        health -= player.damage;
    }
}
