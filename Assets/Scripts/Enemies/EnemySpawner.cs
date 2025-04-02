using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Rampage.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform Target { get => target; set => target = value; }
        public UnityEvent<GunEnemy> Spawned => spawned;
        public GunEnemy[] Enemies => enemies.ToArray();

        [SerializeField] private Transform target;
        [SerializeField] private GunEnemy enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [field: Header("Attributes")]
        [SerializeField][Min(1f)] private int maxEnemySpawnCount = 1;
        [SerializeField][Min(0f)] private float minSpawnDelay = 1f, maxSpawnDelay = 5f;
        [Header("Events")]
        [SerializeField] private UnityEvent<GunEnemy> spawned;

        private readonly List<GunEnemy> enemies = new();

        protected virtual void Awake()
        {
            Assert.IsNotNull(enemyPrefab);
            Assert.IsTrue(spawnPoints.Length > 0 && spawnPoints.All(spawnPoint => spawnPoint));
        }

        protected virtual void Start()
        {
            SpawnEnemies();
        }

        protected virtual void OnDestroy()
        {
            foreach (var enemy in enemies) Destroy(enemy.gameObject);
        }

        protected virtual void OnValidate()
        {
            if (spawnPoints != null)
            {
                int activeSpawnPoints = spawnPoints.Count(spawnPoint => spawnPoint.gameObject.activeInHierarchy);
                if (activeSpawnPoints > 0 && maxEnemySpawnCount > activeSpawnPoints)
                {
                    Debug.LogWarning($"{nameof(maxEnemySpawnCount)} can't exceed the number of active {nameof(spawnPoints)}");
                    maxEnemySpawnCount = activeSpawnPoints;
                }
            }

            if (maxSpawnDelay < minSpawnDelay)
            {
                maxSpawnDelay = minSpawnDelay;
            }
        }

        protected void SpawnEnemies()
        {
            Transform[] shuffledSpawnPoints = spawnPoints.Where(spawnPoint => spawnPoint.gameObject.activeInHierarchy).Shuffle().ToArray();
            int spawnCount = Random.Range(1, maxEnemySpawnCount + 1);
            for (int i = 0; i < spawnCount; i++)
            {
                GunEnemy enemy = SpawnEnemy(shuffledSpawnPoints[i]);
                enemies.Add(enemy);
                StartCoroutine(ActivateEnemyRoutine(enemy));

                spawned.Invoke(enemy);
            }
        }

        protected virtual GunEnemy SpawnEnemy(Transform spawnPoint)
        {
            GunEnemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            enemy.Target = Target;
            enemy.Died.AddListener(() => enemies.Remove(enemy));

            return enemy;
        }

        private IEnumerator ActivateEnemyRoutine(GunEnemy enemy)
        {
            enemy.enabled = false;

            Quaternion startRotation = Quaternion.AngleAxis(-120f, enemy.transform.right) * enemy.transform.rotation;
            Quaternion targetRotation = enemy.transform.rotation;
            enemy.transform.rotation = startRotation;

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            float progress = 0f;
            while (progress <= 1f)
            {
                enemy.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);

                progress += Time.deltaTime;

                yield return null;
            }

            enemy.enabled = true;
        }
    }
}