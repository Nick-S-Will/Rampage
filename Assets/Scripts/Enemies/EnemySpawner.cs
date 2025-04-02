using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rampage.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform Target { get => target; set => target = value; }
        public GunEnemy[] Enemies => enemies.ToArray();

        [SerializeField] private Transform target;
        [SerializeField] private GunEnemy gunEnemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [field: Header("Attributes")]
        [SerializeField][Min(1f)] private int maxEnemySpawnCount = 1;
        [SerializeField][Min(0f)] private float minSpawnDelay = 1f, maxSpawnDelay = 5f;

        private readonly List<GunEnemy> enemies = new();

        protected virtual void Awake()
        {
            Assert.IsNotNull(gunEnemyPrefab);
            Assert.IsTrue(spawnPoints.Length > 0 && spawnPoints.All(spawnPoint => spawnPoint));
        }

        protected virtual void Start()
        {
            SpawnEnemies();
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

        protected virtual void SpawnEnemies()
        {
            Transform[] shuffledSpawnPoints = spawnPoints.Where(spawnPoint => spawnPoint.gameObject.activeInHierarchy).Shuffle().ToArray();
            int spawnCount = Random.Range(1, maxEnemySpawnCount + 1);
            for (int i = 0; i < spawnCount; i++)
            {
                StartCoroutine(SpawnEnemy(shuffledSpawnPoints[i]));
            }
        }

        private IEnumerator SpawnEnemy(Transform spawnPoint)
        {
            GunEnemy gunEnemy = Instantiate(gunEnemyPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            gunEnemy.enabled = false;
            gunEnemy.Target = Target;
            gunEnemy.Died.AddListener(() => enemies.Remove(gunEnemy));
            enemies.Add(gunEnemy);

            Quaternion startRotation = Quaternion.AngleAxis(-120f, spawnPoint.right) * spawnPoint.rotation;
            gunEnemy.transform.rotation = startRotation;

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay), progress = 0f;
            yield return new WaitForSeconds(delay);

            while (progress <= 1f)
            {
                gunEnemy.transform.rotation = Quaternion.Lerp(startRotation, spawnPoint.rotation, progress);

                progress = delay > 0f ? progress + Time.deltaTime : 1f;

                yield return null;
            }

            gunEnemy.enabled = true;
        }
    }
}