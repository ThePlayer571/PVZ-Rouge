using System;
using System.Linq;
using TPL.PVZR.Core;

namespace TPL.PVZR.Classes.MazeMap
{
    /// <summary>
    /// 实用的基类
    /// </summary>
    public class MazeMapData : IMazeMapData
    {
        public IMazeMapDefinition definition { get; }
        public ulong generatedSeed { get; }
        public Matrix<Node> mazeMatrix { get; set; }
        public Node startNode => mazeMatrix.First(n => n.level == 0);

        public MazeMapData(IMazeMapDefinition definition, ulong generatedSeed)
        {
            this.definition = definition;
            this.generatedSeed = generatedSeed;
        }
    }
}