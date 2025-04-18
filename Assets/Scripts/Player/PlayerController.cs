using CustomizableControls.Attacks;
using CustomizableControls.Movement;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Rampage.Player
{
    [RequireComponent(typeof(MoveController))]
    [RequireComponent(typeof(JumpController))]
    [RequireComponent(typeof(ClimbController))]
    [RequireComponent(typeof(WallSmashController))]
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        private const string MoveSpeedFloatName = "Move Speed", OnGroundBoolName = "On Ground";

        [SerializeField] private Animator animator;

        private MoveController moveController;
        private JumpController jumpController;
        private ClimbController climbController;
        private WallSmashController wallSmashController;
        private Vector2 moveInput;

        private void Awake()
        {
            Assert.IsNotNull(animator);
            Assert.IsTrue(animator.parameters.Any(parameter => parameter.name == MoveSpeedFloatName));
            Assert.IsTrue(animator.parameters.Any(parameter => parameter.name == OnGroundBoolName));

            moveController = GetComponent<MoveController>();
            jumpController = GetComponent<JumpController>();
            climbController = GetComponent<ClimbController>();
            wallSmashController = GetComponent<WallSmashController>();
        }

        private void Update()
        {
            UpdateAnimator();
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

        private void UpdateAnimator()
        {
            if (climbController.IsMoving) animator.transform.forward = climbController.MoveForward;
            animator.SetFloat(MoveSpeedFloatName, moveController.MoveSpeed);
            animator.SetBool(OnGroundBoolName, moveController.IsGrounded);
        }
    }
}