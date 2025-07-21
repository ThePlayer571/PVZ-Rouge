using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Level
{
    #region Classes

    [Serializable]
    public enum MapGenerationAlgorithmId
    {
        None = 0,
    }

    [Serializable]
    public enum DifficultyGrowthType
    {
        Logistic = 0,
        Linear = 1,
        Quadratic = 2,
    }

    [Serializable]
    public enum ZombieSpawnPosId
    {
        NotSet = 0,
        Pos_1 = 1,
        Pos_2 = 2,
        Pos_3 = 3,
        Pos_4 = 4,
        Pos_5 = 5,
        Pos_6 = 6,
        Pos_7 = 7,
        Pos_8 = 8,
        Pos_9 = 9,
        Pos_10 = 10,
        Pos_11 = 11,
        Pos_12 = 12,
        Pos_13 = 13,
        Pos_14 = 14,
        Pos_15 = 15,
        Pos_16 = 16,
        PosGroup_1 = 101,
        PosGroup_2 = 102,
        PosGroup_3 = 103,
        PosGroup_4 = 104,
        PosGroup_5 = 105,
        PosGroup_6 = 106,
        PosGroup_7 = 107,
        PosGroup_8 = 108,
        PosGroup_9 = 109,
        PosGroup_10 = 110,
        PosGroup_11 = 111,
        PosGroup_12 = 112,
        PosGroup_13 = 113,
        PosGroup_14 = 114,
        PosGroup_15 = 115,
        PosGroup_16 = 116,
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
        public LevelDef LevelDef;

        [Header("MapInfo")] [Tooltip("关卡的大小（包括基岩）")]
        public Vector2Int MapSize;

        public Vector2 InitialPlayerPos;
        public GameObject LevelPrefab;

        [Tooltip("地图生成算法（暂未启用）")]
        public MapGenerationAlgorithmId MapGenerationAlgorithmId = MapGenerationAlgorithmId.None;

        [Tooltip("阳光生成的坐标（fill式）")] public List<SerializableKeyValuePair<Vector2Int, Vector2Int>> SunFallPositions;

        [Header("Difficulty")] [Tooltip("第一波的强度")]
        public float BaseValue;

        [Tooltip("等价定义：1. 最后一波的出怪量 2. 出怪的上限，不能超过此 3. 玩家阵型成型后10秒内能处理的Value总量\n"
                 + "参考值：plantValue * CellCount")]
        public float maxValue;

        [Tooltip("难度增长型")] public DifficultyGrowthType DifficultyGrowthType;

        [Header("Waves")] public int TotalWaveCount;

        [Tooltip("一大波僵尸的标识，出怪量比常规波次大（需要包含最后一波）")]
        public List<int> HugeWaves;

        [Tooltip("（暂未启用）最后一波会在所有僵尸消灭后开始，召唤boss")]
        public bool HasFinalBoss;

        [Tooltip("波次区间->该波召唤僵尸后的等待时间")] public List<SerializableKeyValuePair<Vector2Int, float>> WaveDurations;
        [Tooltip("大波次比常规波次的等待时间长，这是偏移量")] public float HugeWaveDurationOffset;

        [Header("ZombieSpawn")] public List<SerializableKeyValuePair<ZombieSpawnPosId, Vector2>> PosDef;
        public List<SerializableKeyValuePair<ZombieSpawnPosId, List<ZombieSpawnPosId>>> PosGroupDef;
        public List<ZombieSpawnConfig> ZombieSpawnConfigs;

        [Header("Loot")] public List<PlantId> BasicPlants;
        public List<PlantBookId> BasicPlantBooks;
        public List<LootGenerateInfo> SpecialLoots;
       [Tooltip("Loot的总价值（推荐：1.5 * 通关所需植物价值）")] public float LootValue;
    }
}