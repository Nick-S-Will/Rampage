using ModularMovement.Checkers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace ModularMovement.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GroundChecker))]
    public class JumpController : MonoBehaviour
    {
        public float JumpHeightMultiplier => jumpHeightMultiplier;

        [Header("Attributes")]
        [SerializeField][Min(1e-5f)] private float jumpHeight = 2f;
        [Header("Events")]
        [field: SerializeField] public UnityEvent Jumped { get; private set; }

        private new Rigidbody rigidbody;
        private GroundChecker groundChecker;
        private float jumpHeightMultiplier = 1f;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            groundChecker = GetComponent<GroundChecker>();
        }

        public virtual void Jump()
        {
            if (!groundChecker.IsGrounded) return;

            float jumpMagnitude = Mathf.Sqrt(2f * jumpHeightMultiplier * jumpHeight * Physics.gravity.magnitude);
            Vector3 jumpDirection = rigidbody.rotation * Vector3.up;
            Vector3 jumpForce = jumpMagnitude * jumpDirection;
            rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);

            Jumped.Invoke();
        }

        public void MultiplyJumpHeight(float jumpHeightMultiplier)
        {
            Assert.IsTrue(jumpHeightMultiplier > 0f);

            this.jumpHeightMultiplier *= jumpHeightMultiplier;
        }
    }
}