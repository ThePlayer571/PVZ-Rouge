using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.Classes.ZombieSpawner
{
    public class ZombieSpawnInfo : IGenerateInfo<ZombieSpawnInfo>
    {
        public ZombieId ZombieId { get; }
        public Vector2 SpawnPosition { get; }
        public float Weight { get; }
        public float Value { get; }
        public ZombieSpawnInfo Output => this;

        public ZombieSpawnInfo(ZombieId zombieId, Vector2 spawnPosition, float weight, float value)
        {
            ZombieId = zombieId;
            SpawnPosition = spawnPosition;
            Weight = weight;
            Value = value;
        }
    }
}