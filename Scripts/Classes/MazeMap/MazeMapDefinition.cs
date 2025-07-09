using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public enum GameDifficulty
    {
        N0,
    }

    public enum SpotDifficulty
    {
        Normal_1,
        Normal_2,
        Normal_3,
        Normal_4,
        Normal_5,
        Elite_1,
        Elite_2,
        Elite_3,
        Elite_4,
        Elite_5,
        Boss_1,
        Boss_2,
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