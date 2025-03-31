using CustomizableControls.Movement;
using UnityEngine;

namespace CustomizableControls.Attacks
{
    [RequireComponent(typeof(ClimbController))]
    public class WallSmashController : MonoBehaviour
    {
        private ClimbController climbController;

        protected virtual void Awake()
        {
            climbController = GetComponent<ClimbController>();
        }

        public void Smash()
        {
            if (!climbController.IsClimbing || !climbController.WallHitInfo.Value.collider.TryGetComponent(out IDamageable damageable)) return;

            damageable.TakeDamage(climbController.WallHitInfo.Value.point);
        }
    }
}