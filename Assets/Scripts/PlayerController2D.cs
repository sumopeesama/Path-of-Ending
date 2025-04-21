using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterStats))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float dashDistance = 5f;
    public int dashStaminaCost = 15;
    public float dashCooldown = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Combat")]
    public Transform attackPoint;
    public float attackRange = 1.2f;
    public LayerMask enemyLayers;
    public int lightAttackStaminaCost = 5;
    public int heavyAttackStaminaCost = 15;

    private Rigidbody2D rb;
    private Animator animator;
    private CharacterStats stats;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool canDash = true;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Movement Input
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Handle flipping the sprite
        if (moveInput > 0 && !isFacingRight)
            FlipCharacter();
        else if (moveInput < 0 && isFacingRight)
            FlipCharacter();

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && stats.HasEnoughStamina(dashStaminaCost))
        {
            StartCoroutine(Dash());
        }

        // Light Attack
        if (Input.GetMouseButtonDown(0) && !isAttacking && stats.HasEnoughStamina(lightAttackStaminaCost))
        {
            StartCoroutine(LightAttack());
        }

        // Heavy Attack
        if (Input.GetMouseButtonDown(1) && !isAttacking && stats.HasEnoughStamina(heavyAttackStaminaCost))
        {
            StartCoroutine(HeavyAttack());
        }

        // Update animator
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        }

        // Handle horizontal movement
        if (!isAttacking)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    IEnumerator Dash()
    {
        // Use stamina
        stats.UseStamina(dashStaminaCost);

        // Set cooldown
        canDash = false;

        // Store original gravity
        float originalGravity = rb.gravityScale;

        // Temporarily disable gravity
        rb.gravityScale = 0;

        // Calculate dash direction (use move input or current facing direction)
        float dashDirection = Input.GetAxisRaw("Horizontal");
        if (dashDirection == 0)
            dashDirection = isFacingRight ? 1 : -1;

        // Apply dash velocity
        rb.linearVelocity = new Vector2(dashDirection * dashDistance, 0);

        // Show dash effect
        if (animator != null)
            animator.SetTrigger("Dash");

        // Brief dash duration
        yield return new WaitForSeconds(0.2f);

        // Restore gravity
        rb.gravityScale = originalGravity;

        // Cooldown period
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    IEnumerator LightAttack()
    {
        isAttacking = true;

        // Use stamina
        stats.UseStamina(lightAttackStaminaCost);

        // Play animation
        if (animator != null)
            animator.SetTrigger("LightAttack");

        // Brief startup delay
        yield return new WaitForSeconds(0.1f);

        // Apply damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Calculate damage including critical chance
            int damage = stats.CalculateDamage(false);
            bool isCritical = Random.value <= stats.criticalChance;

            // Apply damage to enemy
            BossController2D boss = enemy.GetComponent<BossController2D>();
            if (boss != null)
                boss.TakeDamage(damage);

            // Show hit effect
            // You could instantiate a hit effect prefab here
        }

        // Attack recovery time
        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    IEnumerator HeavyAttack()
    {
        isAttacking = true;

        // Use stamina
        stats.UseStamina(heavyAttackStaminaCost);

        // Play animation
        if (animator != null)
            animator.SetTrigger("HeavyAttack");

        // Longer startup delay for heavy attack
        yield return new WaitForSeconds(0.3f);

        // Apply damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange * 1.2f, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Calculate damage including critical chance (heavy attack has higher damage)
            int damage = stats.CalculateDamage(true);
            bool isCritical = Random.value <= stats.criticalChance;

            // Apply damage to enemy
            BossController2D boss = enemy.GetComponent<BossController2D>();
            if (boss != null)
                boss.TakeDamage(damage);

            // Show hit effect
            // You could instantiate a hit effect prefab here
        }

        // Longer recovery time for heavy attack
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        // Draw ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}