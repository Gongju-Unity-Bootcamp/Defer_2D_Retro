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
    public bool isHit = false;

    [Header("Attack")]
    public GameObject attackCollider;

    [Header("Skill")]
    public float summonCooldown = 20f; // ��ȯ ��ų ��ٿ� �ð�
    public float hollowCooldown = 15f; // �ҷο� ��ų ��ٿ� �ð�
    public float randomTime;
    private float nextSkillTime; // ���� ��ų ������ ���� �ð�

    [Header("Summons")]
    public int summonTime = 0;
    public GameObject summons;
    public Transform summonPosition;

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
        if (!MH.isDead)
        {
            // ���� ���� �ƴҶ�(���� �ִϸ��̼� ������� �ƴҶ�)
            // ������ �����Ǵ� ���� �ƴҶ�
            // ��ȯ ��ų�� ���� ���� �ƴҶ�
            if (!isAttack && 
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Monster_Attack") && 
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper_Appear") && 
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper_Summon"))
            {
                Trace();

                // �������� ��ų ���
                if (Time.time > nextSkillTime)
                {
                    UseRandomSkill();
                }
            }
            Attack();
            GetHit();
        }
        else if (MH.isDead)
        {
            // ���Ͱ� �׾��� ��� 1.5�ʵ� OnDead ����(��Ȱ��ȭ)
            Invoke(nameof(OnDead), 1.5f);
        }
        AnimControl();
    }

    /// <summary>
    /// ���Ͱ� �׾��� �� ��Ȱ��ȭ ��Ű�� �Լ�
    /// </summary>
    public void OnDead()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �� �ִϸ��̼��� ���¸� ��Ʈ�� �ϴ� �Լ�
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

        if (isSummon)
        {
            anim.SetTrigger("isSummon");
            isSummon = false;
        }

        if (isHollow)
        {
            anim.SetTrigger("isHollow");
            isHollow = false;
        }

        if (MH.isDead)
        {
            anim.SetBool("isDead", true);
            anim.SetBool("isAttack", false);
        }
        else
        {
            anim.SetBool("isDead", false);
        }
    }

    /// <summary>
    /// �Ÿ��� �����Ͽ� �� �Ÿ� �ȿ� �� ��� �÷��̾ �����ϵ��� �ϴ� �Լ�
    /// </summary>
    public void Trace()
    {
        // �ڽŰ� �÷��̾� ������ �Ÿ� ���(float)
        distanceToPlayer = Vector3.Distance(transform.position, PC.transform.position);

        // �ڽŰ� �÷��̾� ������ �Ÿ� ���(Vector2)
        Vector3 movement = -(transform.localPosition - PC.transform.position).normalized * moveSpeed * Time.deltaTime;

        // �ٶ󺸴� ���� ����(���� ������ x ����)
        SetDirection(movement, 2);

        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// ������ �����ϴ� �Լ�, scale�� ��������Ʈ�� �⺻ ũ�⿡ �°� ����
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="scale"></param>
    public void SetDirection(Vector2 dir, float scale)
    {
        // �ٶ󺸴� ���� ����(���� ������ x ����)
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
    /// �÷��̾���� �Ÿ��� ����� ��� ������ Ȱ��ȭ�ϴ� �Լ�.
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
    /// �ִϸ��̼� �� �̺�Ʈ�� ����Ǵ� �Լ�. 1Ÿ ���ݿ� ���
    /// </summary>
    public void OnAttackCollision()
    {
        // ���� �ִϸ��̼ǿ� �̺�Ʈ�� �־ Ȱ��ȭ
        attackCollider.SetActive(true);
    }

    /// <summary>
    /// �������� ��ų�� �����Ͽ� ����ϴ� �Լ�
    /// </summary>
    public void UseRandomSkill()
    {
        // 0���� 1 ������ ������ �� ����
        randomTime = Random.Range(0f, 1f);

        // 0.5���� ���� ��� ��ȯ ��ų ���
        if (randomTime < 0.5f)
        {
            SummonSkill();
        }
        else // 0.5 �̻��� ��� �ҷο� ��ų ���
        {
            HollowSkill();
        }

        // ���� ��ų������ �ð��� �����ϰ� ��ٿ��� ����
        nextSkillTime = Time.time + (randomTime < 0.5f ? summonCooldown : hollowCooldown);
    }

    /// <summary>
    /// ��ȯ ��ų. 1~5���� ������ ������ ��ȯ���� ��ȯ
    /// </summary>
    public void SummonSkill()
    {
        isSummon = true;

        summonTime = Random.Range(1, 5);

        for(int i = 0; i <= summonTime; i++)
        {
            Instantiate(summons, summonPosition.position, summonPosition.rotation);
        }

        Debug.Log("Summon");
    }

    /// <summary>
    /// �ҷο�(���) ��ų. ��ų ����߿� ���� + �浹 X
    /// </summary>
    public void HollowSkill()
    {
        isHollow = true;
    }

    public void EnableHollow()
    {
        plc.enabled = false;
    }

    public void DisableHollow()
    {
        plc.enabled = true;
    }


    /// <summary>
    /// ������ �¾��� �� ��� ����� �� �ֵ���
    /// </summary>
    public void GetHit()
    {
        if (isHit)
        {
            EnableHollow();
        }
        else
        {
            DisableHollow();
        }
    }

    /// <summary>
    /// ��Ʈ�ѷ� ���� �ʱ�ȭ�� �Լ�
    /// </summary>
    public void ResetController()
    {
        isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // isTrigger�� ������� �ʾƵ� ���� ����� �� �ֵ���
        // �÷��̾ �ƴ� ��쿡��
        if (!collision.collider.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
