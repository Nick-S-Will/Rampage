using UnityEngine;
using UnityEngine.Assertions;

namespace CustomizableControls.Attacks.Shooting
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour, IDamageDealer
    {
        public Component DamageSource { get; private set; }
        public int Damage => damage;
        public float MaxDistance => maxDistance;

        [SerializeField][Min(1f)] private int damage = 10;
        [SerializeField][Min(1e-5f)] private float maxDistance = 25f;

        private new Rigidbody rigidbody;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            Assert.IsNotNull(DamageSource, $"{nameof(Projectile)} must be {nameof(Initialize)}d immediately.");
        }

        public void Initialize(Component bulletSource, float speed)
        {
            DamageSource = bulletSource;
            rigidbody.linearVelocity = speed * transform.forward;

            Destroy(gameObject, maxDistance / speed);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);

            if (!collision.collider.TryGetComponent(out IDamageable damageable)) return;

            damageable.TakeDamage(this, collision.GetContact(0).point);
        }
    }
}