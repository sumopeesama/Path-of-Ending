using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBar;
    public float invulnerabilityTime = 0.5f;

    [Header("Stamina Settings")]
    public int maxStamina = 50;
    public int currentStamina;
    public Image staminaBar;
    public float staminaRegenRate = 5f; // Stamina per second
    public float staminaRegenDelay = 1f; // Delay before stamina regen starts
    private float lastStaminaUseTime;

    [Header("Combat Stats")]
    public int attackPower = 15;
    public int defense = 5;
    public float attackSpeed = 1f;
    public float criticalChance = 0.05f; // 5% chance
    public float criticalMultiplier = 1.5f;

    [Header("Effects")]
    public bool showDamageNumbers = true;
    public GameObject damageTextPrefab;
    public GameObject healTextPrefab;

    private bool isInvulnerable = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Events
    public delegate void OnHealthChangedDelegate(int currentHealth, int maxHealth);
    public event OnHealthChangedDelegate OnHealthChanged;

    public delegate void OnStaminaChangedDelegate(int currentStamina, int maxStamina);
    public event OnStaminaChangedDelegate OnStaminaChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        UpdateHealthBar();
        UpdateStaminaBar();
    }

    private void Update()
    {
        RegenerateStamina();
    }

    public bool TakeDamage(int rawDamage, bool isCritical = false)
    {
        if (isInvulnerable) return false;

        // Apply defense to reduce damage
        int actualDamage = Mathf.Max(1, rawDamage - defense);

        // Apply critical multiplier if needed
        if (isCritical)
        {
            actualDamage = Mathf.RoundToInt(actualDamage * criticalMultiplier);
        }

        currentHealth -= actualDamage;

        // Trigger health changed event
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Update UI
        UpdateHealthBar();

        // Show damage number
        if (showDamageNumbers && damageTextPrefab != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity);
            damageText.GetComponent<DamageText>()?.SetText(actualDamage, isCritical);
        }

        // Play hit animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // Visual feedback
        StartCoroutine(FlashSprite());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return true;
        }
        else
        {
            StartCoroutine(BecomeInvulnerable());
            return false;
        }
    }

    public void Heal(int amount)
    {
        int actualHeal = Mathf.Min(maxHealth - currentHealth, amount);
        currentHealth += actualHeal;

        // Trigger health changed event
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Update UI
        UpdateHealthBar();

        // Show heal number
        if (showDamageNumbers && healTextPrefab != null && actualHeal > 0)
        {
            GameObject healText = Instantiate(healTextPrefab, transform.position + Vector3.up, Quaternion.identity);
            healText.GetComponent<DamageText>()?.SetText(actualHeal, false);
        }
    }

    public bool UseStamina(int amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            lastStaminaUseTime = Time.time;

            // Trigger stamina changed event
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);

            // Update UI
            UpdateStaminaBar();
            return true;
        }
        return false;
    }

    private void RegenerateStamina()
    {
        // Only regenerate after delay
        if (Time.time < lastStaminaUseTime + staminaRegenDelay)
            return;

        if (currentStamina < maxStamina)
        {
            currentStamina += Mathf.RoundToInt(staminaRegenRate * Time.deltaTime);

            // Clamp to max stamina
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            // Trigger stamina changed event
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);

            // Update UI
            UpdateStaminaBar();
        }
    }

    public int CalculateDamage(bool isHeavyAttack = false)
    {
        // Base damage
        int damage = attackPower;

        // Heavy attack multiplier
        if (isHeavyAttack)
        {
            damage = Mathf.RoundToInt(damage * 1.5f);
        }

        // Critical hit chance
        bool isCritical = Random.value <= criticalChance;
        if (isCritical)
        {
            damage = Mathf.RoundToInt(damage * criticalMultiplier);
        }

        return damage;
    }

    public bool HasEnoughStamina(int amount)
    {
        return currentStamina >= amount;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private void UpdateStaminaBar()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = (float)currentStamina / maxStamina;
        }
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    private IEnumerator FlashSprite()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        Color damageColor = Color.red;

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");

        // Disable control components
        if (GetComponent<PlayerController2D>() != null)
            GetComponent<PlayerController2D>().enabled = false;

        // Disable collider to prevent further interactions
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

        // Disable rigidbody physics
        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // Play death animation if available
        if (animator != null)
            animator.SetTrigger("Die");

        // Game over
        if (GameManager.Instance != null)
            GameManager.Instance.LoseGame();
    }
}