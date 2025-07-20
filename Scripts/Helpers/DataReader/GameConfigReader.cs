using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class GameConfigReader
    {
        #region 数据配置

        private static Dictionary<LevelDef, LevelDefinition> _levelDefinitionDict;
        private static Dictionary<MazeMapDef, MazeMapDefinition> _mazeMapDefinitionDict;

        static GameConfigReader()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();

            // LevelDefinition
            var levelDefinitionList = resLoader
                .LoadSync<LevelDefinitionList>(Configlist.BundleName, Configlist.LevelDefinitionList)
                .levelDefinitionList;
            _levelDefinitionDict = new Dictionary<LevelDef, LevelDefinition>();
            foreach (var levelConfig in levelDefinitionList)
            {
                _levelDefinitionDict.Add(levelConfig.levelDef, levelConfig.levelDefinition);
            }

            // MazeMapDefinition
            var mazeMapDefinitionList = resLoader
                .LoadSync<MazeMapDefinitionList>(Configlist.BundleName, Configlist.MazeMapDefinitionList)
                .mazeMapDefinitionList;
            _mazeMapDefinitionDict = new Dictionary<MazeMapDef, MazeMapDefinition>();
            foreach (var mazeMapConfig in mazeMapDefinitionList)
            {
                _mazeMapDefinitionDict.Add(mazeMapConfig.mazeMapDef, mazeMapConfig.mazeMapDefinition);
            }
        }

        #endregion

        #region 数据读取

        public static MazeMapDefinition GetMazeMapDefinition(MazeMapDef mazeMapDef)
        {
            if (_mazeMapDefinitionDict.TryGetValue(mazeMapDef, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"找不到对应的MazeMapDefinition: {mazeMapDef}");
        }

        public static LevelDefinition GetLevelDefinition(LevelDef levelDef)
        {
            if (_levelDefinitionDict.TryGetValue(levelDef, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"找不到对应的LevelDefinition: {levelDef}");
        }

        #endregion
    }
}