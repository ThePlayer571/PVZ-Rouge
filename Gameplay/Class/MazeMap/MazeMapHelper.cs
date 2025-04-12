using System;
using System.Collections.Generic;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Core.Save.Modules;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;
using TPL.PVZR.Gameplay.Class.MazeMap.MazeMaps;

namespace TPL.PVZR.Gameplay.Class.MazeMap
{
    public static class MazeMapHelper
    {
        public static IMazeMap CreateMazeMap(MazeMapSaveData data)
        {
            // 设置随机数
            RandomHelper.MazeMap.RestoreState(data.seed);
            switch (data.identifier)
            {
                case MazeMapIdentifier.DaveLawn:
                    return new DaveLawn(data);
                default:
                    throw new ArgumentException($"未处理该参数：{data.identifier}");
            }
        }
    }
}