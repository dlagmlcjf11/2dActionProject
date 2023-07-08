using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    float hAxis;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer renderer;

    bool isJump;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        InputButton();
        Move();
        Turn();
        Jump();
    }
    void InputButton()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
    }
    void Move()
    {
        //움직임
        rigid.AddForce(Vector2.right * hAxis, ForceMode2D.Impulse);
        //속도 리미트값 지정
        if (rigid.velocity.x > maxSpeed) // 오른쪽 최대 속력
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < maxSpeed*(-1)) // 왼쪽 최대 속력
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }

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
    void Jump()
    {
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            
            anim.SetBool("isJump", true);
        }

        //바닥 체크
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Floor"));
        if(rayHit.collider != null)
        {
            if (rayHit.distance < 0.5f)
            {
                Debug.Log(rayHit.collider.name);
                anim.SetBool("isJump", false);
            }
        }
    }
    void Turn()
    {
        if(Input.GetButtonDown("Horizontal"))
        {
            renderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
    }

}
