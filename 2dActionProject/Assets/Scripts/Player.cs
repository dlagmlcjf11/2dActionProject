using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float maxSpeed; // �ִ� �ӵ�
    public float jumpPower; // ���� �Ŀ�
    public int damage = 10; // ���� ������
    public float coolTime = 0.7f; // ���� ��Ÿ��
    public int combo = 0; // �޺� �ױ�
    public int maxCombo = 1; // �ƽ��޺�
    public float comboResetTime = 2.0f; // ���� ���� ����� �ð�
    public float speed;
    public float defaultSpeed;
    float comboTimer = 0.0f; // ���� ��� �ð��� �����ϴ� Ÿ�̸�
    public float chargeTimer = 0.0f; //��¡ �ð�(��¡�� ������ �ð�++)
    float maxChargeTime = 4.0f; //�ƽ� ��¡ �ð� (4�ʱ��� ���� ��¡ ������ ����)
    bool isCharing = false;//��¡�� �ϰ� �ִ��� ����
    bool isDash = false; // �뽬�� �ϴ��� ����
    public float dashSpeed;//�뽬 �ӵ�
    public float defaultTime;
    float dashTime;
    public Transform Meleepos;
    public Vector2 boxSize;

    float curTime;
    float hAxis;
    Rigidbody2D rigid;
    Animator anim;
    [HideInInspector] [SerializeField] new SpriteRenderer renderer;

    public Image chargingBar;
    public float chargingSpeed = 0.5f;
    public float targetWidth = 1.5f;
    RectTransform chargingRectTransform;
    private float chargeSumTime = 0.0f;


    void Awake()
    {
        defaultSpeed = speed;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        chargingRectTransform = chargingBar.GetComponent<RectTransform>();
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
        //������
        rigid.velocity = new Vector2(hAxis * defaultSpeed, rigid.velocity.y);

        //�ӵ� ���߱�
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //�ִϸ��̼�
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
            //���� ����
            renderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            bool isFlipX = renderer.flipX;
            //���ݹ����� �Բ� ����
            Meleepos.localPosition = new Vector3((isFlipX ? -0.5f : 0.5f), Meleepos.localPosition.y, Meleepos.localPosition.z);

        }
    }

    void Attack()
    {
        //���콺 ��Ŭ���� ������ ����
        if (Input.GetButtonDown("Fire1") && curTime <= 0)
        {
            //����
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
            // ������ ���� ������ comboTimer �ʱ�ȭ
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

        //��¡ ����
        if(Input.GetButtonDown("Charge") && !isCharing && chargeSumTime < chargingSpeed)
        {
            anim.SetTrigger("doCharge");
            StartCoroutine(ChargeDamage());
            StartCoroutine(ChargeBar());
            isCharing = true;
        } //chargeTimer�� maxChargeTime���� Ŀ���� ���� ����
        else if(chargeTimer >= maxChargeTime)
        {
            StartCoroutine(ChangeColor());
            StopCoroutine(ChargeDamage());
            StopCoroutine(ChargeBar());
            anim.SetTrigger("doAttack");
            DoDamage();
            StopCoroutine(ChangeColor());
            damage = 10;
            chargeTimer = 0f;
            float currentWidth = chargingRectTransform.rect.x;
            isCharing = false;
        } //��¡�� ���� ��¡ ��� ����
        else if(Input.GetButtonUp("Charge"))
        {
            StopCoroutine(ChargeDamage());
            StopCoroutine(ChargeBar());
            damage = 10;
            chargeTimer = 0f;
            isCharing = false;
        }
    }
    IEnumerator ChargeBar()
    {
        float chargeTime = 4f; // 총 차징 시간 (4초)
        float startTime = Time.time; // 시작 시간

        while (Time.time - startTime < chargeTime)
        {
            float elapsedTime = Time.time - startTime; // 경과 시간
            float progress = elapsedTime / chargeTime; // 진행 상태 (0 ~ 1)

            float newWidth = Mathf.Lerp(0f, targetWidth, progress); // 차징 바의 새로운 너비 계산
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

        else if (Input.GetButtonUp("Charge"))
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
