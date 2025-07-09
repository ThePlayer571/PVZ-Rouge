using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap.New
{
    public class MazeMapData
    {
        #region Definition

        #region 基础设置

        public MazeMapId Id { get; }
        public GameDifficulty GameDifficulty { get; }
        public int RowCount { get; }
        public int ColCount { get; }

        #endregion

        #region Spot生成

        public int TotalLevelCount { get; }
        private List<SerializableKeyValuePair<Vector2Int, Vector2Int>> SpotCountRangeOfLevel { get; }

        public Vector2Int GetSpotCountRangeOfLevel(int level)
        {
            foreach (var pair in SpotCountRangeOfLevel)
            {
                if (pair.Key.x <= level && level <= pair.Key.y)
                {
                    return pair.Value;
                }
            }

            throw new Exception($"未考虑的level: {level}，请检查 SpotCountRangeOfLevel 设置");
        }

        private List<SpotContentConfigs> SpotGenerateConfigs;

        #endregion

        #endregion

        public ulong GeneratedSeed { get; private set; }
        
        public MazeMapData(MazeMapDefinition definition, ulong seed)
        {
            Id = definition.Id;
            GameDifficulty = definition.GameDifficulty;
            RowCount = definition.RowCount;
            ColCount = definition.ColCount;

            TotalLevelCount = definition.TotalLevelCount;
            SpotCountRangeOfLevel = definition.SpotCountRangeOfLevel;
            SpotGenerateConfigs = definition.SpotContentConfigs;

            GeneratedSeed = seed;
        }
    }
}