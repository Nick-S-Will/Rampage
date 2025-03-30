using System;
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
        [Serializable]
        private class DamageablePoint
        {
            public Collider collider;
            public Renderer renderer;
        }

        [SerializeField] private DamageablePoint[] damageablePoints;
        [SerializeField] private Material damagedMaterial;
        [Header("Events")]
        [field: SerializeField] public UnityEvent Damaged { get; private set; }
        [field: SerializeField] public UnityEvent Collapsed { get; private set; }

        private new Rigidbody rigidbody;
        private new Collider collider;
        private readonly HashSet<DamageablePoint> intactPoints = new();

        protected virtual void Awake()
        {
            Assert.IsTrue(damageablePoints.Length > 0 && damageablePoints.All(point => point != null && point.collider != null && point.renderer != null));
            Assert.IsNotNull(damagedMaterial);

            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            foreach (DamageablePoint point in damageablePoints) intactPoints.Add(point);
        }

        public bool TakeDamage(Vector3 position)
        {
            DamageablePoint point = intactPoints.FirstOrDefault(p => p.collider.bounds.Contains(position));
            if (point == null) return false;

            _ = intactPoints.Remove(point);
            point.renderer.material = damagedMaterial;

            Damaged.Invoke();

            if (intactPoints.Count == 0) Collapse();

            return true;
        }

        private void Collapse()
        {
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            collider.enabled = false;

            Destroy(gameObject, 5f);

            Collapsed.Invoke();
        }
    }
}