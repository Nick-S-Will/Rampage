using ModularMovement.Checkers;
using ModularMovement.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rampage.Player
{
    [RequireComponent(typeof(MoveController))]
    [RequireComponent(typeof(JumpController))]
    [RequireComponent(typeof(ClimbController))]
    [RequireComponent(typeof(WallChecker))]
    public class PlayerController : MonoBehaviour
    {
        private MoveController moveController;
        private JumpController jumpController;
        private ClimbController climbController;
        private WallChecker wallChecker;
        private Vector2 moveInput;

        private void Awake()
        {
            moveController = GetComponent<MoveController>();
            jumpController = GetComponent<JumpController>();
            climbController = GetComponent<ClimbController>();
            wallChecker = GetComponent<WallChecker>();
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
        }

        public void Smash(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            Smash();
        }

        private void Smash()
        {
            if (wallChecker.Wall == null || !wallChecker.Wall.TryGetComponent(out IDamageable damageable)) return;

            damageable.TakeDamage(wallChecker.WallPosition.Value);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && wallChecker.WallPosition.HasValue)
            {
                Gizmos.DrawWireSphere(wallChecker.WallPosition.Value, .1f);
            }
        }
    }
}