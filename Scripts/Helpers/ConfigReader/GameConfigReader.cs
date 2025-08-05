using System.Collections.Generic;
using System.Threading.Tasks;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.MazeMap;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class GameConfigReader
    {
        #region 数据配置

        private static Dictionary<LevelDef, LevelDefinition> _levelDefinitionDict;
        private static Dictionary<MazeMapDef, MazeMapDefinition> _mazeMapDefinitionDict;
        private static Dictionary<GameDef, GameDefinition> _gameDefinitionDict;

        public static async Task InitializeAsync()
        {
            var levelDefinition_handle = Addressables.LoadAssetsAsync<LevelDefinition>("LevelDefinition", null);
            var mazeMapDefinition_handle = Addressables.LoadAssetsAsync<MazeMapDefinition>("MazeMapDefinition", null);
            var gameDefinition_handle = Addressables.LoadAssetsAsync<GameDefinition>("GameDefinition", null);

            await Task.WhenAll(levelDefinition_handle.Task, mazeMapDefinition_handle.Task, gameDefinition_handle.Task);

            _levelDefinitionDict = new Dictionary<LevelDef, LevelDefinition>();
            foreach (var levelDefinition in levelDefinition_handle.Result)
            {
                _levelDefinitionDict.Add(levelDefinition.LevelDef, levelDefinition);
            }

            _mazeMapDefinitionDict = new Dictionary<MazeMapDef, MazeMapDefinition>();
            foreach (var mazeMapDefinition in mazeMapDefinition_handle.Result)
            {
                _mazeMapDefinitionDict.Add(mazeMapDefinition.Def, mazeMapDefinition);
            }

            _gameDefinitionDict = new Dictionary<GameDef, GameDefinition>();
            foreach (var gameDefinition in gameDefinition_handle.Result)
            {
                _gameDefinitionDict.Add(gameDefinition.GameDef, gameDefinition);
            }
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