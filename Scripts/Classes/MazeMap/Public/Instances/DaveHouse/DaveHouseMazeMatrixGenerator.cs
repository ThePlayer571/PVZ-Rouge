using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.MazeMap.Generator;
using TPL.PVZR.Core;
using TPL.PVZR.Core.Random;

namespace TPL.PVZR.Classes.MazeMap.Instances.DaveHouse
{
    public class DaveHouseMazeMatrixGenerator : MazeMatrixGenerator
    {
        public DaveHouseMazeMatrixGenerator(in MazeMapData mazeMapData) : base(in mazeMapData)
        {
        }


        #region Public

        public override void ValidateParameters()
        {
            if (MazeMapData == null) throw new ArgumentNullException(nameof(MazeMapData), "MazeMapData 不能为空");
            if (MazeMapDefinition.colCount < 5)
                throw new Exception($"暂不支持：colCount={MazeMapDefinition.colCount}，必须>=5");
            if ((MazeMapDefinition.rowCount - 1) % 2 != 0 || MazeMapDefinition.rowCount < 8)
                throw new Exception($"暂不支持：rowCount={MazeMapDefinition.rowCount}，必须>=8且(rowCount-1)%2==0");
            if (MazeMapDefinition.levelCount != (MazeMapDefinition.rowCount - 1) / 2)
                throw new Exception(
                    $"levelCount不匹配：levelCount={MazeMapDefinition.levelCount}，应为(rowCount-1)/2={((MazeMapDefinition.rowCount - 1) / 2)}");
            if (MazeMapDefinition.spotCountRangeInLevel.min != (MazeMapDefinition.colCount - 1) / 2)
                throw new Exception(
                    $"spotCountRangeInLevel.min不匹配：min={MazeMapDefinition.spotCountRangeInLevel.min}，应为(colCount-1)/2={((MazeMapDefinition.colCount - 1) / 2)}");
            if (MazeMapDefinition.spotCountRangeInLevel.max != (MazeMapDefinition.colCount + 3) / 2)
                throw new Exception(
                    $"spotCountRangeInLevel.max不匹配：max={MazeMapDefinition.spotCountRangeInLevel.max}，应为(colCount+3)/2={((MazeMapDefinition.colCount + 3) / 2)}");
            if (MazeMapDefinition.spotCountRangeInFirstLevel.min != (MazeMapDefinition.colCount + 1) / 2)
                throw new Exception(
                    $"spotCountRangeInFirstLevel.min不匹配：min={MazeMapDefinition.spotCountRangeInFirstLevel.min}，应为(colCount+1)/2={((MazeMapDefinition.colCount + 1) / 2)}");
            if (MazeMapDefinition.spotCountRangeInFirstLevel.max != (MazeMapDefinition.colCount + 1) / 2)
                throw new Exception(
                    $"spotCountRangeInFirstLevel.max不匹配：max={MazeMapDefinition.spotCountRangeInFirstLevel.max}，应为(colCount+1)/2={((MazeMapDefinition.colCount + 1) / 2)}");
        }

        public override Matrix<Node> Generate()
        {
            // 异常处理
            ValidateParameters();

            // 初始化
            RandomHelper.MazeMap.RestoreState(new DeterministicRandom.State(MazeMapData.generatedSeed));
            mazeMatrix = new Matrix<Node>(MazeMapDefinition.rowCount, MazeMapDefinition.colCount);
            mazeMatrix.Fill((i, j) => new Node(i, j));

            //
            GenerateDaveHose();
            return mazeMatrix;
        }

        #endregion

        private void GenerateDaveHose()
        {
            MarkKeyNodes();
            BridgeKeyNodes();
        }

        #region 直接调用的

        void MarkKeyNodes()
        {
            // [1. ] 起始点
            {
                // 起始点在第0行中间
                var col = mazeMatrix.Columns / 2;
                var startNode = mazeMatrix[0, col];
                startNode.isKey = true;
                startNode.level = 0; // 起始点的 level 为 0
                _levelKeyNodes.Add(0, new List<int> { col });
            }
            // [2. ] 第一关
            {
                var level = 1;
                var row = 2;
                var levelCount =
                    RandomHelper.MazeMap.Range(MazeMapDefinition.spotCountRangeInFirstLevel.min,
                        MazeMapDefinition.spotCountRangeInFirstLevel.max + 1); // 第一关有3个关键节点
                List<int> keyNodes = new List<int>();
                // 随机选择levelCount个列
                List<int> chosen = RandomHelper.MazeMap
                    .RandomSubset(Enumerable.Range(0, MazeMapDefinition.colCount), levelCount).ToList();
                foreach (var col in chosen)
                {
                    var node = mazeMatrix[row, col];
                    node.isKey = true;
                    node.level = level;
                    keyNodes.Add(col);
                }

                _levelKeyNodes.Add(level, keyNodes);
            }
            // [3. 其他关]
            for (int level = 2; level < MazeMapDefinition.levelCount; level++)
            {
                // 预制数据
                var row = level * 2;
                var levelCount = RandomHelper.MazeMap.Range(MazeMapDefinition.spotCountRangeInLevel.min,
                    MazeMapDefinition.spotCountRangeInLevel.max + 1);
                List<int> lastCols = _levelKeyNodes[level - 1];
                List<int> keyNodes = new List<int>();

                // 保证每个上一关的关键节点附近有本关的关键节点
                foreach (var lastCol in lastCols)
                {
                    // 取上一关关键节点附近的列（-1, 0, +1）
                    var _ = Enumerable.Range(lastCol - 1, 3).Where(col => col >= 0 && col < MazeMapDefinition.colCount)
                        .ToList();
                    // 如果这些列都没有被标记为关键节点，则随机选一个作为关键节点
                    if (_.All(col => !mazeMatrix[row, col].isKey))
                    {
                        var col = RandomHelper.MazeMap.RandomChoose(_);
                        var node = mazeMatrix[row, col];
                        node.isKey = true;
                        node.level = level; // 设置 level
                        keyNodes.Add(col);
                    }
                }

                // 保证KeyNode的数量
                while (keyNodes.Count < levelCount)
                {
                    // 使用 RandomSubset 从未标记的列中直接选择剩余所需数量的列
                    var remainingCols = Enumerable.Range(0, MazeMapDefinition.colCount)
                        .Where(c => !mazeMatrix[row, c].isKey)
                        .ToList();
                    var additionalCols =
                        RandomHelper.MazeMap.RandomSubset(remainingCols, levelCount - keyNodes.Count);
                    foreach (var col in additionalCols)
                    {
                        var node = mazeMatrix[row, col];
                        node.isKey = true;
                        node.level = level; // 设置 level
                        keyNodes.Add(col);
                    }
                }

                //
                _levelKeyNodes.Add(level, keyNodes);
            }

            // [4. 最终关]
            {
                var level = MazeMapDefinition.levelCount;
                var row = level * 2;
                var levelCount = 1;
                List<int> keyNodes = new List<int>();
                var col = RandomHelper.MazeMap.RandomChoose(Enumerable.Range(0, MazeMapDefinition.colCount));
                var node = mazeMatrix[row, col];
                node.isKey = true;
                node.level = level; 
                keyNodes.Add(col);
                _levelKeyNodes.Add(level, keyNodes);
            }
        }

        /// <summary>
        /// 为关键节点之间生成路径，并更新节点的连接关系。
        /// </summary>
        private void BridgeKeyNodes()
        {
            foreach (var level in _levelKeyNodes.Keys)
            {
                if (!_levelKeyNodes.ContainsKey(level + 1)) continue;

                // 预制数据
                var row = level * 2;
                var fromNodesCol = _levelKeyNodes[level];
                var toNodesCol = _levelKeyNodes[level + 1];

                // [1. ] 粗略选取FromTo
                var FromTo = new Dictionary<int, List<int>>();

                foreach (var from in fromNodesCol)
                {
                    // 找到距离最近的To
                    var closestTo = toNodesCol.OrderBy(to => Math.Abs(from - to)).First();
                    FromTo[from] = new List<int> { closestTo };
                }

                foreach (var to in toNodesCol)
                {
                    if (!FromTo.Values.Any(list => list.Contains(to)))
                    {
                        var closestFrom = FromTo.Keys.OrderBy(from => Math.Abs(from - to)).First();
                        FromTo[closestFrom].Add(to);
                    }
                }

                // [2. ] 根据FromTo生成连通分图
                List<List<int>> connectedComponents = new();
                foreach (var from in fromNodesCol)
                {
                    foreach (var to in FromTo[from])
                    {
                        var route = Enumerable.Range(Math.Min(from, to), Math.Abs(to - from) + 1).ToList();
                        // 融入旧分图
                        if (connectedComponents.Any(list => list.Any(col => route.Contains(col))))
                        {
                            // 找到旧分图
                            var oldComponent = connectedComponents.First(list => list.Any(col => route.Contains(col)));
                            // 将新的路径添加到旧分图中
                            oldComponent.AddRange(route);
                            oldComponent = oldComponent.Distinct().ToList(); // 去重
                        }
                        // 新的分图
                        else
                        {
                            var newComponent = route.Distinct().ToList();
                            connectedComponents.Add(newComponent);
                        }
                    }
                }

                // [3. ] 根据连通分图生成Node的连通情况
                // from往前
                foreach (var col in fromNodesCol)
                {
                    var from = mazeMatrix[row, col];
                    var to = mazeMatrix[row + 1, col];
                    ConnectNodes(from, to);
                }

                // to往后
                var toRow = row + 2;
                foreach (var col in toNodesCol)
                {
                    var from = mazeMatrix[toRow - 1, col];
                    var to = mazeMatrix[toRow, col];
                    ConnectNodes(from, to);
                }

                // 中间的往左右
                var midRow = row + 1;
                foreach (var component in connectedComponents)
                {
                    if (component.Count < 2) continue; // 至少需要两个节点才能连接
                    component.Sort();
                    foreach (var col in component)
                    {
                        if (col == component.Last()) break;
                        var from = mazeMatrix[midRow, col];
                        var to = mazeMatrix[midRow, col + 1];
                        ConnectNodes(from, to);
                    }
                }
            }
        }

        #endregion

        #region 间接调用的

        /// <summary>
        /// 更新两个节点之间的连接关系。
        /// </summary>
        /// <param name="a">第一个节点</param>
        /// <param name="b">第二个节点</param>
        private void ConnectNodes(Node a, Node b)
        {
            if (a == null || b == null) throw new ArgumentNullException("节点不能为空");
            // 此外，还有异常：只有直接直线相邻的两个结点才能调用（后面会处理）

            if (a.x == b.x)
            {
                if (a.y + 1 == b.y)
                {
                    a.connections.Yp = true;
                    b.connections.Yn = true;
                }
                else if (a.y - 1 == b.y)
                {
                    a.connections.Yn = true;
                    b.connections.Yp = true;
                }
                else
                {
                    throw new ArgumentException("ConnectNodes: 两节点的x相同，但y不相邻");
                }
            }
            else if (a.y == b.y)
            {
                if (a.x + 1 == b.x)
                {
                    a.connections.Xp = true;
                    b.connections.Xn = true;
                }
                else if (a.x - 1 == b.x)
                {
                    a.connections.Xn = true;
                    b.connections.Xp = true;
                }
                else
                {
                    throw new ArgumentException("ConnectNodes: 两节点的y相同，但x不相邻");
                }
            }
            else
            {
                throw new ArgumentException(" ConnectNodes: 两节点x,y皆不相同");
            }
        }

        #endregion
    }
}