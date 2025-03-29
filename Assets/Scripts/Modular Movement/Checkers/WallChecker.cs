using UnityEngine;

namespace ModularMovement.Checkers
{
    public class WallChecker : MonoBehaviour
    {
        public Vector3 WallCheckPosition => wallCheckPositionOverride != null ? wallCheckPositionOverride.position : transform.position;

        [SerializeField] private Transform wallCheckPositionOverride;
        [Header("Physics Checks")]
        [SerializeField][Min(1e-5f)] private float maxGrabDistance = .1f;
        [SerializeField][Min(1e-5f)] private float maxWallDistance = 1f;
        [SerializeField] private LayerMask wallMask = 1;

#if DEBUG
        private Vector3 lastCheckDirection = Vector3.forward;
        private bool lastCheckResult = false;
#endif

        protected virtual void Awake()
        {
            CheckWall(transform.forward);
        }

        public bool CheckWall(Vector3 direction)
        {
            Ray ray = new(WallCheckPosition, direction);
            bool hit = Physics.SphereCast(ray, maxGrabDistance, out RaycastHit _, maxWallDistance, wallMask, QueryTriggerInteraction.Ignore);

#if DEBUG
            lastCheckDirection = direction;
            lastCheckResult = hit;
#endif

            return hit;
        }

        protected virtual void OnDrawGizmosSelected()
        {
#if DEBUG
            Gizmos.color = Application.isPlaying ? (lastCheckResult ? Color.green : Color.red) : Color.white;
            Gizmos.DrawWireSphere(WallCheckPosition, maxGrabDistance);
            Gizmos.DrawWireSphere(WallCheckPosition + maxWallDistance * lastCheckDirection, maxGrabDistance);
#endif
        }
    }
}