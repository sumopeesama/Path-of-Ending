using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    public float dodgeDistance = 5f; // Distance of the dodge
    public float dodgeCooldown = 1f; // Cooldown between dodges
    public int dodgeStaminaCost = 10; // Stamina cost per dodge

    private CharacterStats stats;
    private Rigidbody rb;
    private bool canDodge = true;

    private void Start()
    {
        stats = GetComponent<CharacterStats>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDodge && stats.currentStamina >= dodgeStaminaCost)
        {
            Dodge();
        }
    }

    private void Dodge()
    {
        stats.UseStamina(dodgeStaminaCost);
        canDodge = false;

        // Apply force in the player's forward direction
        Vector3 dodgeDirection = transform.forward * dodgeDistance;
        rb.AddForce(dodgeDirection, ForceMode.Impulse);

        // Start cooldown
        Invoke(nameof(ResetDodge), dodgeCooldown);
    }

    private void ResetDodge()
    {
        canDodge = true;
    }
}
