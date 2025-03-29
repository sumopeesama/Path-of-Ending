using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public int maxStamina = 50;
    public int currentStamina;
    public float staminaRegenRate = 5f; // Stamina per second

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        RegenerateStamina();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void UseStamina(int amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
        }
    }

    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += Mathf.RoundToInt(staminaRegenRate * Time.deltaTime);
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        // Add respawn or game over logic here
    }
}
