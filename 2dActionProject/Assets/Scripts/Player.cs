using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed; // 최대 속도
    public float jumpPower; // 점프 파워
    public int damage = 10; // 공격 데미지
    public float coolTime = 0.7f; // 공격 쿨타임
    public int combo = 0; // 콤보 쌓기
    public int maxCombo = 1; // 맥스콤보
    public float comboResetTime = 2.0f; // 공격 없이 대기할 시간
    public float speed;
    public float defaultSpeed;
    float comboTimer = 0.0f; // 현재 대기 시간을 추적하는 타이머
    public float chargeTimer = 0.0f; //차징 시간(차징할 때마다 시간++)
    float maxChargeTime = 4.0f; //맥스 차징 시간 (4초까지 가면 차징 끝내고 공격)
    bool isCharing = false;//차징을 하고 있는지 검증
    bool isDash = false; // 대쉬를 하는지 검증
    public float dashSpeed;//대쉬 속도
    public float defaultTime;
    float dashTime;
    public Transform Meleepos;
    public Vector2 boxSize;

    float curTime;
    float hAxis;
    Rigidbody2D rigid;
    Animator anim;
    [HideInInspector] [SerializeField] new SpriteRenderer renderer;


    void Awake()
    {
        defaultSpeed = speed;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Move();
        Turn();
        Dash();
        Jump();
        Attack();
    }
    void Move()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        //움직임
        rigid.velocity = new Vector2(hAxis * defaultSpeed, rigid.velocity.y);

        //속도 멈추기
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //애니메이션
        if(Mathf.Abs(rigid.velocity.x) < 0.4)
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
        if(Input.GetButtonDown("Dash"))
        {
            isDash = true;
        }
        if(dashTime <= 0)
        {
            defaultSpeed = speed;
            if(isDash)
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
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            
            anim.SetBool("isJump", true);
        }
        
    }
    void Turn()
    {
        if(Input.GetButtonDown("Horizontal"))
        {
            //시점 반전
            renderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            bool isFlipX = renderer.flipX;
            //공격범위도 함께 반전
            Meleepos.localPosition = new Vector3((isFlipX ? -0.5f : 0.5f), Meleepos.localPosition.y, Meleepos.localPosition.z);

        }
    }

    void Attack()
    {
        //마우스 왼클릭을 누르면 공격
        if (Input.GetButtonDown("Fire1") && curTime <= 0)
        {
            //공격
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
            // 공격이 있을 때마다 comboTimer 초기화
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

        //차징 공격
        if(Input.GetButtonDown("Charge") && !isCharing)
        {
            anim.SetTrigger("doCharge");
            StartCoroutine(ChargeDamage());
            isCharing = true;
        } //chargeTimer가 maxChargeTime보다 커지면 공격 실행
        else if(chargeTimer >= maxChargeTime)
        {
            StartCoroutine(ChangeColor());
            StopCoroutine(ChargeDamage());
            anim.SetTrigger("doAttack");
            DoDamage();
            StopCoroutine(ChangeColor());
            damage = 10;
            chargeTimer = 0f;
            isCharing = false;
        } //차징을 떼면 차징 즉시 종료
        else if(Input.GetButtonUp("Charge"))
        {
            StopCoroutine(ChargeDamage());
            damage = 10;
            chargeTimer = 0f;
            isCharing = false;
        }
    }
    
    IEnumerator ChargeDamage()
    {
        if(Input.GetButtonDown("Charge") && !isCharing)
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
        else if(Input.GetButtonUp("Charge"))
        {
            yield return null;
        }
    }

    IEnumerator ChangeColor()
    {
        renderer.color = new Color(0, 0, 0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        renderer.color = new Color(1,1,1);
        
    }

    void DoDamage()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(Meleepos.position, boxSize, 0);
        foreach (Collider2D item in collider2Ds)
        {
            if (item.tag == "Enemy")
            {
                Enemy enemy = item.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.OnDamaged();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Meleepos.position, boxSize);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
        }
    }

}
