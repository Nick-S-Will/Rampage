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
        [SerializeField][Min(1f)] private int spawnCount = 1;

        private readonly List<Building> buildings = new();
        private int nextSpawnPointIndex;

        protected virtual void Awake()
        {
            Assert.IsNotNull(buildingPrefab);
            Assert.IsTrue(spawnPoints.Length > 0 && spawnPoints.All(spawnPoint => spawnPoint));

            SpawnBuildings();
        }

        private void OnValidate()
        {
            if (spawnPoints.Length > 0 && spawnCount > spawnPoints.Length)
            {
                Debug.LogWarning($"{nameof(spawnCount)} can't exceed the number of {nameof(spawnPoints)}");
                spawnCount = spawnPoints.Length;
            }
        }

        private void SpawnBuildings()
        {
            if (buildings.Count(building => building) > 1) return;

            buildings.Clear();

            Transform[] shuffledSpawnPoints = spawnPoints.Shuffle().ToArray();
            for (int i = 0; i < spawnCount; i++)
            {
                Transform spawnPoint = shuffledSpawnPoints[i];
                Building building = Instantiate(buildingPrefab, spawnPoint.position, spawnPoint.rotation, transform);
                building.Collapsed.AddListener(SpawnBuildings);

                buildings.Add(building);
            }
        }
    }
}