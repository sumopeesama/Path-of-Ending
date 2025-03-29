using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 20; // Damage per hit
    public float attackRange = 2f; // Range of the attack
    public float attackCooldown = 0.5f; // Cooldown between attacks

    private Animator animator;
    private bool canAttack = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) // Left Mouse Click to attack
        {
            Attack();
        }
    }

    private void Attack()
    {
        canAttack = false;

        // Play attack animation (Make sure you have an "Attack" animation)
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Check for enemies in range
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {
            Boss1 enemy = hit.collider.GetComponent<Boss1>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

        // Attack cooldown
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}
