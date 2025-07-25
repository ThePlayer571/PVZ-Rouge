using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses.Game;
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
        private static Dictionary<GameDef, GameDefinition> _gameDefinitionDict;

        static GameConfigReader()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();

            // LevelDefinition
            var levelDefinitionList = resLoader
                .LoadSync<LevelDefinitionList>(Configlist.BundleName, Configlist.LevelDefinitionList)
                .levelDefinitionList;
            _levelDefinitionDict = new Dictionary<LevelDef, LevelDefinition>();
            foreach (var levelDefinition in levelDefinitionList)
            {
                _levelDefinitionDict.Add(levelDefinition.LevelDef, levelDefinition);
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

            // GameDefinition
            var gameDefinitionList = resLoader
                .LoadSync<GameDefinitionList>(Configlist.BundleName, Configlist.GameDefinitionList)
                .gameDefinitionList;
            _gameDefinitionDict = new Dictionary<GameDef, GameDefinition>();
            foreach (var gameConfig in gameDefinitionList)
            {
                _gameDefinitionDict.Add(gameConfig.gameDef, gameConfig.gameDefinition);
            }

            resLoader.Recycle2Cache();
        }

        #endregion

        #region 数据读取

        public static GameDefinition GetGameDefinition(GameDef gameDef)
        {
            if (_gameDefinitionDict.TryGetValue(gameDef, out var gameDefinition))
            {
                return gameDefinition;
            }

            throw new KeyNotFoundException($"找不到对应的GameDefinition: {gameDef}");
        }

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