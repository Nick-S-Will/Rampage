using UnityEngine;

namespace CustomizableControls.Checkers
{
    public class GroundChecker : MonoBehaviour
    {
        public Vector3 GroundCheckPosition => groundCheckPositionOverride != null ? groundCheckPositionOverride.position : transform.position;
        public bool IsGrounded
        {
            get
            {
                CheckGround();

                return isGrounded;
            }
        }

        [SerializeField] private Transform groundCheckPositionOverride;
        [Header("Physics Checks")]
        [SerializeField][Min(1e-5f)] private float maxGroundDistance = .1f;
        [SerializeField] private LayerMask groundMask = 1;

        private readonly Collider[] groundColliders = new Collider[1];
        private float lastGroundCheckTime;
        private bool isGrounded;

        protected virtual void Awake()
        {
            CheckGround();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(GroundCheckPosition, maxGroundDistance);
        }

        private void CheckGround()
        {
            if (Time.time < lastGroundCheckTime + Time.fixedDeltaTime) return;

            lastGroundCheckTime = Time.time;
            isGrounded = Physics.OverlapSphereNonAlloc(GroundCheckPosition, maxGroundDistance, groundColliders, groundMask, QueryTriggerInteraction.Ignore) > 0;
        }
    }
}