using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    [Serializable]
    public enum GameDifficulty
    {
        N0 = 0,
    }
    
    [Serializable]
    public enum SpotDifficulty
    {
        Normal_1 = 0,
        Normal_2 = 1,
        Normal_3 = 2,
        Normal_4 = 3,
        Normal_5 = 4,
        Elite_1 = 5,
        Elite_2 = 6,
        Elite_3 = 7,
        Elite_4 = 8,
        Elite_5 = 9,
        Boss_1 = 10,
        Boss_2 = 11,
    }

    [Serializable]
    public class SpotContentConfigs
    {
        public SpotDifficulty SpotDifficulty;
        public Vector2Int ActiveLevels;
        public List<LevelId> IncludedLevels;
    }

    [CreateAssetMenu(fileName = "MazeMapDefinition_", menuName = "PVZR/MazeMapDefinition", order = 4)]
    public class MazeMapDefinition : ScriptableObject
    {
        [Header("基础设置")] public MazeMapId Id;
        public GameDifficulty GameDifficulty;
        [Tooltip("行的数量")] public int RowCount;
        [Tooltip("列的数量")] public int ColCount;

        [Header("Spot生成")] public int TotalLevelCount;
        [Tooltip("等级区间->该等级Spot数量的范围")]
        public List<SerializableKeyValuePair<Vector2Int, Vector2Int>> SpotCountRangeOfLevel;
        public List<SpotContentConfigs> SpotContentConfigs;
    }
}