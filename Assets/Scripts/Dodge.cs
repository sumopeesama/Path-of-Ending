using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDodge : MonoBehaviour
{
    public float dodgeDistance = 5f;
    public float dodgeCooldown = 1f;
    public int dodgeStaminaCost = 10;

    private CharacterStats stats;
    private Rigidbody rb;
    private bool canDodge = true;

    private PlayerInput playerInput;
    private InputAction dodgeAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        dodgeAction = playerInput.actions["Dodge"]; // Make sure the action is named "Dodge" in the InputActions
    }

    private void Start()
    {
        stats = GetComponent<CharacterStats>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        dodgeAction.performed += OnDodge;
        dodgeAction.Enable();
    }

    private void OnDisable()
    {
        dodgeAction.performed -= OnDodge;
        dodgeAction.Disable();
    }

    private void OnDodge(InputAction.CallbackContext context)
    {
        if (canDodge && stats.currentStamina >= dodgeStaminaCost)
        {
            Dodge();
        }
    }

    private void Dodge()
    {
        stats.UseStamina(dodgeStaminaCost);
        canDodge = false;

        Vector3 dodgeDirection = transform.forward * dodgeDistance;
        rb.AddForce(dodgeDirection, ForceMode.Impulse);

        Invoke(nameof(ResetDodge), dodgeCooldown);
    }

    private void ResetDodge()
    {
        canDodge = true;
    }
}
