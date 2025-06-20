using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.MazeMap.Public;
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
        public Dictionary<Node, List<Node>> adjacencyList { get; set; }
        public Node startNode => mazeMatrix.First(n => n.level == 0);

        public MazeMapData(IMazeMapDefinition definition, ulong generatedSeed)
        {
            this.definition = definition;
            this.generatedSeed = generatedSeed;
            //
            GenerateMazeMatrix();
        }

        public void GenerateMazeMatrix()
        {
            doMazeMatrixGenerated = true;
            // 生成迷宫地图数据
            var generator = MazeMapGenerateHelper.GetGenerator(this);
            (this.mazeMatrix, this.adjacencyList) = generator.Generate();
        }

        public bool doMazeMatrixGenerated { get; private set; } = false;
    }
}