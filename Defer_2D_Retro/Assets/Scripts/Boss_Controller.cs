using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Controller : MonoBehaviour
{
    [Header("Damage")]
    public float damage;

    [Header("Monster")]
    public Monster_Health MH;
    public Rigidbody2D rb;
    public PolygonCollider2D plc;

    [Header("Player")]
    public Player_Controller PC;

    [Header("Trace")]
    public float distanceToPlayer;
    public float moveSpeed = 10f;

    [Header("Bools")]
    public bool isSkill = false;
    public bool isAttack = false;

    [Header("Attack")]
    public GameObject attackCollider;

    [Header("Animation")]
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        plc = GetComponent<PolygonCollider2D>();
        PC = FindObjectOfType<Player_Controller>();
        MH = GetComponent<Monster_Health>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttack && !anim.GetCurrentAnimatorStateInfo(0).IsName("Monster_Attack"))
        {
            Trace();
        }
        Attack();
        AnimControl();
    }

    /// <summary>
    /// 각 애니메이션의 상태를 컨트롤 하는 함수
    /// </summary>
    public void AnimControl()
    {
        if (isAttack)
        {
            anim.SetBool("isAttack", true);
        }
        else
        {
            anim.SetBool("isAttack", false);
        }

        /*if (MH.isDead)
        {
            anim.SetBool("isDead", true);
            anim.SetBool("isMove", false);
            anim.SetBool("isAttack", false);
        }
        else
        {
            anim.SetBool("isDead", false);
        }*/
    }

    /// <summary>
    /// 거리를 설정하여 그 거리 안에 들 경우 플레이어를 추적하도록 하는 함수
    /// </summary>
    public void Trace()
    {
        // 자신과 플레이어 사이의 거리 계산(float)
        distanceToPlayer = Vector3.Distance(transform.position, PC.transform.position);

        // 자신과 플레이어 사이의 거리 계산(Vector2)
        Vector3 movement = -(transform.localPosition - PC.transform.position).normalized * moveSpeed * Time.deltaTime;

        // 바라보는 방향 설정(로컬 스케일 x 조정)
        SetDirection(movement, 2);

        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 방향을 설정하는 함수, scale은 스프라이트의 기본 크기에 맞게 설정
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="scale"></param>
    public void SetDirection(Vector2 dir, float scale)
    {
        // 바라보는 방향 설정(로컬 스케일 x 조정)
        if (dir.x < 0)
        {
            transform.localScale = new Vector2(-scale, transform.localScale.y);
        }
        else if (dir.x > 0)
        {
            transform.localScale = new Vector2(scale, transform.localScale.y);
        }
    }

    /// <summary>
    /// 플레이어와의 거리가 가까울 경우 공격을 활성화하는 함수.
    /// </summary>
    public void Attack()
    {
        distanceToPlayer = Vector3.Distance(transform.position, PC.transform.position);

        if (distanceToPlayer <= 2.25f)
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }
    }

    /// <summary>
    /// 애니메이션 속 이벤트에 적용되는 함수. 1타 공격에 사용
    /// </summary>
    public void OnAttackCollision()
    {
        // 공격 애니메이션에 이벤트로 넣어서 활성화
        attackCollider.SetActive(true);
    }

}
