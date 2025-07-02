using UnityEngine;

namespace TPL.PVZR.Classes.ZombieSpawner
{
    public class ZombieSpawnInfo
    {
        public ZombieId ZombieId { get; }
        public Vector2 SpawnPosition { get; }
        public float Weight { get; }
        public float Value { get; }

        public ZombieSpawnInfo(ZombieId zombieId, Vector2 spawnPosition, float weight, float value)
        {
            ZombieId = zombieId;
            SpawnPosition = spawnPosition;
            Weight = weight;
            Value = value;
        }
    }
}