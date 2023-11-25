using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Controller : MonoBehaviour
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
    public float distanceToPlayer;
    public float traceDistance = 10f;
    public float moveSpeed = 30f;

    [Header("Patrol")]
    public Vector3 startPosition;   // ���� ��ġ
    public Vector3 randomPatrolPosition;
    public Vector3 prevPosition;
    public float distanceToOrigin;
    public float patrolRange = 8f;
    public float patrolTime = 2f; // ���� �ִ� �ð�
    public float currentPatrolTime = 0f;

    [Header("Jump")]
    public Transform wallCheck;
    public float jumpForce = 15f;
    public float jumpTime = 1f;

    [Header("Bools")]
    public bool isTrace = false;
    public bool isPatrol = false;
    public bool isPatrolStop = false;
    public bool isAttack = false;
    public bool isHit = false;
    public bool hasAddedScore = false;

    [Header("Animation")]
    public Animator anim;

    [Header("Attack")]
    public GameObject attackCollider;

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask slopeLayer;

    [Header("Item Drop")]
    public GameObject potion;
    public GameObject potionSpawned;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        PC = FindObjectOfType<Player_Controller>();
        MH = GetComponent<Monster_Health>();
        anim = GetComponent<Animator>();

        // �ʱ� ��ġ ����
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Vector3.down * 20f);   // ������ �߷� ����
        if (!MH.isDead)
        {
            Attack();

            // �������� �ƴҶ� + �ǰ� ���°� �ƴҶ� + ���� �ִϸ��̼� ������� �ƴҶ�
            if (!isAttack && !MH.isHit && !anim.GetCurrentAnimatorStateInfo(0).IsName("Monster_Attack") && !isHit)
            {
                // ���� ���� �ƴ� ���� ���� ����
                if (!isTrace)
                {
                    Patrol();
                }

                Trace(traceDistance);
            }

            Jump();
            SlopeCheck();
        }
        else if (MH.isDead && !hasAddedScore)
        {
            // ���Ͱ� �׾��� ��� 3�ʵ� ����
            Destroy(gameObject, 3f);
            GameManager.instance.AddScore(100);
            hasAddedScore = true;

            DropPotion();
            Destroy(potionSpawned, 10f);
        }

        AnimControl();
    }

    /// <summary>
    /// ������ Ȯ���� ������ ����ϰ� �ϴ� �Լ�
    /// </summary>
    public void DropPotion()
    {
        // Ȯ���� ������ ����
        float dropChance = 0.3f;

        // 0���� 1 ������ ������ �� ����
        float randomValue = Random.value;

        // ���� ���� Ȯ������ ������ potion ����
        if (randomValue < dropChance)
        {
            potionSpawned = Instantiate(potion, transform.position, Quaternion.identity);

            Vector2 force = new Vector2(0, 10f);

            potionSpawned.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// �� �ִϸ��̼��� ���¸� ��Ʈ�� �ϴ� �Լ�
    /// </summary>
    public void AnimControl()
    {
        if (isTrace || isPatrolStop)
        {
            anim.SetBool("isMove", true);
        }
        else
        {
            anim.SetBool("isMove", false);
        }

        if (isAttack)
        {
            anim.SetBool("isAttack", true);
        }
        else
        {
            anim.SetBool("isAttack", false);
        }

        if (MH.isDead)
        {
            anim.SetBool("isDead", true);
            anim.SetBool("isMove", false);
            anim.SetBool("isAttack", false);
        }
        else
        {
            anim.SetBool("isDead", false);
        }
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

        // ����ĳ��Ʈ�� ���鿡 �¾Ҵ��� üũ
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
        if (distance >= distanceToPlayer)
        {
            isTrace = true;

            // �ٶ󺸴� ���� ����(���� ������ x ����)
            SetDirection(movement, 1);

            transform.position += movement * moveSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// �������� ���� �� ������ ����(��ȸ)�ϵ��� �ϴ� �Լ�
    /// </summary>
    public void Patrol()
    {
        // �ڽŰ� ���� ��ġ ������ �Ÿ� ���(float)
        distanceToOrigin = Vector2.Distance(transform.position, startPosition);

        if(Mathf.Abs(distanceToOrigin) > patrolRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, (moveSpeed / 10) * Time.deltaTime);
            SetDirection(startPosition, 1);
        }

        if (!isPatrol)
        {
            // ������ ��ġ ����
            randomPatrolPosition = new Vector3(Random.Range(-patrolRange, patrolRange), 0f, 0f);

            // ���� ��ġ ����
            prevPosition = transform.position;

            isPatrol = true;
        }

        // �ٶ󺸴� ���� ����(���� ������ x ����)
        SetPatrolDirection(prevPosition + randomPatrolPosition, prevPosition, 1);

        // ��ǥ ��ġ���� �̵�
        transform.position = Vector3.MoveTowards(transform.position, startPosition + randomPatrolPosition, (moveSpeed / 10) * Time.deltaTime);

        // ��üũ. �̵��� �տ� ���� ������� ��ġ�� �ٽ� ����
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position, 0.5f, groundLayer);

        if (colliders.Length > 0)
        {
            randomPatrolPosition = new Vector3(Random.Range(-patrolRange, patrolRange), 0f, 0f);
        }

        // ��ǥ ��ġ�� �������� ��
        if (Vector3.Distance(transform.position, startPosition + randomPatrolPosition) < 0.1f)
        {
            currentPatrolTime += Time.deltaTime;

            // ���� �ð� ������ ����
            if (currentPatrolTime >= patrolTime)
            {
                currentPatrolTime = 0f;
                isPatrol = false;
            }
        }

        if(currentPatrolTime != 0)
        {
            isPatrolStop = false;
        }
        else
        {
            isPatrolStop = true;
        }
    }

    /// <summary>
    /// �տ� ���� ������� �����ϰ� �ϴ� �Լ�
    /// </summary>
    public void Jump()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position, 0.5f, groundLayer);

        if (colliders.Length > 0 && jumpTime != 0f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            jumpTime--;
        }
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
    /// ������ �����ϴ� �Լ�, scale�� ��������Ʈ�� �⺻ ũ�⿡ �°� ����
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="scale"></param>
    public void SetPatrolDirection(Vector2 dir, Vector2 prev, float scale)
    {
        // �ٶ󺸴� ���� ����(���� ������ x ����)
        if (dir.x < prev.x)
        {
            transform.localScale = new Vector2(-scale, transform.localScale.y);
        }
        else if (dir.x > prev.x)
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

        if (distanceToPlayer <= 1.75f)
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
    /// ��Ʈ�ѷ� ���� �ʱ�ȭ�� �Լ�
    /// </summary>
    public void ResetController()
    {
        isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float slopeForce = moveSpeed;

        // ���� ���� ���.
        if (collision.collider.CompareTag("Ground"))
        {
            jumpTime = 1f;
        }

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
