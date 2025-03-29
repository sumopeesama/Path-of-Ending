using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    public Vector2 movementInput;
    public Vector2 cameraInput;
    public bool enableLogger = true;
    public void OnMovement(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
        if (enableLogger) Debug.LogFormat("Movement {0},{1}", movementInput.x, movementInput.y);
    }
    public void OnLook(InputValue inputValue)
    {
        cameraInput = inputValue.Get<Vector2>();
        if (enableLogger) Debug.LogFormat("Look {0},{1}", cameraInput.x, cameraInput.y);
    }
}