using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager.UI;
using UnityEditor;

public class Player : MonoBehaviour
{
    public float maxSpeed; 
    public float jumpPower;
    public int damage = 10;
    public float coolTime = 0.7f;
    public int combo = 0; 
    public int maxCombo = 1;
    public float comboResetTime = 2.0f; 
    public float speed;
    public float defaultSpeed;
    float comboTimer = 0.0f; 
    bool isDash = false; 
    public float dashSpeed;
    public float defaultTime;
    float dashTime;
    public Transform Meleepos;
    public Vector2 boxSize;
    public RectTransform playerRectTransform;
    public GameObject player;

    float curTime;
    float hAxis;
    Rigidbody2D rigid;
    Animator anim;
    [HideInInspector][SerializeField] new SpriteRenderer renderer;

    //TakeHit
    public int health = 2;
    public bool isHurt = false;
    SpriteRenderer playerRenderer;
    Color halfA = new Color(1,1,1,0.5f);
    Color fullA = new Color(1,1,1,1);
    public float knockbackSpeed;
    bool isKnockback = false;
    public AudioClip knockbackImpact;
    AudioSource audio;

    //Health Alarm
    public Text healthText;


    void Awake()
    {
        defaultSpeed = speed;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        playerRenderer = GetComponent<SpriteRenderer>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        Move();
        Turn();
        Jump();
        Attack();
        Dash();
    }
    void Move()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        //占쏙옙占쏙옙占쏙옙
        rigid.velocity = new Vector2(hAxis * defaultSpeed, rigid.velocity.y);

        //占쌈듸옙 占쏙옙占쌩깍옙
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //占쌍니몌옙占싱쇽옙
        if (Mathf.Abs(rigid.velocity.x) < 0.4)
        {
            anim.SetBool("isRun", false);
        }
        else
        {
            anim.SetBool("isRun", true);
        }
    }
    void Dash()
    {
        if (Input.GetButtonDown("Dash"))
        {
            isDash = true;
        }
        if (dashTime <= 0)
        {
            defaultSpeed = speed;
            if (isDash)
            {
                dashTime = defaultTime;
            }
        }
        else
        {
            dashTime -= Time.deltaTime;
            defaultSpeed = dashSpeed;
        }
        isDash = false;
    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            anim.SetBool("isJump", true);
        }

    }
    void Turn()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            //占쏙옙占쏙옙 占쏙옙占쏙옙
            renderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            bool isFlipX = renderer.flipX;
            //占쏙옙占쌥뱄옙占쏙옙占쏙옙 占쌉뀐옙 占쏙옙占쏙옙
            Meleepos.localPosition = new Vector3((isFlipX ? -0.5f : 0.5f), Meleepos.localPosition.y, Meleepos.localPosition.z);

        }
    }

    void Attack()
    {
        //占쏙옙占쎌스 占쏙옙클占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙
        if (Input.GetButtonDown("Fire1") && curTime <= 0)
        {
            //占쏙옙占쏙옙
            if (combo < maxCombo)
            {
                combo += 1;
                DoDamage();

                anim.SetTrigger("doAttack");
            }
            else
            {
                combo = 0;
                damage += 10;
                DoDamage();
                anim.SetTrigger("doCombo");
                curTime = coolTime;
            }
            comboTimer = comboResetTime;
            damage = 10;
        }
        else
        {
            curTime -= Time.deltaTime;
        }
        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0.0f)
        {
            combo = 0;
        } 
        if (Input.GetButtonDown("Special"))
        {

            StartCoroutine(ChangeColor());
            StartCoroutine(SpecialDamage());
            anim.SetTrigger("doSpecial");
            DoDamage();
        } 
    }

    IEnumerator SpecialDamage()
    {
        damage += 50;
        yield return new WaitForSeconds(1f);
        damage = 10;
    }



    IEnumerator ChangeColor()
    {
        renderer.color = new Color(0, 0, 0, 0.5f);
        yield return new WaitForSeconds(0.8f);
        renderer.color = new Color(1, 1, 1);

    }

    void DoDamage()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(Meleepos.position, boxSize, 0);
        foreach (Collider2D item in collider2Ds)
        {
            if (item.tag == "Enemy" || item.tag == "Dummy")
            {
                Enemy enemy = item.GetComponent<Enemy>();
                TrainingDummies dummy = item.GetComponent<TrainingDummies>();
                if (enemy != null)
                {
                    enemy.OnDamaged();
                }
                if(dummy != null)
                {
                    dummy.OnDamaged();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Meleepos.position, boxSize);
    }
    void TakeHit(int damage, Vector2 pos)
    {
        if (!isHurt)
        {
            isHurt = true;
            health -= damage;
            if(health <= 0)
            {
                anim.SetTrigger("doDeath");
                StartCoroutine(playerDeath());
            }
            else
            {
                anim.SetTrigger("doTakeHit");
                audio.clip = knockbackImpact;
                audio.Play();
                float x = transform.position.x - pos.x;
                if(x < 0)
                {
                    x = 1;
                }
                else
                {
                    x = -1;
                }
                StartCoroutine(Knockback(x));
                StartCoroutine(HurtRoutine());
                StartCoroutine(alphablink());
                StartCoroutine(healthTextAnim());
                
            }
        }
        IEnumerator healthTextAnim()
        {
            healthText.text = damage.ToString();
            yield return new WaitForSeconds(0.4f);
            healthText.text = "";
        }
    }

    IEnumerator playerDeath()
    {
        yield return new WaitForSeconds(2f);
        player.SetActive(false);
    }

    IEnumerator Knockback(float dir)
    {
        isKnockback = true;
        float ctime = 0;
        while(ctime < 0.2f)
        {
            if (transform.rotation.y == 0)
                transform.Translate(Vector2.left * knockbackSpeed * Time.deltaTime * dir);
            else
                transform.Translate(Vector2.left * knockbackSpeed * Time.deltaTime * -1f * dir);
            ctime += Time.deltaTime;
            yield return null;
        }
        isKnockback = false;
    }

    IEnumerator HurtRoutine()
    {
        yield return new WaitForSeconds(4f);
        isHurt = false;
    }

    IEnumerator alphablink()
    {
        while(isHurt)
        {
            yield return new WaitForSeconds(0.1f);
            playerRenderer.color = halfA;
            yield return new WaitForSeconds(0.1f);
            playerRenderer.color = fullA;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
        }
        if (collision.gameObject.tag == "FloorCheck")
        {
            playerRectTransform.localPosition = new Vector3(-14.1f, -0.35f, 0f);
            health -= 10;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAtk"))
        {
            TakeHit(collision.GetComponentInParent<Enemy>().damage, collision.transform.position);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BattlePortal")
        {
            if (Input.GetButtonDown("Interaction"))
            {
                SceneManager.LoadScene(1);
            }
        }
        if(collision.gameObject.tag == "ShopPortal")
        {
            if (Input.GetButtonDown("Interaction"))
            {
                SceneManager.LoadScene(2);
            }
        }
        if(collision.gameObject.tag == "BossPortal")
        {
            if (Input.GetButtonDown("Interaction"))
            {
                SceneManager.LoadScene(3);
            }
        }          
    }
}
