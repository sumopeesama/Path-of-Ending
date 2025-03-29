using UnityEngine;
using UnityEngine.InputSystem;
namespace Pisit
{
    public class CharcterInput : MonoBehaviour
    {
        [SerializeField]
        private Vector2 moveInput;

        [SerializeField]
        private Vector2 lookInput;

        // Interface
        public Vector2 MoveInput => moveInput;
        // Interface
        public Vector2 LookInput => lookInput;

        private void OnMove(InputValue inputValue)
        {
            moveInput = inputValue.Get<Vector2>();
        }

        private void OnLook(InputValue inputValue)
        {
            lookInput = inputValue.Get<Vector2>();
        }
    }
}
