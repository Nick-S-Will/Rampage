using Rampage.Enemies;
using Rampage.Terrain;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Rampage.Score
{
    public class ScoreHandler : MonoBehaviour
    {
        public UnityEvent ScoreChanged => scoreChanged;
        public int Score
        {
            get => score;
            set
            {
                if (score == value) return;

                score = value;

                scoreChanged.Invoke();
            }
        }

        [SerializeField] private BuildingSpawner buildingSpawner;
        [Header("Attributes")]
        [SerializeField][Min(0f)] private int windowSmashScore = 50;
        [SerializeField][Min(0f)] private int killScore = 100;
        [Header("Events")]
        [SerializeField] private UnityEvent scoreChanged;

        private readonly List<Building> buildings = new();
        private readonly List<GunEnemy> enemies = new();
        private int score;

        protected virtual void Awake()
        {
            Assert.IsNotNull(buildingSpawner);

            buildingSpawner.Spawned.AddListener(OnBuildingSpawned);
        }

        private void OnDestroy()
        {
            if (buildingSpawner) buildingSpawner.Spawned.RemoveListener(OnBuildingSpawned);

            foreach (Building building in buildings)
            {
                if (building)
                {
                    building.Damaged.RemoveListener(AddWindowSmashScore);
                    if (building.EnemySpawner) building.EnemySpawner.Spawned.RemoveListener(OnEnemySpawned);
                }
            }

            foreach (GunEnemy enemy in enemies)
            {
                if (enemy) enemy.Died.RemoveListener(AddKillScore);
            }
        }

        private void OnBuildingSpawned(Building building)
        {
            building.Damaged.AddListener(AddWindowSmashScore);
            building.EnemySpawner.Spawned.AddListener(OnEnemySpawned);
            buildings.Add(building);
        }

        private void AddWindowSmashScore() => Score += windowSmashScore;

        private void OnEnemySpawned(GunEnemy enemy)
        {
            enemy.Died.AddListener(AddKillScore);
            enemies.Add(enemy);
        }

        private void AddKillScore() => Score += killScore;
    }
}