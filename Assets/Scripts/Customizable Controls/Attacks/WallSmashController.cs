using CustomizableControls.Movement;
using UnityEngine;

namespace CustomizableControls.Attacks
{
    [RequireComponent(typeof(ClimbController))]
    public class WallSmashController : MonoBehaviour, IDamageDealer
    {
        public Component DamageSource => this;
        public int Damage => damage;

        [SerializeField][Min(1f)] private int damage = 10;

        private ClimbController climbController;

        protected virtual void Awake()
        {
            climbController = GetComponent<ClimbController>();
        }

        public void Smash()
        {
            if (!climbController.IsClimbing || !climbController.WallHitInfo.Value.collider.TryGetComponent(out IDamageable damageable)) return;

            damageable.TakeDamage(this, climbController.WallHitInfo.Value.point);
        }
    }
}