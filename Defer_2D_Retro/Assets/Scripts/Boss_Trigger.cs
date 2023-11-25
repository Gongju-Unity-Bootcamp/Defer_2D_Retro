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
        // spawnedBoss�� null�� �ƴϰ� spawnedBoss�� isDead�� true�� ��
        // ���� ����(���� ������Ʈ�� �ξ��� ������ �ϳ��� �����ص� ��� ����)
        if (spawnedBoss != null && spawnedBoss.GetComponent<Monster_Health>().isDead)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ʈ����(Boss Area)�� �÷��̾ ���� �� ������ ��ȯ
        // �ش� ���� �ݶ��̴� ��Ȱ��ȭ �� �¿� ���� ���� Ȱ��ȭ
        if (collision.gameObject.CompareTag("Player"))
        {
            spawnedBoss = Instantiate(boss, spawnPoint.position, spawnPoint.rotation);
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            area1.SetActive(true);
            area2.SetActive(true);
        }
    }
}
