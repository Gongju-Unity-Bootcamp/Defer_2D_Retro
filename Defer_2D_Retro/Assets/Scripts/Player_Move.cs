using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode jumpKey = KeyCode.Z; // 점프키 설정

    [Header("Movement")]
    public float inputX;
    public Vector2 moveVector;
    public Rigidbody2D rb;

    [Header("Forces")]
    public float moveForce = 100f;
    public float gravityForce = 20f;
    public float jumpForce = 70f;
    public float wallSlideForce = 5f;
    public float slidingForce = 50f;

    [Header("Bools")]
    public bool isJump = false;
    public bool isWallJump = false;
    public bool isWallSlide = false;
    public bool isGround = false;
    public bool isCrouch = false;
    public bool isSliding = false;
    public bool isSlope = false;

    [Header("Wall Jump")]
    public int jumpTime = 2;
    public float wallJumpDuration = 0.4f;
    public float wallJumpDirection;
    public float wallJumpCounter;
    public Transform wallCheck;
    public Vector2 wallJumpPower = new Vector2(50f, 80f);

    [Header("Sliding")]
    public CapsuleCollider2D cc;

    [Header("Layers")]
    public LayerMask wallLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Vector3.down * gravityForce);   // 간단한 중력 구현
        Move();
        Jump();
        Crouch();
        WallSlide();
        WallJump();
        Sliding();
    }

    private void FixedUpdate()
    {
        // Move에서 계산된 값을 토대로 캐릭터를 이동시킴
        if (!isWallJump)
        {
            rb.AddForce(transform.right * moveVector * Time.fixedDeltaTime * 100f, ForceMode2D.Force);
        }
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
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        if (inputX < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
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
                isJump = true;
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);   // 위로 튀어오르는 힘
                jumpTime -= 1;
            }
        }
    }

    /// <summary>
    /// 벽 체크
    /// </summary>
    /// <returns>true 또는 false</returns>
    public bool IsWall1()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.3f, wallLayer);
    }

    /// <summary>
    /// 벽에 붙었을 때 미끄러지며 떨어지도록 하는 함수
    /// </summary>
    public void WallSlide()
    {
        if(IsWall1() && !isGround && inputX != 0f)
        {
            isWallSlide = true;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideForce, float.MaxValue));
        }
        else
        {
            isWallSlide = false;
        }
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
                transform.localScale = new Vector2(localScale.x, 1);
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

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // isCrouch는 이 부분 다음에 true로 바뀌기 때문에 먼저 실행됨
            // 앉을때 잠시 공중에 떠 있는 것을 막기 위해 아래로 힘을 더함
            if (!isCrouch)
            {
                rb.AddForce(Vector2.down * 10f, ForceMode2D.Impulse);
            }
            isCrouch = true;
            cc.size = new Vector2(1, 1);
            transform.localScale = new Vector2(transform.localScale.x, 0.5f);
            moveForce = 65f;
        }
        if (!Input.GetKey(KeyCode.DownArrow) && !isJump)
        {
            // 레이캐스트가 닿았을 때 앉은 상태를 유지하도록
            if (rayHit && isCrouch)
            {
                cc.size = new Vector2(1, 1);
                transform.localScale = new Vector2(transform.localScale.x, 0.5f);
                if (!isSlope)
                {
                    moveForce = 65f;
                }
            }
            else
            {
                isCrouch = false;
                cc.size = new Vector2(1, 2);
                transform.localScale = new Vector2(transform.localScale.x, 1f);
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

            Invoke(nameof(ResetSliding), 1f);
        }
    }

    /// <summary>
    /// 슬라이딩 초기화용 함수
    /// </summary>
    public void ResetSliding()
    {
        isSliding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float slopeForce = moveForce;

        // 땅 또는 경사면에 닿을 경우 점프 횟수와 점프 상태, 점프 횟수를 초기화
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Slope"))
        {
            isGround = true;
            isJump = false;

            if (jumpTime >= 0)
            {
                jumpTime = 2;
            }
        }
        
        // 벽에 닿을 경우 점프 횟수와 점프 상태만 초기화
        if (collision.collider.CompareTag("Wall"))
        {
            isJump = false;

            if (jumpTime >= 0)
            {
                jumpTime = 2;
            }
        }

        // 경사면에 닿을 경우. 경사면 위에서만 이동 속도 증가
        if (collision.collider.CompareTag("Slope"))
        {
            isSlope = true;
            moveForce = slopeForce * 2f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        float slopeForce = moveForce;

        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Slope"))
        {
            isGround = false;
        }

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