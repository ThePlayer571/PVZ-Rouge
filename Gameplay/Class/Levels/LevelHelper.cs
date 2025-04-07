using System;
using TPL.PVZR.Gameplay.Class.Levels.Levels.DaveLawn;

namespace TPL.PVZR.Gameplay.Class.Levels
{
    public static class LevelHelper
    {

        public static Level GetLevel(LevelIdentifier levelIdentifier)
        {
            switch (levelIdentifier)
            {
                case LevelIdentifier.DaveHouse:
                    return new DaveLawn();
                default:
                    throw new ArgumentException($"未处理该参数：{levelIdentifier}");
            }
        }
    }
}