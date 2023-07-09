using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    Player player;
    
    void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
    }

    void Update()
    {
        
    }

    public void OnDamaged()
    {
        health -= player.damage;
    }
}
