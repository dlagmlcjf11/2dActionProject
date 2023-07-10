using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummies : MonoBehaviour
{
    public int health = 99999;
    Player player;
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
    }
    public void OnDamaged()
    {
        health -= player.damage;
    }
}
