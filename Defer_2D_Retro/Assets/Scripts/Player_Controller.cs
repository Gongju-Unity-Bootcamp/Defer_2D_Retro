using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode jumpKey = KeyCode.Z; // ����Ű ����
    public KeyCode attackKey = KeyCode.X;   // ���� Ű ����

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
            rb.AddForce(Vector3.down * gravityForce);   // ������ �߷� ����
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
        // Move���� ���� ���� ���� ĳ���͸� �̵���Ŵ
        if (!isWallJump && !isHit && !isDead)
        {
            rb.AddForce(transform.right * moveVector * Time.fixedDeltaTime * 100f, ForceMode2D.Force);
        }

        DeadCheck();
    }

    /// <summary>
    /// ��� ���¸� üũ�ϴ� �Լ�
    /// </summary>
    private void DeadCheck()
    {
        isDead = PH.isDead;
        anim.SetBool("isDead", isDead);
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
    /// ĳ���� �� �� ���� üũ�ϴ� �Լ�
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
    /// ���� Ƚ���� �����ְ� �ɾ��ִ� ���°� �ƴ� �� �������� Ƣ������� �ϴ� �Լ�
    /// </summary>
    public void Jump()
    {
        if (jumpTime != 0 && !isCrouch)
        {
            if (Input.GetKeyDown(jumpKey))
            {
                jumpTime -= 1;
                anim.SetTrigger("Jump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);   // ���� Ƣ������� ��
            }
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
    /// �� üũ
    /// </summary>
    /// <returns>true �Ǵ� false</returns>
    public bool IsWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.3f, wallLayer);
    }

    /// <summary>
    /// ���� �پ��� �� �̲������� ���������� �ϴ� �Լ�
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

        // ������ �̲������� ���� ��� ��������Ʈ ������ �ذ��ϱ� ���� X�� ����
        if (isWallSlide)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }

        // ���� ���� �ִϸ��̼� ����� ����
        anim.SetBool("onWall", isWallSlide);
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
                transform.localScale = new Vector2(localScale.x, transform.localScale.y);
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

        // ���� �ִϸ��̼��� ��� ���� �� �ɴ� Ű�� ������ ��� ���ִ� ���¸� �����ϵ���
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
            // isCrouch�� �� �κ� ������ true�� �ٲ�� ������ ���� �����
            // ������ ��� ���߿� �� �ִ� ���� ���� ���� �Ʒ��� ���� ����
            if (!isCrouch)
            {
                rb.AddForce(Vector2.down * 10f, ForceMode2D.Impulse);
            }
            isCrouch = true;
            bc.size = new Vector2(0.21f, 0.3f);
            bc.offset = new Vector2(0, 0.02f);
            moveForce = 65f;
            anim.SetBool("Crouch", true);   // ���� �ִϸ��̼� ����� ����
        }
        if (!Input.GetKey(KeyCode.DownArrow) && isGround)
        {
            // ����ĳ��Ʈ�� ����� �� ���� ���¸� �����ϵ���
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
    /// ���� ���¿��� ������ ������ �ٶ󺸴� �������� �����̵� �ϵ��� �ϴ� �Լ�
    /// </summary>
    public void Sliding()
    {
        if(isCrouch && Input.GetKeyDown(jumpKey) && !isSliding)
        {
            isSliding = true;
            rb.AddForce(new Vector2(transform.localScale.x * slidingForce, 0), ForceMode2D.Impulse);

            Invoke(nameof(ResetSliding), 0.5f);

            anim.SetBool("Slide", true);    // �����̵� �ִϸ��̼� ����� ����
            Invoke(nameof(ResetAnimSliding), 0.1f);
        }
    }

    /// <summary>
    /// �����̵� �ʱ�ȭ�� �Լ�
    /// </summary>
    public void ResetSliding()
    {
        isSliding = false;
    }

    /// <summary>
    /// �ִϸ��̼��� �����̵� bool �ʱ�ȭ�� �Լ�
    /// </summary>
    public void ResetAnimSliding()
    {
        anim.SetBool("Slide", false);
    }

    /// <summary>
    /// ����Ű�� ���� �ִϸ������� attack Ʈ���Ÿ� Ȱ��ȭ �ϴ� �Լ�.
    /// </summary>
    public void Attack()
    {
        // ��� ����
        if (Input.GetKeyDown(attackKey) && !isMove)
        {
            anim.SetTrigger("Attack");  // ���� �ִϸ��̼� ����� ����
        }
    }

    /// <summary>
    /// �ִϸ��̼� �� �̺�Ʈ�� ����Ǵ� �Լ�. 1Ÿ ���ݿ� ���
    /// </summary>
    public void OnAttackCollision()
    {
        // ���� �ִϸ��̼ǿ� �̺�Ʈ�� �־ Ȱ��ȭ
        attackCollider1.SetActive(true);
    }

    /// <summary>
    /// �ִϸ��̼� �� �̺�Ʈ�� ����Ǵ� �Լ�. 2Ÿ ���ݿ� ���
    /// </summary>
    public void OnAttack2Collision()
    {
        // ���� �ִϸ��̼ǿ� �̺�Ʈ�� �־ Ȱ��ȭ
        attackCollider2.SetActive(true);
    }

    /// <summary>
    /// �ִϸ��̼� �� �̺�Ʈ�� ����Ǵ� �Լ�. ���� ���ݿ� ���
    /// </summary>
    public void OnCrouchAttackCollision()
    {
        // ���� �ִϸ��̼ǿ� �̺�Ʈ�� �־ Ȱ��ȭ
        crouchAttackCollider.SetActive(true);
    }

    /// <summary>
    /// ��Ʈ�ѷ� ���� �ʱ�ȭ�� �Լ�
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

        // ���鿡 ���� ���. ���� �������� �̵� �ӵ� ����
        if (collision.collider.CompareTag("Slope"))
        {
            isSlope = true;
            moveForce = slopeForce * 2f;
        }

        // ���Ϳ��� ���� ��� �ǰ�
        if (collision.collider.CompareTag("Monster") && !PH.isDead)
        {
            // ������ 10(�ӽ�)
            PH.TakeDamage(10);

            // ���� �ڼ��� ��� �ڽ��ݶ��̴� ũ��� ������ ���� ����
            if (isCrouch)
            {
                bc.size = new Vector2(0.21f, 0.38f);
                bc.offset = new Vector2(0, 0);
            }
            anim.SetTrigger("isHit");
            anim.SetFloat("Move", 0f);
            isHit = true;

            // �ǰ� �� �˹�
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