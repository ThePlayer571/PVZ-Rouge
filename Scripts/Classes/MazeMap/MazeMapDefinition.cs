using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.LootPool;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    [Serializable]
    public enum GameDifficulty
    {
        NotSet = 0,
        N0 = 1,
        N1 = 2,
    }

    [Serializable]
    public enum StageDifficulty
    {
        NotSet = 0,
        Normal_1 = 1,
        Normal_2 = 2,
        Normal_3 = 3,
        Normal_4 = 4,
        Normal_5 = 5,
        Elite_1 = 11,
        Elite_2 = 12,
        Elite_3 = 13,
        Elite_4 = 14,
        Elite_5 = 15,
        Boss_1 = 21,
        Boss_2 = 22,
    }

    [Serializable]
    public class TombContentConfig
    {
        public StageDifficulty StageDifficulty;
        public Vector2Int ActiveStages;
        [Tooltip("运行时会自动识别生成LevelDef")] public List<LevelId> IncludedLevels;
    }

    [CreateAssetMenu(fileName = "MazeMapDefinition_", menuName = "PVZR/MazeMapDefinition", order = 4)]
    public class MazeMapDefinition : ScriptableObject
    {
        [Header("基础设置")] public MazeMapDef Def;
        [Tooltip("列数")] public int RowCount;
        [Tooltip("道路的数量")] public int ColCount;

        //
        [Header("墓碑生成")] [Tooltip("总等级数")] public int TotalStageCount;

        [Tooltip("等级区间->该等级墓碑数量范围")]
        public List<SerializableKeyValuePair<Vector2Int, Vector2Int>> TombCountRangeOfStage;

        [Tooltip("墓碑内容配置")] public List<TombContentConfig> TombContentConfigs;

        // Loot
        [Header("LootPool")] public List<LootPoolDef> LootPools;
        public AwardGenerateInfo InitialAwards;
    }
}