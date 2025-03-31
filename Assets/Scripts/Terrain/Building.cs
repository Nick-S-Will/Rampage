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

        public bool TakeDamage(Vector3 position)
        {
            DamageablePoint point = intactPoints.FirstOrDefault(p => p.collider.bounds.Contains(position));
            if (point == null || !intactPoints.Remove(point)) return false;

            point.renderer.material = damagedMaterial;

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