using CustomizableControls;
using CustomizableControls.Attacks.Shooting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Rampage.Enemies
{
    [SelectionBase]
    public class GunEnemy : MonoBehaviour, IDamageable
    {
        public Transform Target { get => target; set => target = value; }
        public Renderer Renderer => renderer;
        public UnityEvent Died => died;
        public bool TargetIsInLinearRange => Vector3.Distance(gunController.transform.position, Target.position) <= gunController.BulletPrefab.MaxDistance;
        public bool TargetIsInAngularRange => Vector3.Angle(transform.forward, Target.position - gunController.transform.position) <= viewConeHalfAngle;

        [SerializeField] private Transform target;
        [SerializeField] private GunController gunController;
        [SerializeField] private new Renderer renderer;
        [Header("Attributes")]
        [SerializeField][Range(0f, 180f)] private float viewConeHalfAngle = 60f;
        [Header("Events")]
        [SerializeField] private UnityEvent died;
#if DEBUG
        [Header("Debug")]
        [SerializeField][Min(3f)] private int coneResolution = 12;
#endif

        protected virtual void Awake()
        {
            Assert.IsNotNull(gunController);
            Assert.IsNotNull(renderer);

            gunController.User = this;
        }

        protected virtual void FixedUpdate()
        {
            ShootGunAtTarget();
        }

#if DEBUG
        protected virtual void OnDrawGizmosSelected()
        {
            GizmosExtensions.DrawCone(gunController.transform.position, transform.forward, transform.up, viewConeHalfAngle, gunController.BulletPrefab.MaxDistance, coneResolution);
        } 
#endif

        private void ShootGunAtTarget()
        {
            if (Target == null || !TargetIsInLinearRange || !TargetIsInAngularRange)
            {
                gunController.transform.localRotation = Quaternion.identity;
                gunController.ReleaseTrigger();
                return;
            }

            gunController.transform.LookAt(Target, transform.up);
            gunController.HoldTrigger();
        }

        public bool CanTakeDamage(IDamageDealer damageDealer, Vector3 position)
        {
            return isActiveAndEnabled && damageDealer.DamageSource is not GunEnemy && renderer.bounds.Contains(position);
        }

        public bool TakeDamage(IDamageDealer damageDealer, Vector3 position)
        {
            if (!CanTakeDamage(damageDealer, position)) return false;

            Destroy(gameObject);

            died.Invoke();

            return true;
        }
    }
}