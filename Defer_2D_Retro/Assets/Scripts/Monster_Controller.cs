using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Monster_Controller : MonoBehaviour
{
    [Header("Monster")]
    public Monster_Health MH;
    public Rigidbody2D rb;
    public BoxCollider2D bc;

    [Header("Player")]
    public Player_Controller PC;

    [Header("Trace")]
    public float distanceToPlayer;
    public float traceDistance;
    public float moveSpeed = 20f;

    [Header("Patrol")]
    public Vector3 startPosition;   // ���� ��ġ
    public float distanceToOrigin;
    public float patrolRange = 5f;

    [Header("Jump")]
    public Transform wallCheck;
    public float jumpForce = 1.5f;

    [Header("Bools")]
    public bool isTrace = false;
    public bool isPatrol = false;

    [Header("Layers")]
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        PC = FindObjectOfType<Player_Controller>();
        MH = GetComponent<Monster_Health>();

        // �ʱ� ��ġ ����
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Vector3.down * 20f);   // ������ �߷� ����
        Patrol();
        Trace(traceDistance);
        Jump();
    }

    /// <summary>
    /// �Ÿ��� �����Ͽ� �� �Ÿ� �ȿ� �� ��� �÷��̾ �����ϵ��� �ϴ� �Լ�
    /// </summary>
    /// <param name="distance"></param>
    public void Trace(float distance)
    {
        // �ڽŰ� �÷��̾� ������ �Ÿ� ���(float)
        distanceToPlayer = Vector3.Distance(transform.position, PC.transform.position);

        // �ڽŰ� �÷��̾� ������ �Ÿ� ���(Vector2)
        Vector3 movement = -(transform.position - PC.transform.position).normalized * moveSpeed * Time.deltaTime;
        movement.y = 0;

        isTrace = false;

        // �÷��̾���� �Ÿ��� ������ �Ÿ����� �۰ų� ���� ��� ���� ����
        if(distance >= distanceToPlayer)
        {
            isTrace = true;

            // �ٶ󺸴� ���� ����(���� ������ x ����)
            SetDirection(movement, 1);

            // �ǰ� ���°� �ƴҶ��� ����
            if (!MH.isHit)
            {
                // ReturnToBase�� Invoke ������ ���
                CancelInvoke(nameof(ReturnToBase));
                transform.position += movement * moveSpeed * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// �������� ���� �� ������ ����(��ȸ)�ϵ��� �ϴ� �Լ�
    /// </summary>
    public void Patrol()
    {
        // �ʱ� ��ġ�� ���� ��ġ ������ �Ÿ�(float)
        distanceToOrigin = Vector2.Distance(transform.position, startPosition);

        // �ʱ� ��ġ�� ���� ��ġ ������ �Ÿ�(Vector2)
        Vector2 movement = -(transform.position - startPosition).normalized * moveSpeed * Time.deltaTime;
        movement.y = 0;

        // �ٶ󺸴� ���� ����(���� ������ x ����)
        SetDirection(movement, 1);

        if (!isTrace)
        {
            if(distanceToOrigin > 1f)
            {
                // 5�ʵ� ReturnToBase ����
                Invoke(nameof(ReturnToBase), 5f);
            }
        }
    }

    /// <summary>
    /// �տ� ���� ������� �����ϰ� �ϴ� �Լ�
    /// </summary>
    public void Jump()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position, 0.5f, groundLayer);

        if (colliders.Length > 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }


    /// <summary>
    /// ���� ��ġ�� �����ϵ��� ����� �Լ�
    /// </summary>
    public void ReturnToBase()
    {
        Vector3 movement = -(transform.position - startPosition).normalized * moveSpeed * Time.deltaTime;
        movement.y = 0;

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
        if (dir.x > 0)
        {
            transform.localScale = new Vector2(scale, transform.localScale.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
