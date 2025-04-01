using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Rampage.Terrain
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Building : MonoBehaviour, IDamageable
    {
        private class DamageablePoint
        {
            public Collider collider;
            public Renderer renderer;

            public DamageablePoint(Collider collider, Renderer renderer)
            {
                this.collider = collider;
                this.renderer = renderer;
            }
        }

        [SerializeField] private Material damagedMaterial;
        [Header("Events")]
        [field: SerializeField] public UnityEvent Damaged { get; private set; }
        [field: SerializeField] public UnityEvent Collapsed { get; private set; }

        private new Rigidbody rigidbody;
        private Collider[] colliders;
        private readonly HashSet<DamageablePoint> intactPoints = new();

        protected virtual void Awake()
        {
            Assert.IsNotNull(damagedMaterial);

            Damaged.AddListener(Collapse);

            rigidbody = GetComponent<Rigidbody>();
            colliders = GetComponents<Collider>();
            CollectDamageablePoints();
        }

        private void CollectDamageablePoints()
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                if (!collider.TryGetComponent(out Renderer renderer)) continue;

                _ = intactPoints.Add(new DamageablePoint(collider, renderer));
            }
        }

        public bool CanTakeDamage(IDamageDealer damageDealer, Vector3 position)
        {
            if (damageDealer.DamageSource.transform.IsChildOf(transform)) return false;

            return intactPoints.Any(point => point.collider.bounds.Contains(position));
        }

        public bool TakeDamage(IDamageDealer damageDealer, Vector3 position)
        {
            if (!CanTakeDamage(damageDealer, position)) return false;

            DamageablePoint point = intactPoints.First(point => point.collider.bounds.Contains(position));
            point.renderer.material = damagedMaterial;
            _ = !intactPoints.Remove(point);

            Damaged.Invoke();

            return true;
        }

        private void Collapse()
        {
            if (intactPoints.Count > 0) return;

            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            foreach (Collider collider in colliders) collider.enabled = false;

            Destroy(gameObject, 5f);

            Collapsed.Invoke();
        }
    }
}