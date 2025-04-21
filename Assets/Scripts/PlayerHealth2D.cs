using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CharacterStats))]
public class PlayerHealth2D : MonoBehaviour
{
    [Header("References")]
    public Image healthBar;
    public GameObject damageEffect;

    [Header("Invulnerability")]
    public float invulnerabilityTime = 1f;
    public bool flashOnDamage = true;

    private CharacterStats characterStats;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isInvulnerable = false;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (healthBar != null)
        {
            UpdateHealthBar();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;

        // Convert float damage to int for CharacterStats
        int damageAmount = Mathf.RoundToInt(damage);

        // Apply damage using CharacterStats
        characterStats.TakeDamage(damageAmount);

        // Update health bar
        UpdateHealthBar();

        // Play damage effect if available
        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        // Trigger hit animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // Visual feedback
        if (flashOnDamage && spriteRenderer != null)
        {
            StartCoroutine(FlashSprite());
        }

        // Become temporarily invulnerable
        StartCoroutine(BecomeInvulnerable());
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)characterStats.currentHealth / characterStats.maxHealth;
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

    // Expose a method to heal the player if needed
    public void Heal(float amount)
    {
        int healAmount = Mathf.RoundToInt(amount);
        characterStats.Heal(healAmount);
        UpdateHealthBar();
    }

    // Add this method to check if player is dead
    public bool IsDead()
    {
        return characterStats.currentHealth <= 0;
    }
}