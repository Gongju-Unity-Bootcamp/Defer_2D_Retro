using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summons_Controller : MonoBehaviour
{
    [Header("Damage")]
    public float damage;

    [Header("Monster")]
    public Monster_Health MH;
    public Rigidbody2D rb;
    public BoxCollider2D bc;

    [Header("Player")]
    public Player_Controller PC;

    [Header("Trace")]
    public float moveSpeed = 20f;

    [Header("Animation")]
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        PC = FindObjectOfType<Player_Controller>();
        MH = GetComponent<Monster_Health>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!MH.isDead && !anim.GetCurrentAnimatorStateInfo(0).IsName("Summons_Summon"))
        {
            // �������� �ƴҶ� + �ǰ� ���°� �ƴҶ� + ���� �ִϸ��̼� ������� �ƴҶ�
            if (!MH.isHit)
            {
                Trace();
            }
        }
        else if (MH.isDead)
        {
            Destroy(this.gameObject, 0.85f);
        }

        AnimControl();
    }

    /// <summary>
    /// �� �ִϸ��̼��� ���¸� ��Ʈ�� �ϴ� �Լ�
    /// </summary>
    public void AnimControl()
    {
        if (MH.isDead)
        {
            anim.SetBool("isDead", true);
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
        // �ڽŰ� �÷��̾� ������ �Ÿ� ���(Vector2)
        Vector3 movement = -(transform.position - PC.transform.position).normalized * moveSpeed * Time.deltaTime;

        // �ٶ󺸴� ���� ����(���� ������ x ����)
        SetDirection(movement, 1);

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            this.GetComponent<Monster_Health>().currentHealth = 0;
        }
        // isTrigger�� ������� �ʾƵ� ���� ����� �� �ֵ���
        else if (!collision.collider.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}