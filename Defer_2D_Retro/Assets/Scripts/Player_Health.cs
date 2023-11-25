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
    /// 체력을 체크하여 사망 상태를 결정하는 함수. 위치가 y -15일 경우에도 사망
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
    /// 체력 값과 슬라이더 값을 업데이트 하는 함수
    /// </summary>
    public void UpdateHealth()
    {
        if (currentHealth != healthSlider.value)
        {
            // 현재 체력이 최대 체력을 넘지 못하게
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // Lerp 함수를 사용하여 체력바를 부드럽게 감소시킴
            float newHealthValue = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * decreaseSpeed);
            healthSlider.value = newHealthValue;

            // 슬라이더의 이미지 색상을 설정한 색으로 서서히 변경
            fillImage.color = gradient.Evaluate(healthSlider.normalizedValue);
        }
    }

    /// <summary>
    /// 최대 체력과 시작 체력을 설정하는 함수
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
        // 체력바 텍스트 업데이트
        healthText.text = string.Format($"{currentHP}/{maxHP}");
    }

    /// <summary>
    /// 데미지를 주어 체력을 감소시키는 함수
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
