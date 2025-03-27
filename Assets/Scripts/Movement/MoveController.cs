using UnityEngine;
using UnityEngine.Events;

namespace Rampage.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GroundChecker))]
    public class MoveController : MonoBehaviour
    {
        public UnityEvent MoveStarted => moveStarted;
        public UnityEvent MoveStopped => moveStopped;
        public Quaternion Rotation
        {
            get
            {
                Quaternion baseRotation = inputRotationOverride != null ? inputRotationOverride.rotation : transform.rotation;
                Vector3 projectedForward = Vector3.ProjectOnPlane(baseRotation * Vector3.forward, transform.up).normalized;
                if (projectedForward == Vector3.zero) return transform.rotation;

                return Quaternion.LookRotation(projectedForward, transform.up);
            }
        }
        public Vector3 MoveVelocity => Vector3.ProjectOnPlane(rigidbody.linearVelocity, transform.up);
        public float MoveSpeed => MoveVelocity.magnitude;
        public bool IsMoving
        {
            get => isMoving;
            set
            {
                if (isMoving == value) return;

                isMoving = value;

                if (isMoving) moveStarted.Invoke();
                else moveStopped.Invoke();
            }
        }

        [SerializeField] private Transform inputRotationOverride;
        [Header("Attributes")]
        [SerializeField][Min(1e-5f)] private float moveForce = 1000f;
        [SerializeField][Min(0f)] private float moveSpeedThreshold = 1e-3f;
        [Header("Events")]
        [SerializeField] private UnityEvent moveStarted;
        [SerializeField] private UnityEvent moveStopped;

        private new Rigidbody rigidbody;
        private GroundChecker groundChecker;
        private bool isMoving;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            groundChecker = GetComponent<GroundChecker>();
        }

        public virtual void Move(Vector2 moveInput)
        {
            if (moveInput == Vector2.zero || !groundChecker.IsGrounded)
            {
                if (isMoving && MoveSpeed <= moveSpeedThreshold) IsMoving = false;

                return;
            }
            
            Vector3 localInputDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            Vector3 globalInputDirection = Rotation * localInputDirection;
            rigidbody.AddForce(moveForce * globalInputDirection);

            if (!isMoving && MoveSpeed > moveSpeedThreshold) IsMoving = true;
        }
    }
}