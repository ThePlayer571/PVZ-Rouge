using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Core;
using TPL.PVZR.Core.Extensions;
using TPL.PVZR.Core.Random;

namespace TPL.PVZR.Gameplay.Class.MazeMap.Core
{
    public abstract partial class MazeMap
    {
        protected void CreateMain()
        {
            /*
             * [STEP 1] 生成迷宫地图的节点
             * [STEP 2] 选取keyNode的to
             * [STEP 3] 绘制路径(粗略)
             * [STEP 4] 优化路径
             * [STEP 5] 生成关卡
             * [STEP 6] 设置玩家游玩数据
             */
            {
                Node.ResetNodeId();
                // [STEP 1] 生成迷宫地图的节点
                mazeGrid = new Matrix<Node>(MapConfig.rowCount, MapConfig.colCount);
                this.GenerateNodes(mazeGrid);
                // [STEP 2] 选取每个keyNodes的to
                this.GenerateKeyNodesFromToMain();
                // [STEP 3] 绘制路径(粗略)
                this.GenerateRoute();
                // [STEP 4] 优化路径 - 暂时不需要
                // FunctionConfig.BetterRoutes();
                // [STEP 5] 生成关卡数据
                this.GenerateSpots();
                // [STEP 6] 设置玩家游玩数据
                this.SetPlayerData();
            }
            # region Test
            // //
            // // 测试
            // // 打印地图
            // for (int i = 0; i < MapConfig.rowCount; i++)
            // {
            //     for (int j = 0; j < MapConfig.colCount; j++)
            //     {
            //         if (mazeGrid[i, j].level != -1)
            //         {
            //             Console.Write($"{mazeGrid[i, j].level} ");
            //         }
            //         else if (!mazeGrid[i, j].toNode.Any())
            //         {
            //             Console.Write("X ");
            //         }
            //         else
            //         {
            //             var thisNode = mazeGrid[i, j];
            //             var toNode = thisNode.toNode;
            //             ValueTuple<int, int> direction;
            //             if (thisNode.toNode.Count == 1)
            //             {
            //                 direction = thisNode.toNode.First().mazePos - thisNode.mazePos;
            //             }
            //             else
            //             {
            //                 direction = (1, 1);
            //             }
            //
            //             var directionString = direction switch
            //             {
            //                 (1, 0) => "\u2193",
            //                 (0, 1) => "\u2192",
            //                 (0, -1) => "\u2190",
            //                 (-1, 0) => "\u2191",
            //                 (1, 1) => "+",
            //                 _ => "?"
            //             };
            //             Console.Write($"{directionString} ");
            //         }
            //     }
            //
            //     Console.WriteLine();
            // }
            // // // 显示所有的from to
            // //
            // // for (int i = 0; i < MapConfig.rowCount; i++)
            // // {
            // //     var thisLevel = 0;
            // //     for (int line = 0; line < MapConfig.width; line++)
            // //     {
            // //         if (mazeGrid[line, i].level != -1 && mazeGrid[line, i].level != 0)
            // //         {
            // //             thisLevel = mazeGrid[line, i].level;
            // //             goto lable;
            // //         }
            // //     }
            // //     continue;
            // //     lable:
            // //     Console.Write($"level {thisLevel}: \n");
            // //     for (int j = 0; j < MapConfig.width; j++)
            // //     {
            // //         Node node = mazeGrid[j,i];
            // //         if (node.toKey.Any())
            // //         {
            // //             Console.Write(
            // //                 $" - from: {node.mazePos.line}, to: {string.Join(",", node.toKey.Select(node => node.mazePos.line))}\n");
            // //             var to = node.toKey;
            // //         }
            // //     }
            // //     Console.WriteLine();
            // // }
            #endregion
        }
        # region GenerateMazeGrid

        /// <summary>
        /// 在mazeGrid中随机生成高质量的节点，并给出所有生成的keyNodes集合
        /// </summary>
        /// <param name="mazeGrid">待修改的网格对象（将被直接修改）</param>
        /// <param name="keyNodes">输出的关键节点集合</param>
        public virtual void GenerateNodes(Matrix<Node> mazeGrid)
        {
            // 按照有规律的随机生成，结果一定满足如下示意图
            /* key: X->非关键节点, n->n级关键节点
             * 0: X X X X 2 X X 4 5
             * 1: X X 1 X 2 3 X X X
             * 2: 0 0 1 X X X X 4 X
             * 3: X X X X X 3 X X 5
             * 4: X X 1 X 2 X X 4 X
             * remark: 注意空行时机和密度分布
             */
            // [STEP 1] 将mazeGrid用空Node填满
            for (var i = 0; i < mazeGrid.Rows; i++)
            {
                for (var j = 0; j < mazeGrid.Columns; j++)
                {
                    mazeGrid[i, j] = new Node(i, j);
                }
            }

            keyNodes = new HashSet<Node>();
            // [STEP 2] 生成起始点
            // $可配置项$
            int startRow = mazeGrid.Rows / 2;
            startNode = mazeGrid[startRow, 0];
            startNode.level = 0;
            startNode.isKeyNode = true;
            keyNodes.Add(startNode);

            // [STEP 3] 生成其余关键节点
            HashSet<int> keyNodesRowInLastLevel = new() { startRow }; // 记录次级节点的位置，算法会用到
            for (var level = 1; level <= this.MapConfig.finalLevel; level++) // 每个等级依次生成
            {
                // 预制数据
                var nodeRows = GetNodeRows(); // 本次生成的Node在这些row上
                var nodeColumn = 2 * level; // 草，亏我搞了个这么巧妙的公式，被优化掉了
                // var nodeRow = 3 * level / 2 + 1; // 每个等级的关键节点 应该在的列数 满足这个表达式, 公式是找规律搞出来的
                // 生成节点
                foreach (var row in nodeRows)
                {
                    var node = mazeGrid[row, nodeColumn]; // 待设置目标
                    node.level = level;
                    node.isKeyNode = true;
                    keyNodes.Add(node);
                }

                // 重置
                keyNodesRowInLastLevel.Clear();
                keyNodesRowInLastLevel.UnionWith(nodeRows);
                continue;

                List<int> GetNodeRows()
                {
                    List<int> selectedRows = new(); // 返回值
                    HashSet<int> allowedRows = new(); // 不在pool内的值不可能被抽取
                    allowedRows.UnionWith(Enumerable.Range(0, mazeGrid.Rows));
                    // 生成多少个NodeRow $可配置项$
                    var rowsCount = (level == 1 ? 3 : RandomHelper.MazeMap.Range(2, mazeGrid.Rows / 2 + 2));
                    // [STEP 1] 确保row的优质: 在次级节点附近生成节点
                    foreach (var subRow in keyNodesRowInLastLevel.OrderBy(row => RandomHelper.MazeMap.Value))
                        // 对于每个次级节点行,如果附近没有高级节点,则生成一个高级节点
                    {
                        // 选取一个合适的row
                        if (selectedRows.Any(selectedLine => subRow.InRange(selectedLine - 1, selectedLine + 1)))
                            continue; // 本次的subLine已存在优质节点
                        var offsets = Enumerable.Range(-1, 3)
                                .Where(offset => (offset + subRow).InRange(0, this.mazeGrid.Rows-1));
                        var offset = RandomHelper.MazeMap.RandomChoose(offsets);
                        var chosenRow = offset + subRow;
                        // 选中chosenLine
                        selectedRows.Add(chosenRow);
                        allowedRows.Remove(chosenRow);
                    }

                    // [STEP 2] 随机选取其余节点
                    while (selectedRows.Count < rowsCount)
                    {
                        var chosenLine = RandomHelper.MazeMap.RandomChoose(allowedRows);
                        selectedRows.Add(chosenLine);
                        allowedRows.Remove(chosenLine);
                    }

                    // [STEP 3] 返回结果
                    return selectedRows;
                }
            }

            return;
        }

        # endregion

        # region GenerateKeyNodesFromTo

        private struct Branch : IEquatable<Branch>
        {
            public Branch(int from, int to)
            {
                this.From = from;
                this.To = to;
            }

            public int From;
            public int To;
            public bool Equals(Branch other) => From == other.From && To == other.To;

            public override bool Equals(object obj)
            {
                return obj is Branch other && Equals(other);
            }

            public static bool operator ==(Branch left, Branch right) => left.Equals(right);
            public static bool operator !=(Branch left, Branch right) => !left.Equals(right);

            public override int GetHashCode()
            {
                return HashCode.Combine(From, To);
            }
        }

        public virtual void GenerateKeyNodesFromToMain()
        {
            var NodesByLevel = keyNodes.GroupBy(n => n.level);
            // [STEP 1] 设置level0
            {
                var fromNode = NodesByLevel.Single(g => g.Key == 0).First();
                var toGrouping = NodesByLevel.Single(g => g.Key == 1);
                foreach (var toNode in toGrouping)
                {
                    fromNode.toKey.Add(toNode);
                    toNode.fromKey.Add(fromNode);
                }

            }
            // [STEP 2] 设置后续的
            foreach (var level in Enumerable.Range(0, this.MapConfig.finalLevel))
            {
                var fromGrouping = NodesByLevel.Single(g => g.Key == level);
                var toGrouping = NodesByLevel.Single(g => g.Key == level + 1);
                GenerateKeyNodesFromTo(fromGrouping, toGrouping);
            }
        }

        protected virtual void GenerateKeyNodesFromTo(IGrouping<int, Node> fromGrouping,
            IGrouping<int, Node> toGrouping)
        {
            var possibleBranches = new HashSet<Branch>(from i in fromGrouping
                from j in toGrouping
                select new Branch(i.mazePos.row, j.mazePos.row));
            // [STEP 1] 确保from存在to
            foreach (var fromNode in fromGrouping)
            {
                MakeToClosest(fromNode, out var toNode);
                CutBranches(fromNode, toNode);
            }

            // [STEP 2] 确保to存在from
            foreach (var toNode in toGrouping)
            {
                if (!toNode.fromKey.Any())
                {
                    MakeFromClosest(toNode, out var fromNode);
                    CutBranches(fromNode, toNode);
                }
            }

            // [STEP 3] 生成岔路
            var tryBranchCount =
                RandomHelper.MazeMap.Range(0, this.mazeGrid.Rows / 2); // 尝试生成岔路的次数, 生成失败了那也没办法(失败率不算低)
            for (var times = 0; times < tryBranchCount; times++)
            {
                // 提前退出
                if (!possibleBranches.Any()) break;
                // 预制数据
                var newBranch = RandomHelper.MazeMap.RandomChoose(possibleBranches);
                var fromNode = fromGrouping.Single(node => node.mazePos.row == newBranch.From);
                var toNode = toGrouping.Single(node => node.mazePos.row == newBranch.To);
                // 设置fromto
                fromNode.toKey.Add(toNode);
                toNode.fromKey.Add(fromNode);
                //
                CutBranches(fromNode, toNode);
            }

            return;

            void MakeToClosest(Node fromNode, out Node toNode)
            {
                // 预制数据
                var line = fromNode.mazePos.row;
                var closestLine = GetClosestValue(toGrouping.Select(node => node.mazePos.row), line);
                toNode = toGrouping.Single(node => node.mazePos.row == closestLine);
                // 设置fromto
                fromNode.toKey.Add(toNode);
                toNode.fromKey.Add(fromNode);

            }

            void MakeFromClosest(Node toNode, out Node fromNode)
            {
                // 预制数据
                var line = toNode.mazePos.row;
                var closestLine = GetClosestValue(fromGrouping.Select(node => node.mazePos.row), line);
                fromNode = fromGrouping.Single(node => node.mazePos.row == closestLine);
                // 设置fromto
                toNode.fromKey.Add(fromNode);
                fromNode.toKey.Add(toNode);
            }

            void CutBranches(Node fromNode, Node toNode)
            {
                var from = fromNode.mazePos.row;
                var to = toNode.mazePos.row;
                var newBranch = new Branch(from, to);
                foreach (var branchToCut in possibleBranches.ToList()
                             .Where(branchToCut => !isPossibleBranch(newBranch, branchToCut)))
                {
                    possibleBranches.Remove(branchToCut);
                }

                return;

                bool isPossibleBranch(Branch newBranch, Branch branchToCut)
                {
                    // 经研究, 每生成一个route, 满足条件其一的岔路可能生成, 其余皆为不可能
                    // (1) 终点相同
                    // (2) 起点相同
                    // (3) 毫不相干
                    // (1)(2)
                    if (newBranch.From == branchToCut.From && newBranch.To == branchToCut.To) return false;
                    if (newBranch.From == branchToCut.From || newBranch.To == branchToCut.To) return true;
                    // (3) use DeepSeek
                    {
                        // 规范化区间：确保 Start <= End
                        int aStart = Math.Min(newBranch.From, newBranch.To);
                        int aEnd = Math.Max(newBranch.From, newBranch.To);
                        int bStart = Math.Min(branchToCut.From, branchToCut.To);
                        int bEnd = Math.Max(branchToCut.From, branchToCut.To);

                        // 判断区间是否有交集
                        bool hasOverlap = aStart <= bEnd && bStart <= aEnd;

                        // 若有交集返回 false，否则返回 true
                        if (!hasOverlap) return true;
                    }
                    // 失败
                    return false;
                }
            }

            int GetClosestValue(IEnumerable<int> source, int x)
                // use DeepSeek
            {
                if (source == null || !source.Any())
                    throw new ArgumentException("Source collection cannot be null or empty.");

                // 1. 计算每个元素与x的绝对差值，并记录原始值
                var differences = source
                    .Select(num => new { Number = num, Difference = Math.Abs(num - x) })
                    .ToList();

                // 2. 找到最小差值
                var minDifference = differences.Min(item => item.Difference);

                // 3. 筛选出所有差值等于最小差值的候选值
                var candidates = differences
                    .Where(item => item.Difference == minDifference)
                    .Select(item => item.Number)
                    .ToList();

                // 4. 随机选择一个候选值
                return RandomHelper.MazeMap.RandomChoose(candidates);
            }
        }

        #endregion

        #region GenerateRoute

        public void GenerateRoute()
        {
            var mazeGrid = this.mazeGrid;
            // 得益于先前的算法，此处不需要考虑某些情况
            foreach (var startNode in keyNodes)
            {
                // 最终关没有往后的路径
                if (startNode.level == this.MapConfig.finalLevel) continue;
                // 预制数据
                var startLevel = startNode.level;

                // [STEP 1] 往右走
                {
                    var toNode = mazeGrid.Get(startNode.mazePos + (0, 1));
                    SetFromTo(startNode, toNode, startNode);
                }
                // [STEP 2] 纵向走
                {
                    // 依次生成每个toKey的路径
                    foreach (var endNode in startNode.toKey)
                    {
                        {
                            Node rightNode = mazeGrid.Get(startNode.mazePos + (0, 1));
                            startNode.toKeyRoute[endNode] = new List<Node>() { rightNode };
                        }
                        var routeCol = startNode.mazePos.col + 1; // startNode右边一row的位置。以下遍历需要用到
                        var moveDirection =
                            endNode.mazePos.row > startNode.mazePos.row ? 1 : -1; // 无需考虑相等的情况，后面的代码能自动处理
                        // startNode是起点，endNode是终点，遍历其中的所有点
                        foreach (var currentRow in Enumerable.Range(
                                     Math.Min(startNode.mazePos.row, endNode.mazePos.row),
                                     Math.Abs(startNode.mazePos.row - endNode.mazePos.row) +
                                     1)) // 这个Enumerable用的好啊 (我一开心就写一堆逆天注释，难绷)
                        {
                            if (currentRow != endNode.mazePos.row)
                                // 继续走
                            {
                                var fromNode = mazeGrid[currentRow, routeCol];
                                var toNode = mazeGrid.Get(fromNode.mazePos + (moveDirection, 0));
                                SetFromTo(fromNode, toNode, startNode, endNode);
                                startNode.toKeyRoute[endNode].Add(toNode);
                            }
                            else
                                // 走到了
                            {
                                var fromNode = mazeGrid[currentRow, routeCol];
                                var toNode = mazeGrid.Get(fromNode.mazePos + (0, 1));
                                SetFromTo(fromNode, toNode, startNode, endNode);
                                startNode.toKeyRoute[endNode].Add(toNode);
                            }
                        }
                    }
                }
                continue;

                // 设置fromNode的to & toNode的from
                void SetFromTo(Node fromNode, Node toNode, Node startNode, Node endNode = null)
                {
                    if (fromNode.isKeyNode)
                    {
                        fromNode.toNode.Add(toNode);
                        fromNode.level = startNode.level;
                        //
                        toNode.fromNode.Add(fromNode);
                        toNode.fromKey.Add(startNode);
                    }
                    else
                    {
                        if (endNode == null)
                        {
                            throw new ArgumentException("未设置endNode");
                        }

                        fromNode.level = startNode.level;
                        fromNode.toNode.Add(toNode);
                        fromNode.toKey.Add(endNode);
                        //
                        toNode.fromNode.Add(fromNode);
                        toNode.fromKey.Add(startNode);
                    }
                }
            }

        }

        #endregion

        #region BetterRoutes

        public void BetterRoutes()
        {

        }


        #endregion

        # region GenerateSpots

        public void GenerateSpots()
        {
            // [STEP 1] 获取nodesWithLevel
            List<Node> nodesWithLevel = new();
            foreach (var keyNode in keyNodes)
            {
                if (keyNode.level == 0) continue;
                List<Node> possibleNodesToGenerateLevel = new();
                possibleNodesToGenerateLevel.Add(keyNode);
                nodesWithLevel.Add(RandomHelper.MazeMap.RandomChoose(possibleNodesToGenerateLevel));
            }
            // [STEP 2] 设置node的数据
            foreach (var node in nodesWithLevel)
            {
                node.carrySpot = true;
            }
        }

        #endregion

        #region SetPlayerData

        private void SetPlayerData()
        {
            // [STEP 1] 获取passedNodes
            List<Node> passedNodes = new();
            foreach (var node in keyNodes)
            {
                if (MazeMapSaveData.passSpotIds.Contains(node.id))
                {
                    passedNodes.Add(node);
                }
            }
            passedNodes.Sort((a, b) => a.level.CompareTo(b.level));
            //
            passSpotNodes = passedNodes;
        }
        

        #endregion
    }
}