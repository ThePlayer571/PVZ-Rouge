using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.LootPool;
using TPL.PVZR.Tools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

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
        SpecialPos_1 = 201,
        SpecialPos_2 = 202,
        SpecialPos_3 = 203,
        SpecialPos_4 = 204,
        SpecialPos_5 = 205,
        SpecialPos_6 = 206,
        SpecialPos_7 = 207,
        SpecialPos_8 = 208,
        SpecialPos_9 = 209,
        SpecialPos_10 = 210,
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

        //
        [Header("MapInfo")] [Tooltip("关卡的大小（包括基岩）")]
        public Vector2Int MapSize;

        public Vector2 InitialPlayerPos;
        public AssetReference LevelPrefab;

        [Tooltip("地图生成算法（暂未启用）")]
        public MapGenerationAlgorithmId MapGenerationAlgorithmId = MapGenerationAlgorithmId.None;

        [Tooltip("阳光生成的坐标（fill式）")] public List<SerializableKeyValuePair<Vector2Int, Vector2Int>> SunFallPositions;

        [Header("Environment")] public DayPhaseType InitialDayPhase;

        public WeatherType InitialWeather;

        //
        [Header("Difficulty")] [Tooltip("第一波的强度")]
        public float BaseValue;

        [Tooltip("等价定义：1. 最后一波的出怪量 2. 出怪的上限，不能超过此 3. 玩家阵型成型后10秒内能处理的Value总量\n"
                 + "参考值：\n" +
                 " - Easy(完全不会被破阵): plantValue * (ValidCellCount - 2) / 4")]
        public float maxValue;

        public LevelValueDetail LevelValueDetail;

        [Tooltip("难度增长型")] public DifficultyGrowthType DifficultyGrowthType;

//
        [Header("Waves")] public int TotalWaveCount;

        [Tooltip("一大波僵尸的标识，出怪量比常规波次大（需要包含最后一波）")]
        public List<int> HugeWaves;

        [Tooltip("（暂未启用）最后一波会在所有僵尸消灭后开始，召唤boss")]
        public bool HasFinalBoss;

        [Tooltip("波次区间->该波召唤僵尸后的等待时间")] public List<SerializableKeyValuePair<Vector2Int, float>> WaveDurations;

        [Tooltip("大波次比常规波次的等待时间长，这是偏移量")] public float HugeWaveDurationOffset;

//
        [Header("ZombieSpawn")] public List<SerializableKeyValuePair<ZombieSpawnPosId, Vector2>> PosDef;
        public List<SerializableKeyValuePair<ZombieSpawnPosId, List<ZombieSpawnPosId>>> PosGroupDef;

        public List<ZombieSpawnConfig> ZombieSpawnConfigs;

//
        [Header("Award")] public AwardGenerateInfo AwardGenerateInfo;


        [Header("Others")] public List<InitialPlantConfig> InitialPlants;
        public int InitialSunpointOffset = 0;
    }

    [Serializable]
    public class LevelValueDetail
    {
        [Tooltip("本关推荐植物的DPS\n参考值：\n- 豌豆射手: 13.3")]
        public float RecommendedDPS = 13.3f;

        [Tooltip("本关可放置攻击植物的格子数 - 玩家利用这么多个格子就能通关")]
        public int ValidCellCount;

        [Tooltip("推荐值：\n- N0: 0.4")] public float MultiplierOfCell = 0.4f;
        [Tooltip("最后一波的可支配阳光（请使用参考值）")] public int ValidSunpointWhenFinalWave;

        [Tooltip("推荐值：\n- N0|白天: 0.4\n- N0|夜晚向日葵: 0.2")]
        public float MultiplierOfSunpoint = 0.4f;

        public float DPS2Value(float dps) => dps / 13.3f * 10f;
        public float Sunpoint2Value(float sunpoint) => sunpoint / 100f * 10f;

        public float MaxValue_Cell => DPS2Value(RecommendedDPS) * ValidCellCount;
        public float MaxValue_Sunpoint => Sunpoint2Value(ValidSunpointWhenFinalWave);
        public float RecommendedValue_Cell => MaxValue_Cell * MultiplierOfCell;
        public float RecommendedValue_Sunpoint => MaxValue_Sunpoint * MultiplierOfSunpoint;
    }

    [Serializable]
    public class InitialPlantConfig
    {
        public PlantDef PlantDef;
        public Direction2 Direction;
        public Vector2Int SpawnPos;
    }

    public enum DayPhaseType
    {
        NotSet = 0,
        Day = 1,
        Sunset = 2,
        Night = 3,
        MidNight = 4
    }

    public static class DayPhaseTypeExtensions
    {
        public static bool ShouldMushroomAwake(this DayPhaseType dayPhaseType)
        {
            return dayPhaseType is DayPhaseType.Night or DayPhaseType.MidNight;
        }
    }

    public enum WeatherType
    {
        NotSet = 0,
        Sunny = 1,
        Rainy = 2,
    }
}