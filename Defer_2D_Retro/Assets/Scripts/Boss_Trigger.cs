using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Trigger : MonoBehaviour
{
    [Header("Boss")]
    public GameObject boss;
    public Transform spawnPoint;
    public GameObject spawnedBoss;

    [Header("Boss Area")]
    public GameObject area1;
    public GameObject area2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // spawnedBoss가 null이 아니고 spawnedBoss의 isDead가 true일 시
        // 영역 제거(하위 오브젝트로 두었기 때문에 하나만 제거해도 모두 제거)
        if (spawnedBoss != null && spawnedBoss.GetComponent<Monster_Health>().isDead)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거(Boss Area)에 플레이어가 들어올 시 보스를 소환
        // 해당 영역 콜라이더 비활성화 및 좌우 제한 영역 활성화
        if (collision.gameObject.CompareTag("Player"))
        {
            spawnedBoss = Instantiate(boss, spawnPoint.position, spawnPoint.rotation);
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            area1.SetActive(true);
            area2.SetActive(true);
        }
    }
}
