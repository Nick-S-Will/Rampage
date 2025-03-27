using UnityEngine;

namespace Rampage.Movement
{
    public class GroundChecker : MonoBehaviour
    {
        public Vector3 GroundCheckPosition => groundCheckPositionOverride != null ? groundCheckPositionOverride.position : transform.position;
        public bool IsGrounded => Physics.OverlapSphereNonAlloc(GroundCheckPosition, maxGroundDistance, groundColliders, groundMask, QueryTriggerInteraction.Ignore) > 0;

        [SerializeField] private Transform groundCheckPositionOverride;
        [Header("Physics Checks")]
        [SerializeField][Min(1e-5f)] private float maxGroundDistance = .1f;
        [SerializeField] private LayerMask groundMask = 1;

        private readonly Collider[] groundColliders = new Collider[1];

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(GroundCheckPosition, maxGroundDistance);
        }
    }
}