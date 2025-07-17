using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class GameConfigReader
    {
        private static Dictionary<LevelId, LevelDefinition> _levelDefinitionDict;
        private static Dictionary<(MazeMapId, GameDifficulty), MazeMapDefinition> _mazeMapDefinitionDict;
        
        static GameConfigReader()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();
            
            // LevelDefinition
            var levelDefinitionList = resLoader.LoadSync<LevelDefinitionList>(Configlist.BundleName, Configlist.);

        }

        static LevelHelper()
        {
            _levelDefinitionDict = new Dictionary<LevelId, LevelDefinition>
            {
                [LevelId.Dave_Lawn] = resLoader.LoadSync<LevelDefinition>(Leveldefinition.BundleName,
                    Leveldefinition.LevelDefinition_DaveLawn),
            };
        }
        static MazeMapHelper()
        {
            var resLoader = ResLoader.Allocate();
            _mazeMapDict = new Dictionary<(MazeMapId, GameDifficulty), MazeMapDefinition>
            {
                [(MazeMapId.DaveLawn, GameDifficulty.N0)] =
                    resLoader.LoadSync<MazeMapDefinition>(Mazemapdefinition.BundleName,
                        Mazemapdefinition.MazeMapDefinition_DaveLawn),
            };
        }

    }
}