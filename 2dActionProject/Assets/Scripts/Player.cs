using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float maxSpeed; // 占쌍댐옙 占쌈듸옙
    public float jumpPower; // 占쏙옙占쏙옙 占식울옙
    public int damage = 10; // 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙
    public float coolTime = 0.7f; // 占쏙옙占쏙옙 占쏙옙타占쏙옙
    public int combo = 0; // 占쌨븝옙 占쌓깍옙
    public int maxCombo = 1; // 占싣쏙옙占쌨븝옙
    public float comboResetTime = 2.0f; // 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占?占시곤옙
    public float speed;
    public float defaultSpeed;
    float comboTimer = 0.0f; // 占쏙옙占쏙옙 占쏙옙占?占시곤옙占쏙옙 占쏙옙占쏙옙占싹댐옙 타占싱몌옙
    public float chargeTimer = 0.0f; //占쏙옙징 占시곤옙(占쏙옙징占쏙옙 占쏙옙占쏙옙占쏙옙 占시곤옙++)
    float maxChargeTime = 4.0f; //占싣쏙옙 占쏙옙징 占시곤옙 (4占십깍옙占쏙옙 占쏙옙占쏙옙 占쏙옙징 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙)
    bool isCharing = false;//占쏙옙징占쏙옙 占싹곤옙 占쌍댐옙占쏙옙 占쏙옙占쏙옙
    bool isDash = false; // 占쎈쉬占쏙옙 占싹댐옙占쏙옙 占쏙옙占쏙옙
    public float dashSpeed;//占쎈쉬 占쌈듸옙
    public float defaultTime;
    float dashTime;
    public float chargeCnt = 0;
    public Transform Meleepos;
    public Vector2 boxSize;
    public RectTransform playerRectTransform;

    float curTime;
    float hAxis;
    Rigidbody2D rigid;
    Animator anim;
    [HideInInspector][SerializeField] new SpriteRenderer renderer;

    public Image chargingBar;
    public float chargingSpeed = 10f;
    public float targetWidth = 1.5f;
    RectTransform chargingRectTransform;
    private float chargeSumTime = 0.0f;

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


    void Awake()
    {
        defaultSpeed = speed;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        playerRenderer = GetComponent<SpriteRenderer>();
        audio = GetComponent<AudioSource>();
        chargingRectTransform = chargingBar.GetComponent<RectTransform>();
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
            // 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 comboTimer 占십깍옙화
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

        float currentWidth = chargingRectTransform.rect.x;

        //占쏙옙징 占쏙옙占쏙옙
        if (Input.GetButtonDown("Charge") && !isCharing && chargeSumTime < chargingSpeed && chargeCnt == 0)
        {

            chargeCnt = 1;
            anim.SetTrigger("doCharge");
            StartCoroutine(ChargeDamage());
            StartCoroutine(ChargeBar());
            isCharing = true;
        } //chargeTimer占쏙옙 maxChargeTime占쏙옙占쏙옙 커占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙
        else if (chargeTimer >= maxChargeTime)
        {

            StartCoroutine(ChangeColor());
            StopCoroutine(ChargeDamage());
            StopCoroutine(ChargeBar());
            anim.SetTrigger("doAttack");
            DoDamage();
            StopCoroutine(ChangeColor());
            damage = 10;
            chargeTimer = 0f;
            currentWidth = 0f;
            chargingRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);
            chargeCnt = 0;
            isCharing = false;
        } //占쏙옙징占쏙옙 占쏙옙占쏙옙 占쏙옙징 占쏙옙占?占쏙옙占쏙옙
        else if (Input.GetButtonUp("Charge") && chargeCnt == 1)
        {

            StopCoroutine(ChargeDamage());
            StopCoroutine(ChargeBar());
            damage = 10;
            chargeTimer = 0f;
            currentWidth = 0f;
            chargingRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);
            chargeCnt = 0;
            isCharing = false;
        }
    }
    IEnumerator ChargeBar()
    {
        float chargeTime = 4f; // 珥?李⑥쭠 ?쒓컙 (4珥?
        float startTime = Time.time; // ?쒖옉 ?쒓컙


        while (Time.time - startTime < chargeTime)
        {
            float elapsedTime = Time.time - startTime; // 寃쎄낵 ?쒓컙
            float progress = elapsedTime / chargeTime; // 吏꾪뻾 ?곹깭 (0 ~ 1)

            float newWidth = Mathf.Lerp(0f, targetWidth, progress); // 李⑥쭠 諛붿쓽 ?덈줈???덈퉬 怨꾩궛
            chargingRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

            yield return null;
            if (startTime == chargeTime)
            {
                break;
            }
        }
    }

    IEnumerator ChargeDamage()
    {
        if (Input.GetButtonDown("Charge") && !isCharing)
        {
            yield return new WaitForSeconds(1.0f);
            chargeTimer += 1.0f;
            damage += 15;

            yield return new WaitForSeconds(1.0f);
            chargeTimer += 1.0f;
            damage += 15;

            yield return new WaitForSeconds(1.0f);
            chargeTimer += 1.0f;
            damage += 15;

            yield return new WaitForSeconds(1.0f);
            chargeTimer += 1.0f;
            damage += 15;
            }
    }



    IEnumerator ChangeColor()
    {
        renderer.color = new Color(0, 0, 0, 0.5f);
        yield return new WaitForSeconds(0.5f);
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
                //dead
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
            }
        }
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
            Debug.Log(collision.GetComponentInParent<Enemy>().damage);
            Debug.Log(collision.transform.position);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Portal")
        {
            Debug.Log(collision.gameObject.tag);
        }
    }
}
