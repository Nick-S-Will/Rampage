using CustomizableControls.Attacks;
using CustomizableControls.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rampage.Player
{
    [RequireComponent(typeof(MoveController))]
    [RequireComponent(typeof(JumpController))]
    [RequireComponent(typeof(ClimbController))]
    [RequireComponent(typeof(WallSmashController))]
    public class PlayerController : MonoBehaviour
    {
        private MoveController moveController;
        private JumpController jumpController;
        private ClimbController climbController;
        private WallSmashController wallSmashController;
        private Vector2 moveInput;

        private void Awake()
        {
            moveController = GetComponent<MoveController>();
            jumpController = GetComponent<JumpController>();
            climbController = GetComponent<ClimbController>();
            wallSmashController = GetComponent<WallSmashController>();
        }

        private void FixedUpdate()
        {
            moveController.Move(moveInput);
            climbController.Climb(moveInput);
        }

        public void Move(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            jumpController.Jump();
            climbController.JumpOff();
        }

        public void Smash(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            wallSmashController.Smash();
        }
    }
}