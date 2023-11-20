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
    public Vector3 startPosition;   // 시작 위치
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

        // 초기 위치 저장
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Vector3.down * 20f);   // 간단한 중력 구현
        Patrol();
        Trace(traceDistance);
        Jump();
        SlopeCheck();
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
        if(distance >= distanceToPlayer)
        {
            isTrace = true;

            // 바라보는 방향 설정(로컬 스케일 x 조정)
            SetDirection(movement, 1);

            // 피격 상태가 아닐때만 추적
            if (!MH.isHit)
            {
                // ReturnToBase의 Invoke 실행을 취소
                CancelInvoke(nameof(ReturnToBase));
                transform.position += movement * moveSpeed * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 추적하지 않을 때 주위를 순찰(배회)하도록 하는 함수
    /// </summary>
    public void Patrol()
    {
        // 초기 위치와 현재 위치 사이의 거리(float)
        distanceToOrigin = Vector2.Distance(transform.position, startPosition);

        // 초기 위치와 현재 위치 사이의 거리(Vector2)
        Vector2 movement = -(transform.position - startPosition).normalized * moveSpeed * Time.deltaTime;
        movement.y = 0;

        // 바라보는 방향 설정(로컬 스케일 x 조정)
        SetDirection(movement, 1);

        if (!isTrace)
        {
            if(distanceToOrigin > 1f)
            {
                // 5초뒤 ReturnToBase 실행
                Invoke(nameof(ReturnToBase), 5f);
            }
        }
    }

    /// <summary>
    /// 앞에 벽이 있을경우 점프하게 하는 함수
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
    /// 시작 위치로 복귀하도록 만드는 함수
    /// </summary>
    public void ReturnToBase()
    {
        Vector3 movement = -(transform.position - startPosition).normalized * moveSpeed * Time.deltaTime;
        movement.y = 0;

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
        if (dir.x > 0)
        {
            transform.localScale = new Vector2(scale, transform.localScale.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float slopeForce = moveSpeed;

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
