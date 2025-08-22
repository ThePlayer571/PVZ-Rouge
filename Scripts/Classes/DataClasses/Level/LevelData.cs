using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.ZombieSpawner;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools.Random;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngineInternal;

namespace TPL.PVZR.Classes.DataClasses.Level
{
    public interface ILevelData
    {
        #region Others

        int InitialSunPoint { get; }
        GlobalEntityData GlobalEntityData { get; }

        #endregion

        #region Map

        LevelDef LevelDef { get; }
        Vector2Int MapSize { get; }
        Vector2 InitialPlayerPos { get; }
        AssetReference LevelPrefab { get; }
        Vector2Int GetRandomSunFallCellPos();

        #endregion

        #region WaveDef

        int TotalWaveCount { get; }
        IReadOnlyList<int> HugeWaves { get; }
        bool HasFinalBoss { get; }

        #endregion

        #region WaveDifficulty

        float ValueOfWave(int wave);

        #endregion

        #region WaveDurations

        float DurationOfWave(int wave);

        #endregion

        #region ZombieSpawn

        IReadOnlyList<ZombieSpawnInfo> ZombieSpawnInfosOfWave(int wave);

        #endregion

        #region Loot

        AwardGenerateInfo AwardGenerateInfo { get; }

        #endregion

        #region Environment

        DayPhaseType InitialDayPhase { get; }
        WeatherType InitialWeather { get; }

        #endregion

        #region Others

        IReadOnlyList<InitialPlantConfig> InitialPlants { get; }

        #endregion
    }

    /// <summary>
    /// 关卡的配置数据
    /// </summary>
    public class LevelData : ILevelData
    {
        #region Debug

        public void DebugLogValueOfEachWave()
        {
            for (var wave = 1; wave <= TotalWaveCount; wave++)
            {
                $"wave: {wave}, value: {ValueOfWave(wave)}".LogInfo();
            }
        }

        #endregion

        #region Definition

        #region Map

        public LevelDef LevelDef { get; }
        public Vector2Int MapSize { get; }
        public Vector2 InitialPlayerPos { get; }
        public AssetReference LevelPrefab { get; }

        private IReadOnlyList<SerializableKeyValuePair<Vector2Int, Vector2Int>> SunFallPositions { get; }

        public Vector2Int GetRandomSunFallCellPos()
        {
            var region = RandomHelper.Default.RandomChoose(SunFallPositions);
            var x = RandomHelper.Default.Range(region.Key.x, region.Value.x + 1);
            var y = RandomHelper.Default.Range(region.Key.y, region.Value.y + 1);
            return new Vector2Int(x, y);
        }

        #endregion

        #region WaveDef

        public int TotalWaveCount { get; }
        public IReadOnlyList<int> HugeWaves { get; }
        public bool HasFinalBoss { get; }

        #endregion

        #region WaveDifficulty

        private float BaseValue { get; }
        private float MaxValue { get; }
        private DifficultyGrowthType DifficultyGrowthType { get; }

        public float ValueOfWave(int wave)
        {
            switch (DifficultyGrowthType)
            {
                case DifficultyGrowthType.Logistic:
                    float scaler = HugeWaves.Contains(wave) ? 2f : 1f;
                    return LogisticValue(wave) * scaler;

                case DifficultyGrowthType.Linear:
                    if (HugeWaves.Contains(wave)) wave += 10;
                    return Mathf.Clamp(BaseValue * (0.7f + wave * 0.3f), 0, MaxValue);

                case DifficultyGrowthType.Quadratic:
                    if (HugeWaves.Contains(wave)) wave += 10;
                    return Mathf.Clamp(BaseValue * (0.04f * Mathf.Pow(wave, 2f) + 0.1f * wave + 0.86f), 0, MaxValue);
                default:
                    throw new ArgumentException();
            }
        }

        private float LogisticValue(int wave)
        {
            // S 型函数参数
            float x0 = TotalWaveCount / 2f; // 中点（默认居中）
            float k = 8f / TotalWaveCount; // 陡峭程度（经验值）
            float x = wave;

            // 标准化 Logistic 值：取 [0,1] 区间
            float logisticMin = 1f / (1f + Mathf.Exp(-k * (1f - x0)));
            float logisticMax = 1f / (1f + Mathf.Exp(-k * (TotalWaveCount - x0)));
            float logisticRaw = 1f / (1f + Mathf.Exp(-k * (x - x0)));

            float t = (logisticRaw - logisticMin) / (logisticMax - logisticMin);
            return Mathf.Lerp(BaseValue, MaxValue, t);
        }

        #endregion

        #region WaveDurations

        private List<SerializableKeyValuePair<Vector2Int, float>> WaveDurations;
        private float HugeWaveDurationOffset { get; }

        public float DurationOfWave(int wave)
        {
            if (wave == TotalWaveCount) return Mathf.Infinity;

            float offset = 0;
            if (HugeWaves.Contains(wave)) offset += HugeWaveDurationOffset;

            foreach (var info in WaveDurations)
            {
                if (wave >= info.Key.x && wave <= info.Key.y)
                {
                    return info.Value + offset;
                }
            }

            throw new ArgumentOutOfRangeException($"未考虑的Wave: {wave}");
        }

        #endregion

        #region ZombieSpawn

        private List<SerializableKeyValuePair<ZombieSpawnPosId, Vector2>> PosDef;
        private List<SerializableKeyValuePair<ZombieSpawnPosId, List<ZombieSpawnPosId>>> PosGroupDef;
        private List<ZombieSpawnConfig> ZombieSpawnConfigs;

        public Vector2 GetZombieSpawnPos(ZombieSpawnPosId posId)
        {
            return PosDef.First(item => item.Key == posId).Value;
        }

        public IReadOnlyList<ZombieSpawnInfo> ZombieSpawnInfosOfWave(int wave)
        {
            var result = new List<ZombieSpawnInfo>();
            foreach (var activeConfig in ZombieSpawnConfigs.Where(item =>
                         item.ActiveWaves.x <= wave && item.ActiveWaves.y >= wave))
            {
                if ((int)activeConfig.SpawnPos > 100 && (int)activeConfig.SpawnPos <= 200)
                {
                    var posGroup = PosGroupDef.First(item => item.Key == activeConfig.SpawnPos).Value;
                    foreach (var posId in posGroup)
                    {
                        var info = new ZombieSpawnInfo(
                            zombieId: activeConfig.Zombie,
                            spawnPosition: GetZombieSpawnPos(posId),
                            weight: activeConfig.Weight,
                            value: activeConfig.Value
                        );
                        result.Add(info);
                    }
                }
                else
                {
                    var info = new ZombieSpawnInfo(
                        zombieId: activeConfig.Zombie,
                        spawnPosition: PosDef.First(item => item.Key == activeConfig.SpawnPos).Value,
                        weight: activeConfig.Weight,
                        value: activeConfig.Value
                    );
                    result.Add(info);
                }
            }

            return result;
        }

        #endregion

        #region Loot

        public AwardGenerateInfo AwardGenerateInfo { get; }

        #endregion

        public DayPhaseType InitialDayPhase { get; }
        public WeatherType InitialWeather { get; }

        #region Others

        public IReadOnlyList<InitialPlantConfig> InitialPlants { get; }

        #endregion

        #endregion


        public int InitialSunPoint { get; }
        public GlobalEntityData GlobalEntityData { get; }


        public LevelData(in IGameData gameData, in LevelDefinition levelDefinition)
        {
            this.LevelDef = levelDefinition.LevelDef;
            this.InitialSunPoint = gameData.InventoryData.InitialSunPoint + levelDefinition.InitialSunpointOffset;
            this.GlobalEntityData = gameData.GlobalEntityData;

            this.MapSize = levelDefinition.MapSize;
            this.InitialPlayerPos = levelDefinition.InitialPlayerPos;
            this.LevelPrefab = levelDefinition.LevelPrefab;
            this.SunFallPositions = levelDefinition.SunFallPositions;

            this.TotalWaveCount = levelDefinition.TotalWaveCount;
            this.HugeWaves = levelDefinition.HugeWaves;
            this.HasFinalBoss = levelDefinition.HasFinalBoss;

            this.BaseValue = levelDefinition.BaseValue;
            this.MaxValue = levelDefinition.maxValue;
            this.DifficultyGrowthType = levelDefinition.DifficultyGrowthType;

            this.WaveDurations = levelDefinition.WaveDurations;
            this.HugeWaveDurationOffset = levelDefinition.HugeWaveDurationOffset;

            this.PosDef = levelDefinition.PosDef;
            this.PosGroupDef = levelDefinition.PosGroupDef;
            this.ZombieSpawnConfigs = levelDefinition.ZombieSpawnConfigs;

            this.AwardGenerateInfo = levelDefinition.AwardGenerateInfo;

            this.InitialDayPhase = levelDefinition.InitialDayPhase;
            this.InitialWeather = levelDefinition.InitialWeather;

            this.InitialPlants = levelDefinition.InitialPlants;
        }

        #region Private

        //

        #endregion
    }
}