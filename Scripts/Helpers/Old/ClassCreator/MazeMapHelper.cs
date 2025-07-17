using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class MazeMapHelper
    {


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