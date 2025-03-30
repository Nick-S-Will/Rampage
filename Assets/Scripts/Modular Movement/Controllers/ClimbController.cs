using ModularMovement.Checkers;
using UnityEngine;
using UnityEngine.Events;

namespace ModularMovement.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GroundChecker))]
    [RequireComponent(typeof(WallChecker))]
    public class ClimbController : MonoBehaviour
    {
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
        [SerializeField][Min(1e-5f)] private float climbSpeed = 2f;
        [SerializeField][Min(1e-5f)] private float strafeSpeed = .5f;
        [Header("Events")]
        [field: SerializeField] public UnityEvent ClimbStarted { get; private set; }
        [field: SerializeField] public UnityEvent ClimbStopped { get; private set; }

        private new Rigidbody rigidbody;
        private GroundChecker groundChecker;
        private WallChecker wallChecker;
        private Vector3 lastMoveDirection;
        private bool isClimbing;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            groundChecker = GetComponent<GroundChecker>();
            wallChecker = GetComponent<WallChecker>();
            lastMoveDirection = transform.forward;
        }

        protected virtual void FixedUpdate()
        {
            UpdateLastMoveDirection();
            IsClimbing = wallChecker.CheckWall(lastMoveDirection) && CanClimb;
        }

        private void UpdateLastMoveDirection()
        {
            if (isClimbing) return;

            Vector3 moveVelocity = Vector3.ProjectOnPlane(rigidbody.linearVelocity, transform.up);
            if (moveVelocity == Vector3.zero) return;

            lastMoveDirection = moveVelocity.normalized;
        }

        public virtual void Climb(Vector2 input)
        {
            if (input == Vector2.zero || !isClimbing) return;

            Quaternion rotation = Quaternion.LookRotation(lastMoveDirection, transform.up);

            bool ascending = input.y > 0;
            float localY = ascending ? input.y : 0f;
            float localZ = ascending ? 0f : input.y;
            Vector3 localInputDirection = new Vector3(input.x, localY, localZ).normalized;
            Vector3 localMoveDelta = Time.fixedDeltaTime * Vector3.Scale(new Vector3(strafeSpeed, climbSpeed, strafeSpeed), localInputDirection);

            Vector3 globalMoveDelta = rotation * localMoveDelta;

            rigidbody.MovePosition(rigidbody.position + globalMoveDelta);
        }
    }
}