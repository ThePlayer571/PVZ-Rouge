using System;
using TPL.PVZR.Classes.MazeMap.Generator;
using TPL.PVZR.Classes.MazeMap.Instances.DaveHouse;

namespace TPL.PVZR.Classes.MazeMap.Public
{
    public static class MazeMapGenerateHelper
    {
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