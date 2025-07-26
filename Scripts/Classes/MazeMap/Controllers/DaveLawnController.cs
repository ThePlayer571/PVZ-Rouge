using System;
using System.Collections.Generic;
using System.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers.Others.MazeMap;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Classes.MazeMap.Controllers
{
    public class DaveLawnController : MazeMapController
    {
        #region Matrix数据结构生成

        protected override void ValidateMazeMapData()
        {
            /* - Rules:
             * ColCount in [5, ..)
             * RowCount in [6, ..) and RowCount 是 奇数
             * TotalLevelCount == (RowCount - 1) / 2
             */
            if (MazeMapData == null) throw new ArgumentNullException(nameof(MazeMapData), "MazeMapData 不能为空");

            if (MazeMapData.ColCount < 5)
                throw new Exception($"暂不支持的ColCount: {MazeMapData.ColCount}，必须 >= 5");

            if ((MazeMapData.RowCount - 1) % 2 != 0 || MazeMapData.RowCount < 6)
                throw new Exception($"暂不支持的RowCount: {MazeMapData.RowCount}，必须 >= 6 且 为奇数");

            if (MazeMapData.TotalStageCount != (MazeMapData.RowCount - 1) / 2)
                throw new Exception(
                    $"暂不支持的TotalSpotCount: {MazeMapData.TotalStageCount}，应满足表达式: TotalLevelCount = ( rowCount - 1 ) / 2 = {((MazeMapData.RowCount - 1) / 2)}");
        }


        // 维护的数据结构
        private Dictionary<int, List<int>> _levelKeyNodes = new Dictionary<int, List<int>>();

        public override void GenerateMazeMatrix()
        {
            // 异常处理
            ValidateMazeMapData();

            // 初始化
            mazeMatrix = new Matrix<Node>(MazeMapData.RowCount, MazeMapData.ColCount);
            mazeMatrix.Fill((i, j) => new Node(i, j));
            adjacencyList = new Dictionary<Node, List<Node>>();

            //
            MarkKeyNodes();
            BridgeKeyNodes();


            #region 内部函数

            void MarkKeyNodes()
            {
                // [1. ] 起始点
                {
                    // 起始点在第0行中间
                    var col = mazeMatrix.Columns / 2;
                    startNode = mazeMatrix[0, col];
                    startNode.isKey = true;
                    startNode.level = 0; // 起始点的 level 为 0
                    _levelKeyNodes.Add(0, new List<int> { col });
                }
                // [2. ] 第一关
                {
                    var level = 1;
                    var row = 2;
                    var levelCountRange = MazeMapData.GetSpotCountRangeOfStage(level);
                    var levelCount = Random.Range(levelCountRange.x, levelCountRange.y + 1);
                    List<int> keyNodes = new List<int>();
                    // 随机选择levelCount个列
                    List<int> chosen = Random
                        .RandomSubset(Enumerable.Range(0, MazeMapData.ColCount), levelCount).ToList();
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
                for (int level = 2; level < MazeMapData.TotalStageCount; level++)
                {
                    // 预制数据
                    var row = level * 2;
                    var levelCountRange = MazeMapData.GetSpotCountRangeOfStage(level);
                    var levelCount = Random.Range(levelCountRange.x, levelCountRange.y + 1);
                    List<int> lastCols = _levelKeyNodes[level - 1];
                    List<int> keyNodes = new List<int>();

                    // 保证每个上一关的关键节点附近有本关的关键节点
                    foreach (var lastCol in lastCols)
                    {
                        // 取上一关关键节点附近的列（-1, 0, +1）
                        var _ = Enumerable.Range(lastCol - 1, 3)
                            .Where(col => col >= 0 && col < MazeMapData.ColCount)
                            .ToList();
                        // 如果这些列都没有被标记为关键节点，则随机选一个作为关键节点
                        if (_.All(col => !mazeMatrix[row, col].isKey))
                        {
                            var col = Random.RandomChoose(_);
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
                        var remainingCols = Enumerable.Range(0, MazeMapData.ColCount)
                            .Where(c => !mazeMatrix[row, c].isKey)
                            .ToList();
                        var additionalCols =
                            Random.RandomSubset(remainingCols, levelCount - keyNodes.Count);
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
                    var level = MazeMapData.TotalStageCount;
                    var row = level * 2;
                    List<int> keyNodes = new List<int>();
                    var col = Random.RandomChoose(Enumerable.Range(0, MazeMapData.ColCount));
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
            void BridgeKeyNodes()
            {
                keyAdjacencyList = new Dictionary<Node, List<Node>>();

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
                                var oldComponent =
                                    connectedComponents.First(list => list.Any(col => route.Contains(col)));
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
                        if (!from.isKey || from.level == -1) "????".LogWarning();
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
                            from.level = level;
                            to.level = level;
                            ConnectNodes(from, to);
                        }
                    }

                    // 生成keyAdjacencyList
                    foreach (var component in connectedComponents)
                    {
                        var toNodes = toNodesCol.Where(col => component.Contains(col))
                            .Select(col => mazeMatrix[toRow, col]).ToList();
                        var fromNodes = fromNodesCol.Where(col => component.Contains(col))
                            .Select(col => mazeMatrix[row, col]).ToList();

                        foreach (var fromNode in fromNodes)
                        {
                            keyAdjacencyList[fromNode] = toNodes;
                        }
                    }
                }
            }

            void ConnectNodes(Node a, Node b)
            {
                if (a == null || b == null) throw new ArgumentNullException("节点不能为空");
                // 只允许直线相邻节点
                if (!((a.x == b.x && Math.Abs(a.y - b.y) == 1) || (a.y == b.y && Math.Abs(a.x - b.x) == 1)))
                    throw new ArgumentException("ConnectNodes: 仅支持直线相邻节点");

                if (!adjacencyList.ContainsKey(a))
                    adjacencyList[a] = new List<Node>();
                if (!adjacencyList[a].Contains(b))
                    adjacencyList[a].Add(b);

                if (!adjacencyList.ContainsKey(b))
                    adjacencyList[b] = new List<Node>();
                if (!adjacencyList[b].Contains(a))
                    adjacencyList[b].Add(a);
            }

            #endregion
        }

        public override Vector2Int MatrixToTilemapPosition(Vector2Int matrixPos)
        {
            return matrixPos * 3;
        }

        #endregion

        #region 场景中GameObject

        protected override void SetUpTiles()
        {
            // 准备
            var dirtTile =
                resLoader.LoadSync<Tile>(Mazemapdirttile_asset.BundleName, Mazemapdirttile_asset.MazeMapDirtTile);
            var grassTile = resLoader.LoadSync<Tile>(Mazemapgrasstile_asset.BundleName,
                Mazemapgrasstile_asset.MazeMapGrassTile);
            var stoneTile = resLoader.LoadSync<Tile>(Mazemapstonetile_asset.BundleName,
                Mazemapstonetile_asset.MazeMapStoneTile);
            var GroundTilemap = MazeMapTilemapController.Instance.Ground;
            Matrix<Tile> tileMatrix = new(mazeMatrix.Rows * 3 - 2, mazeMatrix.Columns * 3 - 2);
            //
            tileMatrix.Fill(grassTile);
            //
            foreach (var node in mazeMatrix)
            {
                if (node.level == -1) continue;
                var startPos = new Vector2Int(node.x, node.y) * 3;
                var neighbors = adjacencyList[node];
                foreach (var neighbor in neighbors)
                {
                    var endPos = new Vector2Int(neighbor.x, neighbor.y) * 3;
                    tileMatrix.Fill(startPos, endPos, dirtTile);
                }
            }

            // 填充石头
            foreach (var node in mazeMatrix)
            {
                if (node.isKey)
                {
                    tileMatrix[3 * node.x, 3 * node.y] = stoneTile;
                }
            }

            // 修正：Tilemap 需要列优先的数组，BoundsInt 的 size.x=列数，size.y=行数
            Tile[] ToArrayByColumn(Matrix<Tile> matrix)
            {
                var arr = new Tile[matrix.Rows * matrix.Columns];
                int idx = 0;
                for (int y = 0; y < matrix.Columns; y++)
                {
                    for (int x = 0; x < matrix.Rows; x++)
                    {
                        arr[idx++] = matrix[x, y];
                    }
                }

                return arr;
            }

            var bounds = new BoundsInt(0, 0, 0, tileMatrix.Rows, tileMatrix.Columns, 1);
            GroundTilemap.SetTilesBlock(bounds, ToArrayByColumn(tileMatrix));
        }

        protected override void SetUpTombs()
        {
            var tombstonePrefab =
                resLoader.LoadSync<GameObject>(Tombstone_prefab.BundleName, Tombstone_prefab.Tombstone);
            foreach (var node in mazeMatrix)
            {
                if (node == null || !node.isKey || node == startNode) continue;
                var tombstone = tombstonePrefab.Instantiate().GetComponent<TombstoneController>();
                tombstone.Initialize(node.Position);
            }

            // ($"Pass: {String.Join(",", MazeMapData.PassedRoute)}\n" +
            //  $"Discoverd: {String.Join(",", MazeMapData.DiscoveredTombs.Select(tomb => tomb.Position))}\n").LogInfo();
        }

        protected override void DisplayFinalObject()
        {
            var finalMatrixPos = new Vector2Int(mazeMatrix.Rows + 1, mazeMatrix.Columns / 2);
            var finalTilemapPos = MatrixToTilemapPosition(finalMatrixPos);
            var finalWorldPos =
                MazeMapTilemapController.Instance.Ground.CellToWorld(new Vector3Int(finalTilemapPos.x,
                    finalTilemapPos.y, 0));
            var finalObject =
                resLoader.LoadSync<GameObject>(Finalobject_prefab.BundleName, Finalobject_prefab.FinalObject)
                    .Instantiate(finalWorldPos, Quaternion.identity);
        }

        #endregion


        public DaveLawnController(IMazeMapData mazeMapData) : base(mazeMapData)
        {
        }
    }
}