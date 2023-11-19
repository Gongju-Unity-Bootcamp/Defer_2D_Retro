using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Health : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public CanvasGroup cg;

    [Header("Slider")]
    public Slider healthSlider;
    public Image fillImage;
    public float decreaseSpeed = 5f;

    [Header("Bools")]
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        SetHealth(maxHealth);
    }

    private void Awake()
    {
        cg.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        CheckHealth();

        if (Input.GetKeyUp(KeyCode.T))
        {
            SetHealth(maxHealth);
        }
    }

    private void LateUpdate()
    {
        if (isDead)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ü���� üũ�Ͽ� ��� ���¸� �����ϴ� �Լ�
    /// </summary>
    public void CheckHealth()
    {
        if (currentHealth == 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
    }

    /// <summary>
    /// ü�� ���� �����̴� ���� ������Ʈ �ϴ� �Լ�
    /// </summary>
    public void UpdateHealth()
    {
        if (currentHealth != healthSlider.value)
        {
            // Lerp �Լ��� ����Ͽ� ü�¹ٸ� �ε巴�� ���ҽ�Ŵ
            float newHealthValue = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * decreaseSpeed);
            healthSlider.value = newHealthValue;
        }
    }

    /// <summary>
    /// �ִ� ü�°� ���� ü���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="maxHP"></param>
    public void SetHealth(float maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = maxHP;
        maxHealth = maxHP;
        currentHealth = maxHP;
    }

    /// <summary>
    /// �������� �־� ü���� ���ҽ�Ű�� �Լ�
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        if(isDead)
        { 
            return; 
        }

        StopCoroutine("Visible");

        StartCoroutine("Visible");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private IEnumerator Visible()
    {
        cg.alpha = 1.0f; // �ǰݽ� ü�¹� ���̰�

        yield return new WaitForSeconds(2.5f);

        cg.alpha = 0; // �ǰݽ� ü�¹� �Ⱥ��̰�
    }

    private IEnumerator DisableHealthBar()
    {
        yield return new WaitForSeconds(2.5f); // ��� �ִϸ��̼��� ����Ǵ� ���� ���

        cg.alpha = 0; // ü�¹� ����
    }
}
