using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Controller : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem slidingParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallParticle;

    [Header("Range")]
    [Range(0, 10)]
    public int occurAfterVelocity;
    [Range(0, 0.2f)]
    public float dustFormationPeriod;   // 파티클이 연속해서 발생하지 않도록 일정한 시간 간격을 두기 위함

    [Header("ETC")]
    public Rigidbody2D rb;
    public Player_Controller PC;

    public float counter;

    private void Start()
    {
        PC = FindObjectOfType<Player_Controller>();
    }

    private void Update()
    {
        HandleSlideParticle();
        HandleJumpParticle();
    }

    /// <summary>
    /// 슬라이딩 파티클을 컨트롤하는 함수
    /// </summary>
    public void HandleSlideParticle()
    {
        // 시간에 따른 카운터 증가
        counter += Time.deltaTime;

        // 플레이어가 슬라이딩 중이고, 땅에 닿아있고, 앉아있는 상태이며, x축 속도가 지정된 값보다 큰 경우
        if (PC.isSliding && PC.isGround && PC.isCrouch && Mathf.Abs(rb.velocity.x) > occurAfterVelocity)
        {
            // 지정된 시간 간격(dustFormationPeriod)마다 파티클을 재생
            if (counter > dustFormationPeriod)
            {
                slidingParticle.Play();
                counter = 0;
            }
        }
    }

    public void HandleJumpParticle()
    {
        // 점프키를 눌렀을 때 + 벽 점프&슬라이딩 중이 아닐때 + 점프 횟수가 0이 아닐때
        if (Input.GetKeyDown(PC.jumpKey) && !PC.isWallJump && !PC.isWallSlide && PC.jumpTime != 0)
        {
            jumpParticle.Play();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 땅 또는 벽에 닿아있고 isWallSlide가 true 일 경우 벽 파티클 재생
        if ((collision.CompareTag("Ground") || collision.CompareTag("Wall")) && PC.isWallSlide)
        {
            wallParticle.Play();
        }
    }
}
