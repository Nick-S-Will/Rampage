using ModularMovement.Checkers;
using UnityEngine;
using UnityEngine.Events;

namespace ModularMovement.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GroundChecker))]
    public class MoveController : MonoBehaviour
    {
        public Quaternion Rotation
        {
            get
            {
                Quaternion baseRotation = (inputRotationOverride != null ? inputRotationOverride : transform).rotation;
                Vector3 projectedForward = Vector3.ProjectOnPlane(baseRotation * Vector3.forward, transform.up).normalized;
                if (projectedForward == Vector3.zero) return transform.rotation;

                return Quaternion.LookRotation(projectedForward, transform.up);
            }
        }
        public Vector3 MoveVelocity => Vector3.ProjectOnPlane(rigidbody.linearVelocity, transform.up);
        public float MoveSpeed => MoveVelocity.magnitude;
        public bool CanMove => canMoveOffGround || groundChecker.IsGrounded;
        public bool IsMoving
        {
            get => isMoving;
            private set
            {
                if (isMoving == value) return;

                isMoving = value;

                if (isMoving) MoveStarted.Invoke();
                else MoveStopped.Invoke();
            }
        }

        [SerializeField] private Transform inputRotationOverride;
        [Header("Attributes")]
        [SerializeField][Min(0f)] private float moveSpeedThreshold = 1e-3f;
        [SerializeField][Min(1e-5f)] private float moveForce = 1000f;
        [SerializeField] private bool canMoveOffGround;
        [Header("Events")]
        [field: SerializeField] public UnityEvent MoveStarted { get; private set; }
        [field: SerializeField] public UnityEvent MoveStopped { get; private set; }

        private new Rigidbody rigidbody;
        private GroundChecker groundChecker;
        private bool isMoving;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            groundChecker = GetComponent<GroundChecker>();
        }

        protected virtual void FixedUpdate()
        {
            if (MoveSpeed <= moveSpeedThreshold) IsMoving = false;
        }

        public virtual void Move(Vector2 input)
        {
            if (input == Vector2.zero || !CanMove) return;
            
            Vector3 localInputDirection = new Vector3(input.x, 0f, input.y).normalized;
            Vector3 globalInputDirection = Rotation * localInputDirection;
            rigidbody.AddForce(moveForce * globalInputDirection);

            if (MoveSpeed > moveSpeedThreshold) IsMoving = true;
        }
    }
}