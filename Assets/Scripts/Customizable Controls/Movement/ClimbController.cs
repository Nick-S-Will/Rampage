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
        public RaycastHit? WallHitInfo
        {
            get => wallHitInfo; 
            private set
            {
                bool isSameClimbingState = wallHitInfo.HasValue == value.HasValue;
                wallHitInfo = value;

                if (isSameClimbingState) return;

                rigidbody.useGravity = !IsClimbing;
                if (IsClimbing) rigidbody.linearVelocity = Vector3.zero;

                if (IsClimbing) ClimbStarted.Invoke();
                else ClimbStopped.Invoke();
            }
        }
        public Quaternion Rotation => Quaternion.LookRotation(MoveForward, transform.up);
        public Vector3 MoveForward
        {
            get
            {
                if (IsClimbing) return -WallHitInfo.Value.normal;

                bool isMoving = rigidbody.linearVelocity.magnitude > moveSpeedThreshold;
                return isMoving ? Vector3.ProjectOnPlane(rigidbody.linearVelocity, transform.up).normalized : transform.forward;
            }
        }
        public bool CanClimb => !groundChecker.IsGrounded;
        public bool IsClimbing => WallHitInfo.HasValue;

        [Header("Attributes")]
        [SerializeField][Min(0f)] private float moveSpeedThreshold = 1e-3f;
        [SerializeField][Min(0f)] private float climbSpeedDamping = 2f;
        [SerializeField][Min(1e-5f)] private float climbForce = 500, strafeForce = 750f, jumpOffForce = 250f;
        [Header("Events")]
        [field: SerializeField] public UnityEvent ClimbStarted { get; private set; }
        [field: SerializeField] public UnityEvent ClimbStopped { get; private set; }

        private new Rigidbody rigidbody;
        private GroundChecker groundChecker;
        private WallChecker wallChecker;
        private RaycastHit? wallHitInfo;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            groundChecker = GetComponent<GroundChecker>();
            wallChecker = GetComponent<WallChecker>();
        }

        protected virtual void FixedUpdate()
        {
            bool hitWall = wallChecker.CheckWall(MoveForward, out RaycastHit wallHitInfo) && CanClimb;
            WallHitInfo = hitWall ? wallHitInfo : null;

            DampenClimbVelocity();
        }

        private void DampenClimbVelocity()
        {
            if (!IsClimbing) return;

            float climbSpeedThreshold = climbSpeedDamping * Time.fixedDeltaTime;
            if (rigidbody.linearVelocity.magnitude < climbSpeedThreshold) return;

            rigidbody.AddForce(-climbSpeedDamping * rigidbody.linearVelocity.normalized, ForceMode.Acceleration);
        }

        public virtual void Climb(Vector2 input)
        {
            if (input == Vector2.zero || !IsClimbing) return;

            Vector3 localClimbForce = new(strafeForce * input.x, climbForce * input.y);
            Vector3 globalClimbForce = Rotation * localClimbForce;

            rigidbody.AddForce(globalClimbForce);
        }

        public virtual void JumpOff()
        {
            if (!IsClimbing) return;

            Vector3 localJumpForce = jumpOffForce * Vector3.back;
            Vector3 globalJumpForce = Rotation * localJumpForce;

            rigidbody.AddForce(globalJumpForce, ForceMode.Impulse);
        }
    }
}