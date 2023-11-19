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
    public float dustFormationPeriod;

    [Header("ETC")]
    public Rigidbody2D rb;
    public Player_Controller pc;

    public float counter;

    private void Start()
    {
        pc = FindObjectOfType<Player_Controller>();
    }

    private void Update()
    {
        counter += Time.deltaTime;

        if(pc.isSliding && pc.isGround && pc.isCrouch && Mathf.Abs(rb.velocity.x) > occurAfterVelocity)
        {
            if(counter > dustFormationPeriod)
            {
                slidingParticle.Play();
                counter = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.CompareTag("Ground") || collision.CompareTag("Slope")) && pc.isGround)
        {
            jumpParticle.Play();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && pc.isWallSlide)
        {
            wallParticle.Play();
        }
    }
}
