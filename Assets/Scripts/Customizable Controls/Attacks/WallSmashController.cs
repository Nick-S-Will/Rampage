using CustomizableControls.Movement;
using UnityEngine;
using UnityEngine.Events;

namespace CustomizableControls.Attacks
{
    [RequireComponent(typeof(ClimbController))]
    public class WallSmashController : MonoBehaviour, IDamageDealer
    {
        public Component DamageSource => this;
        public int Damage => damage;
        public UnityEvent Smashed => smashed;

        [SerializeField][Min(0f)] private float smashInterval = 1f;
        [SerializeField][Min(1f)] private int damage = 10;
        [Header("Events")]
        [SerializeField] private UnityEvent smashed;

        private ClimbController climbController;
        private float lastSmashTime;

        protected virtual void Awake()
        {
            climbController = GetComponent<ClimbController>();
            lastSmashTime = -smashInterval;
        }

        [ContextMenu(nameof(Smash))]
        public void Smash()
        {
            if (!climbController.IsClimbing || Time.time < lastSmashTime + smashInterval) return;

            _ = ((IDamageDealer)this).DealDamage(climbController.WallHitInfo.Value.collider, climbController.WallHitInfo.Value.point);
            lastSmashTime = Time.time;

            smashed.Invoke();
        }
    }
}