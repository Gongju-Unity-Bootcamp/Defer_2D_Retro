using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode jumpKey = KeyCode.Z; // ����Ű ����

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
        rb.AddForce(Vector3.down * gravityForce);   // ������ �߷� ����
        Move();
        Jump();
        Crouch();
        WallSlide();
        WallJump();
        Sliding();
    }

    private void FixedUpdate()
    {
        // Move���� ���� ���� ���� ĳ���͸� �̵���Ŵ
        if (!isWallJump)
        {
            rb.AddForce(transform.right * moveVector * Time.fixedDeltaTime * 100f, ForceMode2D.Force);
        }
    }

    /// <summary>
    /// �⺻ ������ ���� ����ϴ� �Լ�
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
    /// ���� Ƚ���� �����ְ� �ɾ��ִ� ���°� �ƴ� �� �������� Ƣ������� �ϴ� �Լ�
    /// </summary>
    public void Jump()
    {
        if (jumpTime != 0 && !isCrouch)
        {
            if (Input.GetKeyDown(jumpKey))
            {
                isJump = true;
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);   // ���� Ƣ������� ��
                jumpTime -= 1;
            }
        }
    }

    /// <summary>
    /// �� üũ
    /// </summary>
    /// <returns>true �Ǵ� false</returns>
    public bool IsWall1()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.3f, wallLayer);
    }

    /// <summary>
    /// ���� �پ��� �� �̲������� ���������� �ϴ� �Լ�
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
    /// ���� �پ��� �� ����Ű�� ���� �� �ݴ� �������� Ƣ������� �ϴ� �Լ�
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
    /// �� ���� �ʱ�ȭ�� �Լ�
    /// </summary>
    public void StopWallJump()
    {
        isWallJump = false;
    }

    /// <summary>
    /// �Ʒ� ����Ű�� ������ ĳ���͸� ���� ���·� ����� �Լ�
    /// </summary>
    public void Crouch()
    {
        // ĳ���� �Ӹ� �� �� üũ�� ���� ����ĳ��Ʈ
        RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.25f), transform.up, 1f);

        // Ȯ�ο� ����ĳ��Ʈ �׸���
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 1.25f), transform.up, Color.red);

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // isCrouch�� �� �κ� ������ true�� �ٲ�� ������ ���� �����
            // ������ ��� ���߿� �� �ִ� ���� ���� ���� �Ʒ��� ���� ����
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
            // ����ĳ��Ʈ�� ����� �� ���� ���¸� �����ϵ���
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
    /// ���� ���¿��� ������ ������ �ٶ󺸴� �������� �����̵� �ϵ��� �ϴ� �Լ�
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
    /// �����̵� �ʱ�ȭ�� �Լ�
    /// </summary>
    public void ResetSliding()
    {
        isSliding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float slopeForce = moveForce;

        // �� �Ǵ� ���鿡 ���� ��� ���� Ƚ���� ���� ����, ���� Ƚ���� �ʱ�ȭ
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Slope"))
        {
            isGround = true;
            isJump = false;

            if (jumpTime >= 0)
            {
                jumpTime = 2;
            }
        }
        
        // ���� ���� ��� ���� Ƚ���� ���� ���¸� �ʱ�ȭ
        if (collision.collider.CompareTag("Wall"))
        {
            isJump = false;

            if (jumpTime >= 0)
            {
                jumpTime = 2;
            }
        }

        // ���鿡 ���� ���. ���� �������� �̵� �ӵ� ����
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