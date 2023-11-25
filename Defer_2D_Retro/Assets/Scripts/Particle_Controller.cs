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
    public float dustFormationPeriod;   // ��ƼŬ�� �����ؼ� �߻����� �ʵ��� ������ �ð� ������ �α� ����

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
    /// �����̵� ��ƼŬ�� ��Ʈ���ϴ� �Լ�
    /// </summary>
    public void HandleSlideParticle()
    {
        // �ð��� ���� ī���� ����
        counter += Time.deltaTime;

        // �÷��̾ �����̵� ���̰�, ���� ����ְ�, �ɾ��ִ� �����̸�, x�� �ӵ��� ������ ������ ū ���
        if (PC.isSliding && PC.isGround && PC.isCrouch && Mathf.Abs(rb.velocity.x) > occurAfterVelocity)
        {
            // ������ �ð� ����(dustFormationPeriod)���� ��ƼŬ�� ���
            if (counter > dustFormationPeriod)
            {
                slidingParticle.Play();
                counter = 0;
            }
        }
    }

    public void HandleJumpParticle()
    {
        // ����Ű�� ������ �� + �� ����&�����̵� ���� �ƴҶ� + ���� Ƚ���� 0�� �ƴҶ�
        if (Input.GetKeyDown(PC.jumpKey) && !PC.isWallJump && !PC.isWallSlide && PC.jumpTime != 0)
        {
            jumpParticle.Play();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // �� �Ǵ� ���� ����ְ� isWallSlide�� true �� ��� �� ��ƼŬ ���
        if ((collision.CompareTag("Ground") || collision.CompareTag("Wall")) && PC.isWallSlide)
        {
            wallParticle.Play();
        }
    }
}
