using System.Collections.Generic;
using TPL.PVZR.Core;

namespace TPL.PVZR.Classes.MazeMap
{
    public interface IMazeMapData
    {
        // Definition
        IMazeMapDefinition definition { get; }
        
        // Runtime Data
        ulong generatedSeed { get; }
        
        Matrix<Node> mazeMatrix { get; }
        public Dictionary<Node, List<Node>> adjacencyList { get; }

        // 数据结构生成
        public void GenerateMazeMatrix();
        bool doMazeMatrixGenerated { get; }
        
        // 便民属性
        Node startNode { get; }
        //
    }
}