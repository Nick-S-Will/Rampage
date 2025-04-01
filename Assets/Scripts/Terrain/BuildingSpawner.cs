using Rampage.Enemies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rampage.Terrain
{
    public class BuildingSpawner : MonoBehaviour
    {
        [SerializeField] private Building buildingPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Transform target;
        [Header("Attributes")]
        [SerializeField][Min(1f)] private int maxSpawnCount = 1;

        private readonly List<Building> buildings = new();

        protected virtual void Awake()
        {
            Assert.IsNotNull(buildingPrefab);
            Assert.IsTrue(spawnPoints.Length > 0 && spawnPoints.All(spawnPoint => spawnPoint));
            Assert.IsNotNull(target);

            SpawnBuildings();
        }

        protected virtual void OnValidate()
        {
            if (spawnPoints == null) return;

            int activeSpawnPoints = spawnPoints.Count(spawnPoint => spawnPoint.gameObject.activeInHierarchy);
            if (activeSpawnPoints > 0 && maxSpawnCount > activeSpawnPoints)
            {
                Debug.LogWarning($"{nameof(maxSpawnCount)} can't exceed the number of active {nameof(spawnPoints)}");
                maxSpawnCount = spawnPoints.Length;
            }
        }

        protected virtual void SpawnBuildings()
        {
            if (buildings.Count(building => building) > 1) return;

            buildings.Clear();

            Transform[] shuffledSpawnPoints = spawnPoints.Where(spawnPoint => spawnPoint.gameObject.activeInHierarchy).Shuffle().ToArray();
            int spawnCount = Random.Range(1, maxSpawnCount + 1);
            for (int i = 0; i < spawnCount; i++)
            {
                Transform spawnPoint = shuffledSpawnPoints[i];
                Building building = Instantiate(buildingPrefab, spawnPoint.position, spawnPoint.rotation, transform);
                building.Collapsed.AddListener(SpawnBuildings);
                if (building.TryGetComponent(out EnemySpawner enemySpawner)) enemySpawner.Target = target;

                buildings.Add(building);
            }
        }
    }
}