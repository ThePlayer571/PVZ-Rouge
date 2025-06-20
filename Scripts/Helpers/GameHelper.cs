using System;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Classes.MazeMap.Instances.DaveHouse;
using TPL.PVZR.Core.Random;

namespace TPL.PVZR.Helpers
{
    public static class GameHelper
    {
        public static GameData CreateGameData(ulong? seed = null)
        {
            seed ??= RandomHelper.Default.NextUnsigned();
            //
            return new GameData(MazeMapHelper.CreateMazeMapData(MazeMapIdentifier.DaveHouse, seed.Value));
        }
    }
}