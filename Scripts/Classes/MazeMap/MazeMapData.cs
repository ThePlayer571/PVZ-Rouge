using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Classes.LootPool;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.Save;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public interface IMazeMapWiseData : IMazeMapData
    {
        void AddDiscoveredTomb(ITombData tombData);
        void AddPassedTomb(ITombData tombData);
    }

    public interface IMazeMapData : ISavable<MazeMapSaveData>
    {
        // 基本数据
        MazeMapDef Def { get; }
        int RowCount { get; }
        int ColCount { get; }
        int TotalStageCount { get; }
        Vector2Int GetSpotCountRangeOfStage(int stage);
        LevelDef GetRandomLevelOfStage(int stage);


        ulong GenerateSeed { get; }

        // 奖励池
        IReadOnlyList<LootPoolDef> LootPools { get; }
        
        AwardGenerateInfo InitialAwards { get; }


        // 通关进度（以最低限度存储）
        IReadOnlyList<ITombData> DiscoveredTombs { get; }
        IReadOnlyList<Vector2Int> PassedRoute { get; }
    }

    public class MazeMapData : IMazeMapWiseData
    {
        #region Definition

        #region 基础设置

        public MazeMapDef Def { get; }
        public int RowCount { get; }
        public int ColCount { get; }

        #endregion

        #region 墓碑生成

        public int TotalStageCount { get; }
        private List<SerializableKeyValuePair<Vector2Int, Vector2Int>> TombCountRangeOfStage { get; }

        public Vector2Int GetSpotCountRangeOfStage(int stage)
        {
            foreach (var pair in TombCountRangeOfStage)
            {
                if (pair.Key.x <= stage && stage <= pair.Key.y)
                {
                    return pair.Value;
                }
            }

            throw new Exception($"未考虑的stage: {stage}，请检查 TombCountRangeOfStage 设置");
        }

        private List<TombContentConfig> TombContentConfigs { get; }

        private readonly Dictionary<StageDifficulty, List<LevelId>> _availableLevelsPerDifficulty = new();

        public LevelDef GetRandomLevelOfStage(int stage)
        {
            var tombContentConfig =
                TombContentConfigs.First(config => config.ActiveStages.x <= stage && stage <= config.ActiveStages.y);

            // 尝试初始化
            if (!_availableLevelsPerDifficulty.ContainsKey(tombContentConfig.StageDifficulty) ||
                _availableLevelsPerDifficulty[tombContentConfig.StageDifficulty].Count == 0)
            {
                var shuffledLevels =
                    RandomHelper.Game.Shuffle(tombContentConfig.IncludedLevels.ToList()) as List<LevelId>;
                _availableLevelsPerDifficulty[tombContentConfig.StageDifficulty] = shuffledLevels;
            }

            var availableLevels = _availableLevelsPerDifficulty[tombContentConfig.StageDifficulty];
            var selectedLevelId = availableLevels.Pop();

            // 创建并返回 LevelDef
            return new LevelDef
            {
                Id = selectedLevelId,
                Difficulty = Def.Difficulty,
                StageDifficulty = tombContentConfig.StageDifficulty,
            };
        }

        #endregion

        #endregion

        public ulong GenerateSeed { get; private set; }
        public IReadOnlyList<LootPoolDef> LootPools { get; }
        public AwardGenerateInfo InitialAwards { get; }
        private readonly List<ITombData> _discoveredTombs = new();
        private readonly List<Vector2Int> _passedRoute = new();
        public IReadOnlyList<ITombData> DiscoveredTombs => _discoveredTombs;
        public IReadOnlyList<Vector2Int> PassedRoute => _passedRoute;

        public void AddDiscoveredTomb(ITombData tombData)
        {
            _discoveredTombs.Add(tombData);
        }

        public void AddPassedTomb(ITombData tombData)
        {
            _passedRoute.Add(tombData.Position);
        }


        public MazeMapData(MazeMapDefinition definition, ulong seed)
        {
            Def = definition.Def;
            RowCount = definition.RowCount;
            ColCount = definition.ColCount;

            TotalStageCount = definition.TotalStageCount;
            TombCountRangeOfStage = definition.TombCountRangeOfStage;
            TombContentConfigs = definition.TombContentConfigs;

            GenerateSeed = seed;

            LootPools = definition.LootPools;
            InitialAwards = definition.InitialAwards;
        }

        public MazeMapData(MazeMapSaveData saveData)
        {
            var definition = GameConfigReader.GetMazeMapDefinition(saveData.mazeMapDef);
            Def = definition.Def;
            RowCount = definition.RowCount;
            ColCount = definition.ColCount;
            TotalStageCount = definition.TotalStageCount;
            TombCountRangeOfStage = definition.TombCountRangeOfStage;
            TombContentConfigs = definition.TombContentConfigs;
            GenerateSeed = saveData.generateSeed;
            LootPools = definition.LootPools;
            // 没存initialAwards，因为大抵是不需要
            _discoveredTombs = saveData.discoveredTombs.Select(tombSaveData => new TombData(tombSaveData) as ITombData)
                .ToList();
            _passedRoute = saveData.passedRoute.Select(pos => new Vector2Int(pos.x, pos.y)).ToList();
        }

        public MazeMapSaveData ToSaveData()
        {
            return new MazeMapSaveData
            {
                mazeMapDef = Def,
                generateSeed = GenerateSeed,
                discoveredTombs = DiscoveredTombs.Select(tomb => tomb.ToSaveData()).ToList(),
                passedRoute = PassedRoute.Select(pos => new Vector2IntSerializable(pos)).ToList()
            };
        }
    }
}