using CustomizableControls.Attacks.Shooting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rampage.Enemies
{
    [SelectionBase]
    public class GunEnemy : MonoBehaviour
    {
        public Transform Target { get; set; }
        public bool TargetIsInLinearRange => Vector3.Distance(gunController.transform.position, Target.position) <= gunController.BulletPrefab.MaxDistance;
        public bool TargetIsInAngularRange => Vector3.Angle(transform.forward, Target.position - gunController.transform.position) <= viewConeHalfAngle;

        [SerializeField] private GunController gunController;
        [Header("Attributes")]
        [SerializeField][Range(0f, 180f)] private float viewConeHalfAngle = 60f;
#if DEBUG
        [Header("Debug")]
        [SerializeField][Min(3f)] private int coneResolution = 12;
#endif

        protected virtual void Awake()
        {
            Assert.IsNotNull(gunController);
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
    }
}