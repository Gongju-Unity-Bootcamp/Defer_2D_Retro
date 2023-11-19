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
    /// 체력을 체크하여 사망 상태를 결정하는 함수
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
    /// 체력 값과 슬라이더 값을 업데이트 하는 함수
    /// </summary>
    public void UpdateHealth()
    {
        if (currentHealth != healthSlider.value)
        {
            // Lerp 함수를 사용하여 체력바를 부드럽게 감소시킴
            float newHealthValue = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * decreaseSpeed);
            healthSlider.value = newHealthValue;
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

    /// <summary>
    /// 데미지를 주어 체력을 감소시키는 함수
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
        cg.alpha = 1.0f; // 피격시 체력바 보이게

        yield return new WaitForSeconds(2.5f);

        cg.alpha = 0; // 피격시 체력바 안보이게
    }

    private IEnumerator DisableHealthBar()
    {
        yield return new WaitForSeconds(2.5f); // 사망 애니메이션이 재생되는 동안 대기

        cg.alpha = 0; // 체력바 숨김
    }
}
