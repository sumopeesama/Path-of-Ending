using System.Collections;
using UnityEngine;

public class BossController2D : MonoBehaviour
{
    // Boss Stats
    [Header("Boss Stats")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public float attackDamage = 25f;
    public float attackSpeed = 1.5f; // Attacks per second
    public float movementSpeed = 3.5f;
    public float attackCooldown = 2f;
    public float skillCooldown = 8f;

    // Enrage Settings
    [Header("Enrage Settings")]
    public float enrageHealthThreshold = 0.3f; // 30% of max health
    public float enragedDamageMultiplier = 1.5f;
    public float enragedSpeedMultiplier = 1.3f;
    public float enragedAttackSpeedMultiplier = 1.4f;
    public bool isEnraged = false;
    public Color enragedColor = Color.red;
    private Color originalColor;

    // Attack Indicators
    [Header("Attack Indicators")]
    public GameObject normalAttackIndicator;
    public GameObject leapAttackIndicator;
    public float indicatorDisplayTime = 1.5f;

    // References
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isAttacking = false;
    private bool isUsingSkill = false;
    private float lastAttackTime;
    private float lastSkillTime;
    private SpriteRenderer bossRenderer;
    private bool isFacingRight = true;

    void Start()
    {
        // Initialize boss
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bossRenderer = GetComponent<SpriteRenderer>();
        originalColor = bossRenderer.color;

        // Initialize cooldowns
        lastAttackTime = -attackCooldown;
        lastSkillTime = -skillCooldown;

        // Hide indicators initially
        if (normalAttackIndicator) normalAttackIndicator.SetActive(false);
        if (leapAttackIndicator) leapAttackIndicator.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        // Check if boss should enter enraged state
        if (!isEnraged && currentHealth / maxHealth <= enrageHealthThreshold)
        {
            EnterEnragedState();
        }

        // Check for attack opportunities
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if in range to attack
        if (!isAttacking && !isUsingSkill && distanceToPlayer <= 2f)
        {
            // Try normal attack
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(PerformAttack());
            }
        }
        // Check if skill is ready
        else if (!isAttacking && !isUsingSkill && Time.time >= lastSkillTime + skillCooldown)
        {
            StartCoroutine(PerformLeapAttack());
        }
        // Move towards player when not attacking
        else if (!isAttacking && !isUsingSkill)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        // Get direction to player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move towards player
        rb.linearVelocity = direction * (isEnraged ? movementSpeed * enragedSpeedMultiplier : movementSpeed);

        // Flip sprite based on movement direction
        if (direction.x > 0 && !isFacingRight)
        {
            FlipSprite();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            FlipSprite();
        }

        // Set animation if available
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
    }

    void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        // Face the player
        if ((player.position.x > transform.position.x && !isFacingRight) ||
            (player.position.x < transform.position.x && isFacingRight))
        {
            FlipSprite();
        }

        // Set animation
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        // Calculate attack position (in front of the boss)
        Vector2 attackPosition;
        if (isFacingRight)
        {
            attackPosition = (Vector2)transform.position + new Vector2(1.5f, 0);
        }
        else
        {
            attackPosition = (Vector2)transform.position + new Vector2(-1.5f, 0);
        }

        // Show attack indicator
        normalAttackIndicator.transform.position = new Vector3(attackPosition.x, attackPosition.y, 0);
        normalAttackIndicator.SetActive(true);

        // Wait for indicator time
        yield return new WaitForSeconds(indicatorDisplayTime);

        // Attack animation
        if (animator) animator.SetTrigger("Attack");
        else Debug.Log("Boss performs normal attack!");

        // Damage player if in attack area
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPosition, 1.5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerHealth2D playerHealth = hitCollider.GetComponent<PlayerHealth2D>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(isEnraged ? attackDamage * enragedDamageMultiplier : attackDamage);
                }
            }
        }

        // Hide attack indicator
        normalAttackIndicator.SetActive(false);

        // Set cooldown
        lastAttackTime = Time.time;

        // Wait for attack to finish
        yield return new WaitForSeconds(1f / attackSpeed);

        isAttacking = false;
    }

    IEnumerator PerformLeapAttack()
    {
        isUsingSkill = true;
        rb.linearVelocity = Vector2.zero;

        // Face the player
        if ((player.position.x > transform.position.x && !isFacingRight) ||
            (player.position.x < transform.position.x && isFacingRight))
        {
            FlipSprite();
        }

        // Set animation
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        // Show leap attack indicator at player position
        Vector2 targetPosition = player.position;
        leapAttackIndicator.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
        leapAttackIndicator.SetActive(true);

        // Wait for indicator time
        yield return new WaitForSeconds(indicatorDisplayTime);

        // Leap attack animation
        if (animator) animator.SetTrigger("LeapAttack");
        else Debug.Log("Boss performs leap attack!");

        // Leap to player
        Vector2 startPos = transform.position;
        Vector2 jumpTarget = player.position;
        float jumpHeight = 5f;
        float jumpDuration = 1f;

        // Perform jump parabola
        float elapsed = 0;
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / jumpDuration;

            // Parabola movement
            float height = Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight;
            Vector2 currentPos = Vector2.Lerp(startPos, jumpTarget, normalizedTime);
            currentPos.y += height;

            transform.position = currentPos;
            yield return null;
        }

        // Land at target position
        transform.position = jumpTarget;

        // Damage player in area
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 2.5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerHealth2D playerHealth = hitCollider.GetComponent<PlayerHealth2D>();
                if (playerHealth != null)
                {
                    // Leap attack does more damage than normal attack
                    float leapDamage = attackDamage * 1.5f;
                    if (isEnraged) leapDamage *= enragedDamageMultiplier;
                    playerHealth.TakeDamage(leapDamage);
                }
            }
        }

        // Hide leap attack indicator
        leapAttackIndicator.SetActive(false);

        // Set cooldown
        lastSkillTime = Time.time;

        // Wait for attack to finish
        yield return new WaitForSeconds(1.5f);

        isUsingSkill = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Check if boss died
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Check if should enter enraged state
        if (!isEnraged && currentHealth / maxHealth <= enrageHealthThreshold)
        {
            EnterEnragedState();
        }
    }

    void EnterEnragedState()
    {
        isEnraged = true;

        // Apply enrage buffs
        attackDamage *= enragedDamageMultiplier;
        movementSpeed *= enragedSpeedMultiplier;
        attackSpeed *= enragedAttackSpeedMultiplier;

        // Visual feedback for enrage
        bossRenderer.color = enragedColor;

        // Animation or effect for enrage
        if (animator) animator.SetTrigger("Enrage");

        Debug.Log("Boss has entered enraged state!");
    }

    void Die()
    {
        // Death animation
        if (animator) animator.SetTrigger("Die");
        else Debug.Log("Boss has been defeated!");

        // Disable components
        rb.linearVelocity = Vector2.zero;
        enabled = false;

        // Could add particle effects, sound, etc.

        // Optionally destroy after delay
        Destroy(gameObject, 3f);
    }

    // Helper to visualize attack ranges in editor
    void OnDrawGizmosSelected()
    {
        // Normal attack range
        Gizmos.color = Color.red;
        Vector2 attackPosition;
        if (transform.localScale.x >= 0) // Facing right
        {
            attackPosition = (Vector2)transform.position + new Vector2(1.5f, 0);
        }
        else // Facing left
        {
            attackPosition = (Vector2)transform.position + new Vector2(-1.5f, 0);
        }
        Gizmos.DrawWireSphere(attackPosition, 1.5f);

        // Leap attack range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2.5f);
    }
}