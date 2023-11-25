using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summons_Controller : MonoBehaviour
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
    public float moveSpeed = 20f;

    [Header("Bools")]
    public bool hasAddedScore = false;

    [Header("Animation")]
    public Animator anim;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!MH.isDead && !anim.GetCurrentAnimatorStateInfo(0).IsName("Summons_Summon"))
        {
            // 공격중이 아닐때 + 피격 상태가 아닐때 + 공격 애니메이션 재생중이 아닐때
            if (!MH.isHit)
            {
                Trace();
            }
        }
        else if (MH.isDead && !hasAddedScore)
        {
            Destroy(this.gameObject, 0.85f);
            GameManager.instance.AddScore(10);
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
        if (MH.isDead)
        {
            anim.SetBool("isDead", true);
        }
        else
        {
            anim.SetBool("isDead", false);
        }
    }

    /// <summary>
    /// 거리를 설정하여 그 거리 안에 들 경우 플레이어를 추적하도록 하는 함수
    /// </summary>
    public void Trace()
    {
        // 자신과 플레이어 사이의 거리 계산(Vector2)
        Vector3 movement = -(transform.position - PC.transform.position).normalized * moveSpeed * Time.deltaTime;

        // 바라보는 방향 설정(로컬 스케일 x 조정)
        SetDirection(movement, 1);

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
        else if (dir.x > 0)
        {
            transform.localScale = new Vector2(scale, transform.localScale.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            this.GetComponent<Monster_Health>().currentHealth = 0;
        }
        // isTrigger를 사용하지 않아도 벽을 통과할 수 있도록
        else if (!collision.collider.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
