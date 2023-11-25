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
    public Vector3 startPosition;   // 시작 위치
    public Vector3 randomPatrolPosition;
    public Vector3 prevPosition;
    public float distanceToOrigin;
    public float patrolRange = 8f;
    public float patrolTime = 2f; // 멈춰 있는 시간
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

        // 초기 위치 저장
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Vector3.down * 20f);   // 간단한 중력 구현
        if (!MH.isDead)
        {
            Attack();

            // 공격중이 아닐때 + 피격 상태가 아닐때 + 공격 애니메이션 재생중이 아닐때
            if (!isAttack && !MH.isHit && !anim.GetCurrentAnimatorStateInfo(0).IsName("Monster_Attack") && !isHit)
            {
                // 추적 중이 아닐 때만 순찰 실행
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
            // 몬스터가 죽었을 경우 3초뒤 제거
            Destroy(gameObject, 3f);
            GameManager.instance.AddScore(100);
            hasAddedScore = true;

            DropPotion();
            Destroy(potionSpawned, 10f);
        }

        AnimControl();
    }

    /// <summary>
    /// 랜덤한 확률로 포션을 드롭하게 하는 함수
    /// </summary>
    public void DropPotion()
    {
        // 확률을 설정할 변수
        float dropChance = 0.3f;

        // 0에서 1 사이의 랜덤한 값 생성
        float randomValue = Random.value;

        // 랜덤 값이 확률보다 작으면 potion 생성
        if (randomValue < dropChance)
        {
            potionSpawned = Instantiate(potion, transform.position, Quaternion.identity);

            Vector2 force = new Vector2(0, 10f);

            potionSpawned.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// 각 애니메이션의 상태를 컨트롤 하는 함수
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
    /// 경사면에 올라설 경우, 그 각도에 따라 캐릭터의 회전값이 변하도록 하는 함수
    /// </summary>
    public void SlopeCheck()
    {
        Vector3 front = new Vector3(transform.position.x + 1f, transform.position.y, 0);
        Vector3 back = new Vector3(transform.position.x - 1f, transform.position.y, 0);

        RaycastHit2D frontHit = Physics2D.Raycast(front, new Vector2(0, -1), Mathf.Infinity, slopeLayer);
        RaycastHit2D backHit = Physics2D.Raycast(back, new Vector2(0, -1), Mathf.Infinity, slopeLayer);

        Debug.DrawRay(front, new Vector3(0, -0.5f, 0), Color.red);
        Debug.DrawRay(back, new Vector3(0, -0.5f, 0), Color.red);

        // 레이캐스트가 경사면에 맞았는지 체크
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
    /// 거리를 설정하여 그 거리 안에 들 경우 플레이어를 추적하도록 하는 함수
    /// </summary>
    /// <param name="distance"></param>
    public void Trace(float distance)
    {
        // 자신과 플레이어 사이의 거리 계산(float)
        distanceToPlayer = Vector3.Distance(transform.position, PC.transform.position);

        // 자신과 플레이어 사이의 거리 계산(Vector2)
        Vector3 movement = -(transform.position - PC.transform.position).normalized * moveSpeed * Time.deltaTime;
        movement.y = 0;

        isTrace = false;

        // 플레이어와의 거리가 설정한 거리보다 작거나 같을 경우 추적 시작
        if (distance >= distanceToPlayer)
        {
            isTrace = true;

            // 바라보는 방향 설정(로컬 스케일 x 조정)
            SetDirection(movement, 1);

            transform.position += movement * moveSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// 추적하지 않을 때 주위를 순찰(배회)하도록 하는 함수
    /// </summary>
    public void Patrol()
    {
        // 자신과 시작 위치 사이의 거리 계산(float)
        distanceToOrigin = Vector2.Distance(transform.position, startPosition);

        if(Mathf.Abs(distanceToOrigin) > patrolRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, (moveSpeed / 10) * Time.deltaTime);
            SetDirection(startPosition, 1);
        }

        if (!isPatrol)
        {
            // 무작위 위치 생성
            randomPatrolPosition = new Vector3(Random.Range(-patrolRange, patrolRange), 0f, 0f);

            // 이전 위치 저장
            prevPosition = transform.position;

            isPatrol = true;
        }

        // 바라보는 방향 설정(로컬 스케일 x 조정)
        SetPatrolDirection(prevPosition + randomPatrolPosition, prevPosition, 1);

        // 목표 위치까지 이동
        transform.position = Vector3.MoveTowards(transform.position, startPosition + randomPatrolPosition, (moveSpeed / 10) * Time.deltaTime);

        // 벽체크. 이동중 앞에 벽이 있을경우 위치를 다시 설정
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position, 0.5f, groundLayer);

        if (colliders.Length > 0)
        {
            randomPatrolPosition = new Vector3(Random.Range(-patrolRange, patrolRange), 0f, 0f);
        }

        // 목표 위치에 도달했을 때
        if (Vector3.Distance(transform.position, startPosition + randomPatrolPosition) < 0.1f)
        {
            currentPatrolTime += Time.deltaTime;

            // 일정 시간 움직임 정지
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
    /// 앞에 벽이 있을경우 점프하게 하는 함수
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
    /// 방향을 설정하는 함수, scale은 스프라이트의 기본 크기에 맞게 설정
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="scale"></param>
    public void SetPatrolDirection(Vector2 dir, Vector2 prev, float scale)
    {
        // 바라보는 방향 설정(로컬 스케일 x 조정)
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
    /// 플레이어와의 거리가 가까울 경우 공격을 활성화하는 함수.
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
    /// 애니메이션 속 이벤트에 적용되는 함수. 1타 공격에 사용
    /// </summary>
    public void OnAttackCollision()
    {
        // 공격 애니메이션에 이벤트로 넣어서 활성화
        attackCollider.SetActive(true);
    }

    /// <summary>
    /// 컨트롤러 상태 초기화용 함수
    /// </summary>
    public void ResetController()
    {
        isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float slopeForce = moveSpeed;

        // 땅에 닿을 경우.
        if (collision.collider.CompareTag("Ground"))
        {
            jumpTime = 1f;
        }

        // 경사면에 닿을 경우. 경사면 위에서만 이동 속도 증가
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
