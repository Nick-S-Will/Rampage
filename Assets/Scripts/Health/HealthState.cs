using CustomizableControls;
using UnityEngine;
using UnityEngine.Events;

namespace Rampage.Health
{
    public class HealthState : MonoBehaviour, IDamageable
    {
        public int MaxHealth => maxHealth;
        public UnityEvent HealthChanged => healthChanged;
        public UnityEvent Died => died;
        public int Health
        {
            get => health;
            set
            {
                if (health == value) return;

                health = value;

                healthChanged.Invoke();
                if (health == 0) died.Invoke();
            }
        }

        [Header("Attributes")]
        [SerializeField] private int maxHealth = 100;
        [Header("Events")]
        [SerializeField] private UnityEvent healthChanged;
        [SerializeField] private UnityEvent died;

        private int health;

        protected virtual void Awake()
        {
            Health = maxHealth;
        }

        public bool CanTakeDamage(IDamageDealer damageDealer, Vector3 position)
        {
            return isActiveAndEnabled && damageDealer.DamageSource.gameObject != gameObject;
        }

        public bool TakeDamage(IDamageDealer damageDealer, Vector3 position)
        {
            if (!CanTakeDamage(damageDealer, position)) return false;

            Health = Mathf.Max(0, health - damageDealer.Damage);

            return true;
        }
    }
}