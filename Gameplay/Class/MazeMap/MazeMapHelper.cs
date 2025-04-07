using System;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.Class.MazeMap.MazeMaps;

namespace TPL.PVZR.Gameplay.Class.MazeMap
{
    public static class MazeMapHelper
    {
        public struct MazeMapCreateData
        {
            public MazeMapIdentifier identifier;
            public ulong seed;
        }
        public static MazeMap CreateMazeMap(MazeMapCreateData data)
        {
            RandomHelper.MazeMap.RestoreState(data.seed);
            switch (data.identifier)
            {
                case MazeMapIdentifier.DaveLawn:
                    return new DaveLawn();
                default:
                    throw new ArgumentException($"未处理该参数：{data.identifier}");
            }
        }
    }
}