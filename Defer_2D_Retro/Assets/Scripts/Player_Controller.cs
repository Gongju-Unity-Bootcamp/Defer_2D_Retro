using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode jumpKey = KeyCode.Z; // 점프키 설정
    public KeyCode attackKey = KeyCode.X;   // 공격 키 설정

    [Header("Player")]
    public Player_Health PH;

    [Header("Forces")]
    public float moveForce = 100f;
    public float gravityForce = 20f;
    public float jumpForce = 70f;
    public float wallSlideForce = 1f;
    public float slidingForce = 15f;
    public float knockbackForce = 40f;

    [Header("Bools")]
    public bool isMove = false;
    public bool isWallJump = false;
    public bool isWallSlide = false;
    public bool isGround = false;
    public bool isCrouch = false;
    public bool isSliding = false;
    public bool isSlope = false;
    public bool isHit = false;
    public bool isDead = false;

    [Header("Layers")]
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public LayerMask slopeLayer;

    [Header("Animation")]
    public Animator anim;

    [Header("Movement")]
    public float inputX;
    public Vector2 moveVector;
    public Rigidbody2D rb;

    [Header("GroundCheck")]
    public Transform groundCheck;

    [Header("Wall Jump")]
    public SpriteRenderer sr;
    public int jumpTime = 1;
    public float wallJumpDuration = 0.4f;
    public float wallJumpDirection;
    public float wallJumpCounter;
    public Transform wallCheck;
    public Vector2 wallJumpPower = new Vector2(10f, 65f);

    [Header("Sliding")]
    public BoxCollider2D bc;

    [Header("Attack")]
    public GameObject attackCollider1;
    public GameObject attackCollider2;
    public GameObject crouchAttackCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        PH = GetComponent<Player_Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            rb.AddForce(Vector3.down * gravityForce);   // 간단한 중력 구현
            if (!isHit)
            {
                Move();
                Jump();
                Crouch();
                Attack();
                Sliding();
            }
            GroundCheck();
            SlopeCheck();
            WallSlide();
            WallJump();
        }

    }

    private void FixedUpdate()
    {
        // Move에서 계산된 값을 토대로 캐릭터를 이동시킴
        if (!isWallJump && !isHit && !isDead)
        {
            rb.AddForce(transform.right * moveVector * Time.fixedDeltaTime * 100f, ForceMode2D.Force);
        }

        DeadCheck();
    }

    /// <summary>
    /// 사망 상태를 체크하는 함수
    /// </summary>
    private void DeadCheck()
    {
        isDead = PH.isDead;
        anim.SetBool("isDead", isDead);
    }

    /// <summary>
    /// 기본 움직임 값을 계산하는 함수
    /// </summary>
    public void Move()
    {
        inputX = Input.GetAxis("Horizontal");

        moveVector = new Vector2(inputX * moveForce, rb.velocity.y);

        if(inputX > 0)
        {
            transform.localScale = new Vector2(5, transform.localScale.y);
        }
        if (inputX < 0)
        {
            transform.localScale = new Vector2(-5, transform.localScale.y);
        }

        if (Mathf.Abs(inputX) >= 0.3f)
        {
            isMove = true;
        }
        else
        {
            isMove = false;
        }

        anim.SetFloat("Move", Mathf.Abs(inputX));
    }

    /// <summary>
    /// 캐릭터 발 밑 땅을 체크하는 함수
    /// </summary>
    public void GroundCheck()
    {
        isGround = false;
        anim.SetBool("isAir", true);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f, groundLayer);

        if(colliders.Length > 0)
        {
            anim.SetBool("isAir", false);
            isGround = true;

            if (jumpTime != 1)
            {
                jumpTime = 1;
            }
        }
    }

    /// <summary>
    /// 점프 횟수가 남아있고 앉아있는 상태가 아닐 시 공중으로 튀어오르게 하는 함수
    /// </summary>
    public void Jump()
    {
        if (jumpTime != 0 && !isCrouch)
        {
            if (Input.GetKeyDown(jumpKey))
            {
                jumpTime -= 1;
                anim.SetTrigger("Jump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);   // 위로 튀어오르는 힘
            }
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
    /// 벽 체크
    /// </summary>
    /// <returns>true 또는 false</returns>
    public bool IsWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.3f, wallLayer);
    }

    /// <summary>
    /// 벽에 붙었을 때 미끄러지며 떨어지도록 하는 함수
    /// </summary>
    public void WallSlide()
    {
        if(IsWall() && !isGround && inputX != 0f)
        {
            isWallSlide = true;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideForce, float.MaxValue));
        }
        else
        {
            isWallSlide = false;
        }

        // 벽에서 미끄러지고 있을 경우 스프라이트 문제를 해결하기 위해 X값 반전
        if (isWallSlide)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }

        // 벽에 붙은 애니메이션 재생을 위함
        anim.SetBool("onWall", isWallSlide);
    }

    /// <summary>
    /// 벽에 붙었을 때 점프키를 누를 시 반대 방향으로 튀어오르게 하는 함수
    /// </summary>
    public void WallJump()
    {
        if (isWallSlide)
        {
            isWallJump = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpCounter = 0.2f;

            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if(Input.GetKeyDown(jumpKey) && wallJumpCounter > 0f)
        {
            isWallJump = true;
            rb.velocity = new Vector3(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpCounter = 0f;
            
            if(transform.localScale.x != wallJumpDirection)
            {
                Vector3 localScale = transform.localScale;
                localScale *= -1f;
                transform.localScale = new Vector2(localScale.x, transform.localScale.y);
            }

            Invoke(nameof(StopWallJump), wallJumpDuration);
        }
    }

    /// <summary>
    /// 벽 점프 초기화용 함수
    /// </summary>
    public void StopWallJump()
    {
        isWallJump = false;
    }

    /// <summary>
    /// 아래 방향키를 누를시 캐릭터를 앉은 상태로 만드는 함수
    /// </summary>
    public void Crouch()
    {
        // 캐릭터 머리 위 벽 체크를 위한 레이캐스트
        RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.25f), transform.up, 1f);

        // 확인용 레이캐스트 그리기
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 1.25f), transform.up, Color.red);

        // 공격 애니메이션이 재생 중일 시 앉는 키를 눌러도 계속 서있는 상태를 유지하도록
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack2")))
        {
            anim.SetBool("Crouch", false);
            isCrouch = false;
            bc.size = new Vector2(0.21f, 0.38f);
            bc.offset = new Vector2(0, 0);
            if (!isSlope)
            {
                moveForce = 100f;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && isGround)
        {
            // isCrouch는 이 부분 다음에 true로 바뀌기 때문에 먼저 실행됨
            // 앉을때 잠시 공중에 떠 있는 것을 막기 위해 아래로 힘을 더함
            if (!isCrouch)
            {
                rb.AddForce(Vector2.down * 10f, ForceMode2D.Impulse);
            }
            isCrouch = true;
            bc.size = new Vector2(0.21f, 0.3f);
            bc.offset = new Vector2(0, 0.02f);
            moveForce = 65f;
            anim.SetBool("Crouch", true);   // 앉음 애니메이션 재생을 위함
        }
        if (!Input.GetKey(KeyCode.DownArrow) && isGround)
        {
            // 레이캐스트가 닿았을 때 앉은 상태를 유지하도록
            if (rayHit && isCrouch)
            {
                anim.SetBool("Crouch", true);
                bc.size = new Vector2(0.21f, 0.3f);
                bc.offset = new Vector2(0, 0.02f);
                if (!isSlope)
                {
                    moveForce = 65f;
                }
            }
            else
            {
                anim.SetBool("Crouch", false);
                isCrouch = false;
                bc.size = new Vector2(0.21f, 0.38f);
                bc.offset = new Vector2(0, 0);
                if (!isSlope)
                {
                    moveForce = 100f;
                }
            }
        }
    }

    /// <summary>
    /// 앉은 상태에서 점프를 누를시 바라보는 방향으로 슬라이딩 하도록 하는 함수
    /// </summary>
    public void Sliding()
    {
        if(isCrouch && Input.GetKeyDown(jumpKey) && !isSliding)
        {
            isSliding = true;
            rb.AddForce(new Vector2(transform.localScale.x * slidingForce, 0), ForceMode2D.Impulse);

            Invoke(nameof(ResetSliding), 0.5f);

            anim.SetBool("Slide", true);    // 슬라이딩 애니메이션 재생을 위함
            Invoke(nameof(ResetAnimSliding), 0.1f);
        }
    }

    /// <summary>
    /// 슬라이딩 초기화용 함수
    /// </summary>
    public void ResetSliding()
    {
        isSliding = false;
    }

    /// <summary>
    /// 애니메이션의 슬라이딩 bool 초기화용 함수
    /// </summary>
    public void ResetAnimSliding()
    {
        anim.SetBool("Slide", false);
    }

    /// <summary>
    /// 공격키를 눌러 애니메이터의 attack 트리거를 활성화 하는 함수.
    /// </summary>
    public void Attack()
    {
        // 평상 공격
        if (Input.GetKeyDown(attackKey) && !isMove)
        {
            anim.SetTrigger("Attack");  // 공격 애니메이션 재생을 위함
        }
    }

    /// <summary>
    /// 애니메이션 속 이벤트에 적용되는 함수. 1타 공격에 사용
    /// </summary>
    public void OnAttackCollision()
    {
        // 공격 애니메이션에 이벤트로 넣어서 활성화
        attackCollider1.SetActive(true);
    }

    /// <summary>
    /// 애니메이션 속 이벤트에 적용되는 함수. 2타 공격에 사용
    /// </summary>
    public void OnAttack2Collision()
    {
        // 공격 애니메이션에 이벤트로 넣어서 활성화
        attackCollider2.SetActive(true);
    }

    /// <summary>
    /// 애니메이션 속 이벤트에 적용되는 함수. 앉은 공격에 사용
    /// </summary>
    public void OnCrouchAttackCollision()
    {
        // 공격 애니메이션에 이벤트로 넣어서 활성화
        crouchAttackCollider.SetActive(true);
    }

    /// <summary>
    /// 컨트롤러 상태 초기화용 함수
    /// </summary>
    public void ResetController()
    {
        isHit = false;
        isCrouch = false;
        anim.SetBool("Crouch", false);
        anim.SetBool("Jump", false);
        anim.SetBool("isAir", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float slopeForce = moveForce;

        // 경사면에 닿을 경우. 경사면 위에서만 이동 속도 증가
        if (collision.collider.CompareTag("Slope"))
        {
            isSlope = true;
            moveForce = slopeForce * 2f;
        }

        // 몬스터에게 닿을 경우 피격
        if (collision.collider.CompareTag("Monster") && !PH.isDead)
        {
            // 데미지 10(임시)
            PH.TakeDamage(10);

            // 앉은 자세일 경우 박스콜라이더 크기와 오프셋 원상 복구
            if (isCrouch)
            {
                bc.size = new Vector2(0.21f, 0.38f);
                bc.offset = new Vector2(0, 0);
            }
            anim.SetTrigger("isHit");
            anim.SetFloat("Move", 0f);
            isHit = true;

            // 피격 시 넉백
            Vector2 difference = (transform.position - collision.transform.position).normalized;
            Vector2 force = difference * knockbackForce;
            rb.AddForce(force, ForceMode2D.Impulse);

            Invoke(nameof(ResetController), 0.5f);

            Debug.Log("Hurt");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        float slopeForce = moveForce;

        if (collision.collider.CompareTag("Slope"))
        {
            isSlope = false;

            if(moveForce > 100)
            {
                moveForce = slopeForce / 2f;
            }
        }
    }
}