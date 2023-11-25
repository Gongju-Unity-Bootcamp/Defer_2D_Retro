using System.Collections;
using UnityEngine;

public class Monster_AttackCollision : MonoBehaviour
{
    [Header("Monster")]
    public Monster_Controller MC;

    [Header("Boss")]
    public Boss_Controller BC;

    [Header("Player")]
    public Player_Controller PC;

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
		if (other.CompareTag("Player") && !other.GetComponent<Player_Health>().isDead)
		{
            // 몬스터와 플레이어간의 거리
            // 이걸 사용하지 않고 가까이서 공격 시 넉백을 플레이어 방향으로 하기 때문에 계산
            Vector2 distance = new Vector2(transform.position.x - other.transform.position.x,
                transform.position.y - other.transform.position.y).normalized;
            Vector2 difference;

            // 몬스터의 로컬 스케일로 몬스터가 바라보는 방향을 결정
            // 거리가 가까울 경우(0보다 작을 경우) 밀려나게, 아닐 경우 평소대로
            if (this.transform.parent.CompareTag("Boss"))
            {
                if (BC.transform.localScale.x > 0)
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
            }
            else
            {
                if (MC.transform.localScale.x > 0)
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
            }

            PC.anim.SetTrigger("isHit");
            PC.anim.SetFloat("Move", 0f);
            PC.isHit = true;

            // 부모 오브젝트의 컴포넌트(Monster_Controller)에서 값을 가져옴
            // 객체별 데미지를 위한 설정
            if (this.transform.parent.CompareTag("Monster"))
            {
                other.GetComponent<Player_Health>().TakeDamage(GetComponentInParent<Monster_Controller>().damage);
            }
            else if (this.transform.parent.CompareTag("Boss"))
            {
                other.GetComponent<Player_Health>().TakeDamage(GetComponentInParent<Boss_Controller>().damage);
            }

            Vector2 force = difference * knockbackForce;
            other.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

            PC.Invoke(nameof(PC.ResetController), 0.5f);

            Debug.Log("HIT");
        }
	}


    /// 0.05초 후에 오브젝트가 비활성화 되도록 한다
    private IEnumerator AutoDisable()
	{
		yield return new WaitForSeconds(0.05f);

		gameObject.SetActive(false);
    }
}

