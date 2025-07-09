using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Loot;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Level
{
    #region Classes

    public enum MapGenerationAlgorithmId
    {
        None,
    }

    public enum DifficultyGrowthType
    {
        Linear,
        Quadratic,
        Exponential,
    }

    public enum ZombieSpawnPosId
    {
        Pos_Random,
        Pos_1,
        Pos_2,
        Pos_3,
        Pos_4,
        Pos_5,
        Pos_6,
        Pos_7,
        Pos_8,
        Pos_9,
        Pos_10,
    }

    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
    }

    [Serializable]
    public class ZombieSpawnConfig
    {
        public ZombieId Zombie;
        public ZombieSpawnPosId SpawnPos;
        public float Weight;
        public float Value;
        public Vector2Int ActiveWaves;
    }

    #endregion

    [CreateAssetMenu(fileName = "LevelDefinition_", menuName = "PVZR/LevelDefinition", order = 2)]
    public class LevelDefinition : ScriptableObject
    {
        public LevelId LevelId;

        [Header("MapInfo")] public Vector2Int MapSize;
        public Vector2 InitialPlayerPos;
        public GameObject LevelPrefab;
        public MapGenerationAlgorithmId MapGenerationAlgorithmId = MapGenerationAlgorithmId.None;
        public List<SerializableKeyValuePair<Vector2Int, Vector2Int>> SunFallPositions;

        [Header("Difficulty")] public float BaseValue; // 第一波的强度
        public DifficultyGrowthType DifficultyGrowthType;

        [Header("Waves")] public int TotalWaveCount;
        public List<int> HugeWaves;
        public bool HasFinalBoss;
        public List<SerializableKeyValuePair<Vector2Int, float>> WaveDurations;
        public float HugeWaveDurationOffset;

        [Header("ZombieSpawn")] public List<SerializableKeyValuePair<ZombieSpawnPosId, Vector2>> PosDef;
        public List<ZombieSpawnConfig> ZombieSpawnConfigs;

        [Header("Loot")] public List<LootGenerateInfo> LootGenerateInfos;
        public float LootValue;
    }
}