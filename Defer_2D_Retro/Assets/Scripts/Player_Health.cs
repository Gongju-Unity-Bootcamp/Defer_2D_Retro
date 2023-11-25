using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Slider")]
    public Slider healthSlider;
    public Image fillImage;
    public TextMeshProUGUI healthText;
    public float decreaseSpeed = 5f;

    [Header("Color")]
    public Gradient gradient;

    [Header("Bools")]
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        SetHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        UpdateHealthText(currentHealth, maxHealth);
        CheckHealth();

        if (isDead)
        {
            UIManager.instance.ShowGameOver();
        }
    }

    /// <summary>
    /// ü���� üũ�Ͽ� ��� ���¸� �����ϴ� �Լ�. ��ġ�� y -15�� ��쿡�� ���
    /// </summary>
    public void CheckHealth()
    {
        if(currentHealth == 0 || transform.position.y <= -50f)
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
            // ���� ü���� �ִ� ü���� ���� ���ϰ�
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // Lerp �Լ��� ����Ͽ� ü�¹ٸ� �ε巴�� ���ҽ�Ŵ
            float newHealthValue = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * decreaseSpeed);
            healthSlider.value = newHealthValue;

            // �����̴��� �̹��� ������ ������ ������ ������ ����
            fillImage.color = gradient.Evaluate(healthSlider.normalizedValue);
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

    public void UpdateHealthText(float currentHP, float maxHP)
    {
        // ü�¹� �ؽ�Ʈ ������Ʈ
        healthText.text = string.Format($"{currentHP}/{maxHP}");
    }

    /// <summary>
    /// �������� �־� ü���� ���ҽ�Ű�� �Լ�
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
