using System.Collections;
using UnityEngine;

public class Player_AttackCollision : MonoBehaviour
{
    public float knockBackForce = 3f; // 넉백 힘의 강도

    private void Awake()
    {
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(AutoDisable));
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.CompareTag("Monster"))
		{
			Debug.Log("HIT");
		}
	}

	private IEnumerator AutoDisable()
	{
		// 0.05초 후에 오브젝트가 비활성화 되도록 한다
		yield return new WaitForSeconds(0.05f);

		gameObject.SetActive(false);
    }
}

