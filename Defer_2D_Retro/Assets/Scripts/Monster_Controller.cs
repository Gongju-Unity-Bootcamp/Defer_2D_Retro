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
    public LayerMask slopeLayer;

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
        SlopeCheck();
    }

    /// <summary>
    /// ���鿡 �ö� ���, �� ������ ���� ĳ������ ȸ������ ���ϵ��� �ϴ� �Լ�
    /// </summary>
    public void SlopeCheck()
    {
        Vector3 front = new Vector3(transform.position.x + 1f, transform.position.y, 0);
        Vector3 back = new Vector3(transform.position.x - 1f, transform.position.y, 0);

        RaycastHit2D frontHit = Physics2D.Raycast(front, new Vector2(0, -1), Mathf.Infinity, slopeLayer);
        RaycastHit2D backHit = Physics2D.Raycast(back, new Vector2(0, -1), Mathf.Infinity, slopeLayer);

        Debug.DrawRay(front, new Vector3(0, -0.5f, 0), Color.red);
        Debug.DrawRay(back, new Vector3(0, -0.5f, 0), Color.red);

        // Check if the ray hits the slopeLayer
        if (frontHit.collider != null && backHit.collider != null)
        {
            Vector3 frontPos = frontHit.point;
            Vector3 backPos = backHit.point;
            Vector3 lookDir = frontPos - backPos;
            float SlopeAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

            Quaternion charRotation = Quaternion.Euler(0, 0, SlopeAngle);
            this.transform.rotation = charRotation;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float slopeForce = moveSpeed;

        // ���鿡 ���� ���. ���� �������� �̵� �ӵ� ����
        if (collision.collider.CompareTag("Slope"))
        {
            moveSpeed = slopeForce * 2f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        float slopeForce = moveSpeed;

        if (collision.collider.CompareTag("Slope"))
        {

            if (moveSpeed > 20)
            {
                moveSpeed = slopeForce / 2f;
            }
        }
    }
}
