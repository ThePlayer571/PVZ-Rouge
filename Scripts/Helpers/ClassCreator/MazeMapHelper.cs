using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Classes.MazeMap.Public;
using TPL.PVZR.Classes.MazeMap.Public.DaveHouse;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class MazeMapHelper
    {
        public static MazeMapData CreateMazeMapData(MazeMapIdentifier mazeMapIdentifier, ulong seed)
        {
            switch (mazeMapIdentifier)
            {
                case MazeMapIdentifier.DaveHouse:
                    return new MazeMapData(new DaveHouseMazeMapDefinition(), seed);
                default:
                    throw new System.NotSupportedException($"不支持的迷宫地图标识符：{mazeMapIdentifier}");
            }
        }
    }
}