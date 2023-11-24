using System.Collections;
using UnityEngine;

public class Player_AttackCollision : MonoBehaviour
{
    [Header("Player")]
    public Player_Controller PC;

    [Header("Monster")]
    public Monster_Controller MC;

    [Header("Boss")]
    public Boss_Controller BC;

    [Header("Force")]
    public float knockbackForce = 15f; // 넉백 힘의 강도

    private void Awake()
    {
        MC = FindObjectOfType<Monster_Controller>();
        BC = FindObjectOfType<Boss_Controller>();
        PC = FindObjectOfType<Player_Controller>();
    }

    // 이 스크립트가 적용된 오브젝트가 활성화 될 시
    private void OnEnable()
    {
        StartCoroutine(nameof(AutoDisable));
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.CompareTag("Monster") && !other.GetComponent<Monster_Health>().isDead)
		{
            // 플레이어와 몬스터간의 거리
            // 이걸 사용하지 않고 가까이서 공격 시 넉백을 플레이어 방향으로 하기 때문에 계산
            Vector2 distance = new Vector2(transform.position.x - other.transform.position.x,
                transform.position.y - other.transform.position.y).normalized;
            Vector2 difference;

            // 플레이어의 로컬 스케일로 플레이어가 바라보는 방향을 결정
            // 거리가 가까울 경우(0보다 작을 경우) 밀려나게, 아닐 경우 평소대로
            if (PC.transform.localScale.x > 0)
            {
                if (distance.x < 0)
                {
                    difference = -(transform.position - other.transform.position).normalized;
                }
                else
                {
                    difference = (transform.position - other.transform.position).normalized;
                }
            }
            else
            {
                if (distance.x < 0)
                {
                    difference = (transform.position - other.transform.position).normalized;
                }
                else
                {
                    difference = -(transform.position - other.transform.position).normalized;
                }
            }

            // 피격받은 몬스터에게만 적용하기 위해
            other.GetComponent<Monster_Controller>().anim.SetTrigger("isHit");
            other.GetComponent<Monster_Controller>().isHit = true;

            // Player_Controller에서 데미지 값을 가져옴
            // 이후에 인게임에서 이벤트 등으로 데미지 증가 가능
            other.GetComponent<Monster_Health>().TakeDamage(PC.damage);

            Vector2 force = difference * knockbackForce;
            other.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

            // 피격받은 몬스터에게만 적용하기 위해
            other.GetComponent<Monster_Controller>().Invoke(nameof(MC.ResetController), 0.5f);

            Debug.Log("HIT");
        }

        if (other.CompareTag("Summons") && !other.GetComponent<Monster_Health>().isDead)
        {
            // Player_Controller에서 데미지 값을 가져옴
            // 이후에 인게임에서 이벤트 등으로 데미지 증가 가능
            other.GetComponent<Monster_Health>().TakeDamage(PC.damage);

            Debug.Log("HIT");
        }

        if (other.CompareTag("Boss") && !other.GetComponent<Monster_Health>().isDead)
        {
            // 피격받은 몬스터에게만 적용하기 위해
            other.GetComponent<Boss_Controller>().isHit = true;

            // Player_Controller에서 데미지 값을 가져옴
            // 이후에 인게임에서 이벤트 등으로 데미지 증가 가능
            other.GetComponent<Monster_Health>().TakeDamage(PC.damage);

            // 피격받은 몬스터에게만 적용하기 위해
            other.GetComponent<Boss_Controller>().Invoke(nameof(BC.ResetController), 0.5f);
        }
    }


    /// 0.05초 후에 오브젝트가 비활성화 되도록 한다
    private IEnumerator AutoDisable()
	{
		yield return new WaitForSeconds(0.05f);

		gameObject.SetActive(false);
    }
}

