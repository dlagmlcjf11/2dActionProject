using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummies : MonoBehaviour
{
    public int health = 99999;
    Player player;
    bool isHurt = false;
    Color halfA = new Color(1, 1, 1, 0.5f);
    Color fullA = new Color(1, 1, 1, 1);
    SpriteRenderer dummyRenderer;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        dummyRenderer = GetComponent<SpriteRenderer>();
    }
    public void OnDamaged()
    {
        health -= player.damage;
        isHurt = true;
        StartCoroutine(alphablink());
    }

    IEnumerator alphablink()
    {
        while (isHurt)
        {
            isHurt = false;
            yield return new WaitForSeconds(0.1f);
            dummyRenderer.color = halfA;
            yield return new WaitForSeconds(0.1f);
            dummyRenderer.color = fullA;
            
        }
    }
}
