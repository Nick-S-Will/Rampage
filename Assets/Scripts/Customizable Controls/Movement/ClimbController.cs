using CustomizableControls.Checkers;
using UnityEngine;
using UnityEngine.Events;

namespace CustomizableControls.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GroundChecker))]
    [RequireComponent(typeof(WallChecker))]
    public class ClimbController : MonoBehaviour
    {
        public RaycastHit WallHitInfo => wallHitInfo;
        public Quaternion Rotation => Quaternion.LookRotation(MoveForward, transform.up);
        public Vector3 MoveForward
        {
            get
            {
                if (isClimbing) return -wallHitInfo.normal;

                bool isMoving = rigidbody.linearVelocity.magnitude > moveSpeedThreshold;
                return isMoving ? Vector3.ProjectOnPlane(rigidbody.linearVelocity, transform.up).normalized : transform.forward;
            }
        }
        public bool CanClimb => !groundChecker.IsGrounded;
        public bool IsClimbing
        {
            get => isClimbing;
            private set
            {
                if (isClimbing == value) return;

                isClimbing = value;

                rigidbody.useGravity = !isClimbing;
                if (isClimbing) rigidbody.linearVelocity = Vector3.zero;

                if (isClimbing) ClimbStarted.Invoke();
                else ClimbStopped.Invoke();
            }
        }

        [Header("Attributes")]
        [SerializeField][Min(0f)] private float moveSpeedThreshold = 1e-3f;
        [SerializeField][Min(0f)] private float climbSpeedDamping = 1f;
        [SerializeField][Min(1e-5f)] private float climbForce = 500, strafeForce = 750f, jumpOffForce = 500f;
        [Header("Events")]
        [field: SerializeField] public UnityEvent ClimbStarted { get; private set; }
        [field: SerializeField] public UnityEvent ClimbStopped { get; private set; }

        private new Rigidbody rigidbody;
        private GroundChecker groundChecker;
        private WallChecker wallChecker;
        private RaycastHit wallHitInfo;
        private bool isClimbing;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            groundChecker = GetComponent<GroundChecker>();
            wallChecker = GetComponent<WallChecker>();
        }

        protected virtual void FixedUpdate()
        {
            IsClimbing = wallChecker.CheckWall(MoveForward, out wallHitInfo) && CanClimb;

            DampenClimbVelocity();
        }

        private void DampenClimbVelocity()
        {
            if (!isClimbing) return;

            float climbSpeedThreshold = climbSpeedDamping * Time.fixedDeltaTime;
            if (rigidbody.linearVelocity.magnitude < climbSpeedThreshold) return;

            rigidbody.AddForce(-climbSpeedDamping * rigidbody.linearVelocity.normalized, ForceMode.Acceleration);
        }

        public virtual void Climb(Vector2 input)
        {
            if (input == Vector2.zero || !isClimbing) return;

            Vector3 localClimbForce = new(strafeForce * input.x, climbForce * input.y);
            Vector3 globalClimbForce = Rotation * localClimbForce;

            rigidbody.AddForce(globalClimbForce);
        }

        public virtual void JumpOff()
        {
            if (!isClimbing) return;

            Vector3 localJumpForce = jumpOffForce * Vector3.back;
            Vector3 globalJumpForce = Rotation * localJumpForce;

            rigidbody.AddForce(globalJumpForce, ForceMode.Impulse);
        }
    }
}