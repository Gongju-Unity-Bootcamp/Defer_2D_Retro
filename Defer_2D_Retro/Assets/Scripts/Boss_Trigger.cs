using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Trigger : MonoBehaviour
{
    [Header("Boss")]
    public GameObject boss;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거(Boss Area)에 플레이어가 들어올 시 보스를 소환, 해당 영역은 제거
        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(boss, spawnPoint.position, spawnPoint.rotation);
            Destroy(this.gameObject);
        }
    }
}
