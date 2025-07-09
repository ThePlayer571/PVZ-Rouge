using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Classes.MazeMap.New;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class MazeMapHelper
    {
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

        private static Dictionary<(MazeMapId, GameDifficulty), MazeMapDefinition> _mazeMapDict;


        public static MazeMapData CreateMazeMapData(MazeMapId mazeMapId, GameDifficulty difficulty, ulong seed)
        {
            if (_mazeMapDict.TryGetValue((mazeMapId, difficulty), out var mazeMapDefinition))
            {
                return new MazeMapData(mazeMapDefinition, seed);
            }

            throw new Exception($"不支持的mazeMapId和难度组合：{mazeMapId}, {difficulty}");
        }
    }
}