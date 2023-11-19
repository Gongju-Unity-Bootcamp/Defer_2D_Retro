using System.Collections;
using UnityEngine;

public class Player_AttackCollision : MonoBehaviour
{
    [Header("Player")]
    public Player_Controller pc;

    [Header("Force")]
    public float knockbackForce = 8f; // 넉백 힘의 강도

    private void Awake()
    {
        pc = FindObjectOfType<Player_Controller>();
    }

    // 이 스크립트가 적용된 오브젝트가 활성화 될 시
    private void OnEnable()
    {
        StartCoroutine(nameof(AutoDisable));
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.CompareTag("Monster"))
		{
            // 플레이어와 몬스터간의 거리
            // 이걸 사용하지 않고 가까이서 공격 시 넉백을 플레이어 방향으로 하기 때문에 계산
            Vector2 distance= new Vector2(transform.position.x - other.transform.position.x,
                transform.position.y - other.transform.position.y).normalized;
            Vector2 difference;

            // 플레이어의 로컬 스케일로 플레이어가 바라보는 방향을 결정
            // 거리가 가까울 경우(0보다 작을 경우) 밀려나게, 아닐 경우 평소대로
            if (pc.transform.localScale.x > 0)
            {
                if (distance.x < 0 && distance.y < 0)
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
                if (distance.x < 0 && distance.y < 0)
                {
                    difference = (transform.position - other.transform.position).normalized;
                }
                else
                {
                    difference = -(transform.position - other.transform.position).normalized;
                }
            }

            Vector2 force = difference * knockbackForce;
            other.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

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

