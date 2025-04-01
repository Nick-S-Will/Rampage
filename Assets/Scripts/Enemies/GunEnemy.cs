using CustomizableControls.Attacks.Shooting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rampage.Enemies
{
    [SelectionBase]
    public class GunEnemy : MonoBehaviour
    {
        public Transform Target { get; set; }

        [SerializeField] private GunController gunController;
        [Header("Attributes")]
        [SerializeField][Range(0f, 180f)] private float maxPointAngle = 60f;

        protected virtual void Awake()
        {
            Assert.IsNotNull(gunController);

            Target = GameObject.FindGameObjectWithTag("Player").transform; // For debug purposes only
        }

        protected virtual void FixedUpdate()
        {
            ShootGunAtTarget();
        }

        private void ShootGunAtTarget()
        {
            if (Target == null || Vector3.Angle(Target.position - transform.position, transform.forward) > maxPointAngle)
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