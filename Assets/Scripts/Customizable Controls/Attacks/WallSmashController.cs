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

        [ContextMenu(nameof(Smash))]
        public void Smash()
        {
            if (!climbController.IsClimbing) return;

            _ = ((IDamageDealer)this).DealDamage(climbController.WallHitInfo.Value.collider, climbController.WallHitInfo.Value.point);
        }
    }
}