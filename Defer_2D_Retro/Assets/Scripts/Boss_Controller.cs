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
    public float moveSpeed = 30f;

    [Header("Bools")]
    public bool isAttack = false;
    public bool isSummon = false;
    public bool isHollow = false;

    [Header("Attack")]
    public GameObject attackCollider;

    [Header("Skill")]
    public float summonCooldown = 10f; // 소환 스킬 쿨다운 시간
    public float hollowCooldown = 15f; // 할로우 스킬 쿨다운 시간
    public float randomTime;

    private float nextSkillTime; // 다음 스킬 사용까지 남은 시간

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

        // 랜덤으로 스킬 사용
        if (Time.time > nextSkillTime)
        {
            UseRandomSkill();
        }
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

        if (distanceToPlayer <= 2.5f)
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

    /// <summary>
    /// 랜덤으로 소환 스킬 또는 할로우 스킬을 선택하여 사용하는 함수
    /// </summary>
    public void UseRandomSkill()
    {
        // 0부터 1 사이의 랜덤한 값 생성
        randomTime = Random.Range(0f, 1f);

        // 0.5보다 작은 경우 소환 스킬 사용
        if (randomTime < 0.5f)
        {
            SummonSkill();
        }
        else // 0.5 이상인 경우 할로우 스킬 사용
        {
            HollowSkill();
        }

        // 다음 스킬까지의 시간을 설정하고 쿨다운을 적용
        nextSkillTime = Time.time + (randomTime < 0.5f ? summonCooldown : hollowCooldown);
    }

    /// <summary>
    /// 소환 스킬
    /// </summary>
    public void SummonSkill()
    {
        // isSummon = true;

        Debug.Log("Summon");
    }

    /// <summary>
    /// 할로우(통과) 스킬
    /// </summary>
    public void HollowSkill()
    {
        // isHollow = true;

        Debug.Log("Hollow");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // isTrigger를 사용하지 않아도 벽을 통과할 수 있도록
        // 플레이어가 아닐 경우에만
        if (!collision.collider.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
