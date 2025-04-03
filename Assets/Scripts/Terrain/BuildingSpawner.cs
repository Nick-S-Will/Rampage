using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
namespace Rampage.Terrain
{
    public class BuildingSpawner : MonoBehaviour
    {
        public UnityEvent<Building> Spawned => spawned;

        [SerializeField] private Transform target;
        [SerializeField] private Building buildingPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [Header("Attributes")]
        [SerializeField][Min(1f)] private int maxSpawnCount = 1;
        [Header("Events")]
        [SerializeField] private UnityEvent<Building> spawned;

        private readonly List<Building> buildings = new();

        protected virtual void Awake()
        {
            Assert.IsNotNull(buildingPrefab);
            Assert.IsTrue(spawnPoints.Length > 0 && spawnPoints.All(spawnPoint => spawnPoint));
            Assert.IsNotNull(target);
        }

        protected virtual void Start()
        {
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

        protected void SpawnBuildings()
        {
            if (buildings.Count(building => building) > 1) return;

            buildings.Clear();

            Transform[] shuffledSpawnPoints = spawnPoints.Where(spawnPoint => spawnPoint.gameObject.activeInHierarchy).Shuffle().ToArray();
            int spawnCount = Random.Range(1, maxSpawnCount + 1);
            for (int i = 0; i < spawnCount; i++)
            {
                Building building = SpawnBuilding(shuffledSpawnPoints[i]);
                buildings.Add(building);

                spawned.Invoke(building);
            }
        }

        protected virtual Building SpawnBuilding(Transform spawnPoint)
        {
            Building building = Instantiate(buildingPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            building.Collapsed.AddListener(SpawnBuildings);
            building.EnemySpawner.Target = target;

            return building;
        }
    }
}