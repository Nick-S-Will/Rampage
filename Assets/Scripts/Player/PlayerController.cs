using Rampage.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rampage.Player
{
    [RequireComponent(typeof(MoveController))]
    [RequireComponent(typeof(JumpController))]
    public class PlayerController : MonoBehaviour
    {
        private MoveController moveController;
        private JumpController jumpController;
        private Vector2 moveInput;

        private void Awake()
        {
            moveController = GetComponent<MoveController>();
            jumpController = GetComponent<JumpController>();
        }

        private void FixedUpdate()
        {
            moveController.Move(moveInput);
        }

        public void Move(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            jumpController.Jump();
        }
    }
}