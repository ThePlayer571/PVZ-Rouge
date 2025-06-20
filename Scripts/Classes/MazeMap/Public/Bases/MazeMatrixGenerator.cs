using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Core;
using TPL.PVZR.Core.Random;

namespace TPL.PVZR.Classes.MazeMap.Generator
{
    /// <summary>
    /// 一个好用的基类
    /// </summary>
    public abstract class MazeMatrixGenerator : IMazeMatrixGenerator
    {
        // 数据存储
        protected readonly MazeMapData MazeMapData;
        protected IMazeMapDefinition MazeMapDefinition => MazeMapData.definition;

        // 生成的迷宫矩阵 
        protected Matrix<Node> mazeMatrix;
        protected Dictionary<Node, List<Node>> adjacencyList = new();

        // 维护的数据结构
        protected Dictionary<int, List<int>> _levelKeyNodes = new Dictionary<int, List<int>>();

        protected MazeMatrixGenerator(in MazeMapData mazeMapData)
        {
            this.MazeMapData = mazeMapData;
        }

        public abstract (Matrix<Node> mazeMatrix, Dictionary<Node, List<Node>>) Generate();
        public abstract void ValidateParameters();
    }
}