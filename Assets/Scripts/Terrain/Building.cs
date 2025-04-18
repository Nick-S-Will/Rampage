using CustomizableControls;
using Rampage.Enemies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Rampage.Terrain
{
    [RequireComponent(typeof(EnemySpawner))]
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

        public UnityEvent Damaged => damaged;
        public UnityEvent Collapsed => collapsed;
        public UnityEvent Destroyed => destroyed;
        public EnemySpawner EnemySpawner => enemySpawner;
        public bool IsCollapsing { get; private set; }

        [SerializeField] private Material damagedMaterial;
        [Header("Attributes")]
        [SerializeField][Min(0f)] private float collapseTime = 2f;
        [Header("Events")]
        [SerializeField] private UnityEvent damaged;
        [SerializeField] private UnityEvent collapsed, destroyed;

        private EnemySpawner enemySpawner;
        private Collider[] colliders;
        private readonly HashSet<DamageablePoint> intactPoints = new();

        protected virtual void Awake()
        {
            Assert.IsNotNull(damagedMaterial);
            Assert.IsTrue(GetComponentsInChildren<MeshRenderer>().Length > 0);

            damaged.AddListener(Collapse);

            enemySpawner = GetComponent<EnemySpawner>();
            colliders = GetComponents<Collider>();
            CollectDamageablePoints();
        }

        protected virtual void OnDestroy()
        {
            destroyed.Invoke();
        }

        private void CollectDamageablePoints()
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                if (!collider.TryGetComponent(out Renderer renderer)) continue;

                _ = intactPoints.Add(new DamageablePoint(collider, renderer));
            }
        }

        public virtual bool CanTakeDamage(IDamageDealer damageDealer, Vector3 position)
        {
            if (!isActiveAndEnabled || damageDealer.DamageSource.transform.IsChildOf(transform)) return false;

            bool positionInEnemy = enemySpawner.Enemies.Any(enemy => enemy.CanTakeDamage(damageDealer, position));
            bool positionInWall = intactPoints.Any(point => point.collider.bounds.Contains(position));

            return positionInEnemy || positionInWall;
        }

        public virtual bool TakeDamage(IDamageDealer damageDealer, Vector3 position)
        {
            if (!CanTakeDamage(damageDealer, position)) return false;

            GunEnemy enemy = enemySpawner.Enemies.FirstOrDefault(enemy => enemy.Renderer.bounds.Contains(position));
            if (enemy && enemy.TakeDamage(damageDealer, position)) return true;

            DamageablePoint point = intactPoints.First(point => point.collider.bounds.Contains(position));
            point.renderer.material = damagedMaterial;
            _ = !intactPoints.Remove(point);

            damaged.Invoke();

            return true;
        }

        private void Collapse()
        {
            if (intactPoints.Count > 0) return;

            foreach (Collider collider in colliders) collider.enabled = false;
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;

            IsCollapsing = true;
            Destroy(gameObject, collapseTime);

            collapsed.Invoke();
        }
    }
}