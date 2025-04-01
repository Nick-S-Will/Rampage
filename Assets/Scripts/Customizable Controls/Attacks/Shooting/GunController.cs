using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace CustomizableControls.Attacks.Shooting
{
    public class GunController : MonoBehaviour
    {
        public float RoundsPerSecond => roundsPerSecond;
        public float BulletSpeed => bulletSpeed;
        public float ReloadTime => reloadTime;
        public int ClipSize => clipSize;
        public bool HasInfiniteAmmo => hasInfiniteAmmo;
        public UnityEvent ReloadStarted => reloadStarted;
        public UnityEvent ReloadEnded => reloadEnded;
        public UnityEvent BulletShot => bulletShot;
        public int AmmoInClip => ammoInClip;
        public int SpareAmmo => spareAmmo;
        public bool IsHoldingTrigger => isHoldingTrigger;
        public bool IsReloading => reloading != null;
        public bool IsShooting => shooting != null;

        protected Projectile BulletPrefab => bulletPrefab;
        protected Transform BulletSpawnPoint => bulletSpawnPoint;
        protected virtual float ShotInterval => 1f / roundsPerSecond;

        [SerializeField] private Projectile bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;
        [Header("Attributes")]
        [SerializeField][Min(1e-5f)] private float roundsPerSecond = 8f;
        [SerializeField][Min(1e-5f)] private float bulletSpeed = 50f, reloadTime = 1f;
        [SerializeField][Min(1f)] private int clipSize = 32;
        [SerializeField][Min(0f)] private int startingAmmo = 64;
        [SerializeField] private bool isFullyAutomatic = true, hasInfiniteAmmo;
#if DEBUG
        [Header("Validation")]
        [SerializeField][Range(0f, 1f)] private float sizePercentageBuffer = .1f;
#endif
        [Header("Events")]
        [SerializeField] private UnityEvent reloadStarted;
        [SerializeField] private UnityEvent reloadEnded, bulletShot;

        private Coroutine reloading, shooting;
        private float lastShootTime;
        private int ammoInClip, spareAmmo;
        private bool isHoldingTrigger;

        protected virtual void Awake()
        {
            Assert.IsNotNull(bulletPrefab);
            Assert.IsNotNull(bulletSpawnPoint);

            lastShootTime = -ShotInterval;
            spareAmmo = startingAmmo;
            Reload(true);
        }

        protected virtual void FixedUpdate()
        {
            if (isHoldingTrigger && isFullyAutomatic) Shoot();
        }

#if DEBUG
        private void OnValidate()
        {
            if (bulletPrefab == null) return;

            Collider collider = bulletPrefab.GetComponent<Collider>();
            Vector3 size = collider.GetSize();
            float maxSize = Mathf.Max(size.x, size.y, size.z);
            if (bulletSpeed / roundsPerSecond < (1f + sizePercentageBuffer) * maxSize)
            {
                Debug.LogWarning($"{nameof(bulletSpeed)} / {nameof(roundsPerSecond)} must exceed the {nameof(bulletPrefab)}'s size or they will collide with each other");
                bulletSpeed = (1f + 2f * sizePercentageBuffer) * maxSize * roundsPerSecond;
            }
        }
#endif

        #region Controls
        public void Reload() => Reload(false);

        public void TapTrigger()
        {
            Shoot();
        }

        public void HoldTrigger()
        {
            TapTrigger();

            isHoldingTrigger = true;
        }

        public void ReleaseTrigger()
        {
            isHoldingTrigger = false;
        }
        #endregion

        #region Reload
        protected void Reload(bool isInstant)
        {
            if (IsReloading || spareAmmo <= 0 && !hasInfiniteAmmo) return;

            int requestedAmmoCount = clipSize - ammoInClip;
            if (requestedAmmoCount <= 0) return;

            reloading = StartCoroutine(ReloadRoutine(requestedAmmoCount, isInstant));
        }

        protected virtual IEnumerator ReloadRoutine(int requestedAmmoCount, bool isInstant)
        {
            reloadStarted.Invoke();

            if (!isInstant) yield return new WaitForSeconds(reloadTime);

            int loadCount = hasInfiniteAmmo ? requestedAmmoCount : Mathf.Min(requestedAmmoCount, spareAmmo);
            if (!hasInfiniteAmmo) spareAmmo -= loadCount;
            ammoInClip += loadCount;

            reloadEnded.Invoke();

            reloading = null;
        }
        #endregion

        #region Shooting
        protected virtual bool CanShoot() => !IsReloading && !IsShooting && ammoInClip > 0 && Time.time >= lastShootTime + ShotInterval;

        private void Shoot()
        {
            if (!CanShoot())
            {
                if (ammoInClip == 0) Reload();
                return;
            }

            shooting = StartCoroutine(ShootRoutine());
            lastShootTime = Time.time;
        }

        protected virtual IEnumerator ShootRoutine()
        {
            Projectile bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.Initialize(this, bulletSpeed);

            ammoInClip--;

            bulletShot.Invoke();

            shooting = null;

            yield break;
        }
        #endregion
    }
}