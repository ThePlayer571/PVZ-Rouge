using System;
using TPL.PVZR.Classes.MazeMap.Interfaces;
using TPL.PVZR.Classes.MazeMap.Public;
using TPL.PVZR.Classes.MazeMap.Public.DaveHouse;

namespace TPL.PVZR.Classes.MazeMap
{
    public static class MazeMapGenerateHelper
    {
        // Generator需要阅读MazeMapData里的信息（比如地图的长宽），这里不能传enum进来
        public static IMazeMatrixGenerator GetGenerator(MazeMapData mazeMapData)
        {
            switch (mazeMapData.definition.generatorId)
            {
                case GeneratorId.DaveHouse: return new DaveHouseMazeMatrixGenerator(mazeMapData);
                default:
                    throw new NotSupportedException($"不支持的生成器ID：{mazeMapData.definition.generatorId}");
            }
        }
    }
}