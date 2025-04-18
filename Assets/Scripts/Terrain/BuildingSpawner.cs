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
            Assert.IsNotNull(target);
            Assert.IsNotNull(buildingPrefab);
            Assert.IsTrue(spawnPoints.Length > 0 && spawnPoints.All(spawnPoint => spawnPoint));
            Assert.IsTrue(maxSpawnCount <= spawnPoints.Length);
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
            Transform[] shuffledSpawnPoints = spawnPoints.Where(spawnPoint => spawnPoint.gameObject.activeInHierarchy).Shuffle().ToArray();
            if (shuffledSpawnPoints.Length == 0) return;

            int spawnCount = Random.Range(1, Mathf.Min(maxSpawnCount, shuffledSpawnPoints.Length) + 1);
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
            building.Destroyed.AddListener(OnBuildingDestroyed);
            building.EnemySpawner.Target = target;

            return building;
        }

        private void OnBuildingDestroyed()
        {
            if (buildings.Count(building => building && !building.IsCollapsing) > 0) return;

            buildings.Clear();
            SpawnBuildings();
        }
    }
}