using System;
using System.Collections.Generic;
using System.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class.ZombieAI.New.Others;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit
{
    // 用于僵尸AI的计算单元
    public class ZombieAIUnit : ICanGetModel, IZombieAIUnit
    {
        #region IZombieAIUnit

        // 初始化
        public void InitializeFromMap()
        {
            BakeFromMap();
        }

        // 获取路径
        public Path FindPath(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            return _pathCache.GetPathAllowUnreachable(startVertex, endVertex, aiTendency);
        }

        public ZombieAIUnit()
        {
            _pathCache = new PathManager(this);
        }

        #region 1

        // // 预制数据
        //     var tempKeyAdjacencyList = new Dictionary<Vertex, List<KeyEdge>>(keyAdjacencyList);
        //     tempKeyAdjacencyList.Add(startVertex);
        //
        //     var excludeVertices = new List<Vertex>();
        //     var currentEndVertex = endVertex;
        //     // 获取可用的allowedPaths
        //     List<Path> allowedPaths = null;
        //     while (true)
        //     {
        //         allowedPaths = GetAllowedPaths();
        //         if (allowedPaths.Any()) break;
        //         else // 没有通路
        //         {
        //             currentEndVertex = GetClosestKeyVertex(currentEndVertex, excludeVertices);
        //             excludeVertices.Add(currentEndVertex);
        //         }
        //     }
        //
        //     // 返回值
        //     return aiTendency.ChooseOnePath(allowedPaths);
        //
        //     List<Path> GetAllowedPaths()
        //     {
        //         var allPaths = GetAllPaths(startVertex, currentEndVertex, tempMapMatrix);
        //         aiTendency.ApplyFilter(allPaths);
        //         return allPaths;
        //     }
        // }

        #endregion

        #endregion

        #region 数据结构

        // 必要数据
        private Matrix<Vertex> mapMatrix;

        public Dictionary<Vertex, List<IEdge>> adjacencyList;
        public Dictionary<Vertex, List<IKeyEdge>> keyAdjacencyList;

        // 为了性能而记录的数据
        private List<Vertex> vertices;
        private List<Vertex> keyVertices;

        #endregion

        #region 烘焙相关

        /// <summary>
        /// 根据当前的地图烘焙数据结构
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BakeFromMap()
        {
            // !!== 注释有重要的调试代码，请勿删除 ==!!
            var _LevelModel = this.GetModel<ILevelModel>();
            var CellGrid = _LevelModel.CellGrid;
            // 初始化
            mapMatrix = new Matrix<Vertex>(_LevelModel.MapConfig.size.x, _LevelModel.MapConfig.size.y);
            vertices = new List<Vertex>();
            keyVertices = new List<Vertex>();
            adjacencyList = new Dictionary<Vertex, List<IEdge>>();
            keyAdjacencyList = new Dictionary<Vertex, List<IKeyEdge>>();

            // [STEP 1] 记录所有结点
            RecordAllVertices();
            // [STEP 2] 遍历结点，构建所有边
            foreach (var vertex in vertices)
            {
                BuildAdjacencyList(vertex);
            }

            // [STEP 3] 遍历结点，设置关键结点
            foreach (var vertex in vertices)
            {
                if (ShouldBeKeyVertex(vertex))
                {
                    vertex.isKey = true;
                    keyVertices.Add(vertex);
                }
            }

            #region 一大坨调试代码（包含keyVertex去重，前面的代码似乎有点问题）

            // foreach (var doubledVertex in vertices.Where(v => adjacencyList[v].Count > 1))
            // {
            //     string output = $"({doubledVertex.x}, {doubledVertex.y})连接了多个结点：";
            //     foreach (var edge in adjacencyList[doubledVertex])
            //     {
            //         output += $" ({edge.To.x}, {edge.To.y})";
            //     }
            //     output.LogInfo();
            // }
            foreach (var doubledKey in keyVertices.GroupBy(keyVertex => keyVertex).Where(group => group.Count() > 1))
            {
                $"发现重复的关键结点({doubledKey.First().x},{doubledKey.First().y}), 数量是{doubledKey.Count()}, 已成功去重".LogError();
            }

            keyVertices = keyVertices.Distinct().ToList();
            // foreach (var doubledKey in keyVertices.GroupBy(keyVertex => keyVertex).Where(group => group.Count() > 1))
            // {
            //     $"????去重后发现重复的关键结点({doubledKey.First().x},{doubledKey.First().y}), 数量是{doubledKey.Count()}, 已成功去重"
            //         .LogError();
            // }

            #endregion

            // [STEP 4] 连接关键结点
            foreach (var keyVertex in keyVertices)
            {
                ConnectToAdjacentKeyVertices(keyVertex);
            }

            #region 一大坨调试代码

            // foreach (var keyVertex in keyVertices)
            // {
            //     foreach (var keyEdge in keyAdjacencyList[keyVertex])
            //     {
            //         $"key, from:{keyEdge.From.x}, {keyEdge.From.y}, to:{keyEdge.To.x}, {keyEdge.To.y}, edgeType: {keyEdge.edgeType}, height: {keyEdge.minAllowedPassHeight}"
            //             .LogInfo();
            //     }
            // }
            // $"找到关键结点数量{keyVertices.Count}".LogInfo();

            #endregion


            return;

            #region 函数（很大一坨，不建议展开）

            void RecordAllVertices()
            {
                for (int x = 1; x < _LevelModel.MapConfig.size.x - 1; x++)
                {
                    // y = 0 一定是基岩，没必要考虑 (其实还有其他无需考虑的值，但只有这个会报错所以。。)
                    for (int y = 1; y < _LevelModel.MapConfig.size.y - 1; y++)
                    {
                        // ↓ 一些稍微复杂的逻辑
                        if (CellGrid[x, y].IsEmpty && CellGrid[x, y - 1].IsStableTile)
                        {
                            if (CellGrid[x, y + 1].IsEmpty)
                            {
                                var newVertex = new Vertex(x, y, AllowedPassHeight.TwoAndMore);
                                mapMatrix[x, y] = newVertex;
                                vertices.Add(newVertex);
                            }
                            else
                            {
                                var newVertex = new Vertex(x, y, AllowedPassHeight.One);
                                mapMatrix[x, y] = newVertex;
                                vertices.Add(newVertex);
                            }
                        }
                    }
                }
            }

            void BuildAdjacencyList(Vertex vertex)
            {
                adjacencyList[vertex] = new List<Edge>(); // 每个结点都会有个表（即便是空的）
                var adjacentEdges = adjacencyList[vertex];
                // [TYPE 1] WalkJump
                foreach (var adjacentVertex in mapMatrix.GetNeighbors(vertex.x, vertex.y))
                {
                    if (adjacentVertex is null || ReferenceEquals(vertex, adjacentVertex)) continue;
                    if (CanWalkJumpTo(vertex, adjacentVertex))
                    {
                        adjacentEdges.Add(new Edge(vertex, adjacentVertex, Edge.EdgeType.WalkJump));
                        // $"BuildWalkJump: from({vertex.x},{vertex.y}), to({adjacentVertex.x},{adjacentVertex.y})"
                        //     .LogInfo();
                    }
                }

                // [TYPE 2] Fall
                foreach (int x in new int[2] { vertex.x - 1, vertex.x + 1 }) // 检测左右两格
                {
                    // [STEP 0] 排除不可能Fall的情况
                    if ((CellGrid[x, vertex.y].IsStableTile) // 不可能能往下走
                        || mapMatrix[x, vertex.y] is not null || mapMatrix[x, vertex.y + 1] is not null ||
                        mapMatrix[x, vertex.y - 1] is not null // 不用Fall，WalkJump就行了
                       ) continue;

                    // [STEP 1] 获取通路高度
                    AllowedPassHeight allowedPassHeight;
                    if (CellGrid[x, vertex.y].IsEmpty && CellGrid[x, vertex.y + 1].IsStableTile)
                    {
                        allowedPassHeight = AllowedPassHeight.One;
                    }
                    else
                    {
                        allowedPassHeight = AllowedPassHeight.TwoAndMore;
                    }

                    // [STEP 2] 向下遍历
                    for (int y = vertex.y - 2; y > 0; y--)
                        // 可以直接从WalkJump无法考虑到的高度开始（虽然似乎有点冒险）
                    {
                        if (mapMatrix[x, y] is null) continue;
                        var toVertex = mapMatrix[x, y];
                        // 设置边
                        adjacentEdges.Add(new Edge(vertex, toVertex, Edge.EdgeType.Fall, allowedPassHeight));
                        // 设置key
                        vertex.isKey = true;
                        toVertex.isKey = true;
                        keyVertices.Add(vertex);
                        keyVertices.Add(toVertex);
                        break;
                    }
                }

                bool CanWalkJumpTo(Vertex a, Vertex b)
                {
                    if (a.x == 17 && a.y == 15)
                    {
                        $"与({b.x},{b.y})的比较，isNearBy:{isNearBy()}, haveNoFrontier:{haveNoFrontier()}, 坏的cell坐标:(18,15)({(a.y < b.y ? a.y : b.y)},{Mathf.Max(a.y, b.y)})"
                            .LogInfo();
                    }

                    return isNearBy() && haveNoFrontier();

                    /* 需要满足所有条件
                     * Case 1: 在附近
                     * Case 2: 之间无障碍阻隔
                     */
                    bool isNearBy()
                    {
                        return Mathf.Abs(a.x - b.x) <= 1 && Mathf.Abs(a.y - b.y) <= 1;
                    }

                    bool haveNoFrontier()
                    {
                        if (a.y == b.y) return true;
                        int xToTest, yToTest;
                        xToTest = a.y < b.y ? a.x : b.x; // 较矮的那格
                        yToTest = Mathf.Max(a.y, b.y); // 较高的那个
                        return CellGrid[xToTest, yToTest].IsEmpty;
                    }
                }
            }

            bool ShouldBeKeyVertex(Vertex vertex)
            {
                var edges = adjacencyList[vertex];
                // 特殊情况处理
                // 1 - 无任何相邻结点
                if (edges.Count == 0) return false;
                // 核心代码
                // 1 - WalkJump，只能往一个方向走（是边界）
                if (edges.Count == 1 && edges.First().edgeType is Edge.EdgeType.WalkJump) return true;
                // 2 - 可以Fall/Climb到其他地方
                if (edges.Any(edge => edge.edgeType is Edge.EdgeType.Fall)) return true;
                return false;
            }

            void ConnectToAdjacentKeyVertices(Vertex keyVertex)
            {
                keyAdjacencyList[keyVertex] = new List<KeyEdge>();
                // 初始为当前keyVertex相连的所有边
                var toDiscover = new Queue<KeyEdge>(adjacencyList[keyVertex].Select(edge => new KeyEdge(edge)));
                var haveDiscovered = new List<Vertex>() { keyVertex };
                // 探索所有连接的结点
                int tryCount = 0;
                while (toDiscover.Any())
                {
                    if (tryCount++ > 1000) throw new Exception("单个关键结点遍历超过1000次");
                    var keyEdge = toDiscover.Dequeue();
                    if (keyEdge.To.isKey) // 设置终点
                    {
                        // "set end".LogInfo();
                        keyAdjacencyList[keyVertex].Add(keyEdge);
                    }
                    else // 继续探索
                    {
                        var vertexToDiscover = keyEdge.To;
                        // if (adjacencyList[vertexToDiscover].Count >= 3) $"非关键结点有{adjacencyList[vertexToDiscover].Count}个To".LogError();
                        // 探索vertex的所有邻接边
                        foreach (var edgeToDiscover in adjacencyList[vertexToDiscover])
                        {
                            if (haveDiscovered.Contains(edgeToDiscover.To)) continue;
                            haveDiscovered.Add(edgeToDiscover.To);
                            toDiscover.Enqueue(new KeyEdge(keyEdge).AddEdge(edgeToDiscover));
                            // $"探索：from:{vertexToDiscover.x},{vertexToDiscover.y}, to:{edgeToDiscover.To.x},{edgeToDiscover.To.y}".LogInfo();
                        }
                    }
                }
            }

            #endregion
        }

        #endregion


        #region 寻路相关

        private IPathManager _pathCache;


        /// <summary>
        /// 找到与vertex相邻的所有关键节点。
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="includeSelf">为true时，如果vertex.isKey，返回它自己</param>
        /// <returns></returns>
        public List<Vertex> GetAdjacentKeyVertices(Vertex vertex, bool includeSelf = false)
        {
            // 如果vertex.isKey，返回它自己
            if (includeSelf && vertex.isKey) return new List<Vertex>() { vertex };
            // 利用BFS搜索附近的关键结点
            HashSet<Vertex> visited = new HashSet<Vertex>();
            Queue<Vertex> queue = new Queue<Vertex>();
            List<Vertex> result = new List<Vertex>();

            visited.Add(vertex);
            queue.Enqueue(vertex);

            while (queue.TryDequeue(out Vertex current))
            {
                if (current.isKey)
                {
                    result.Add(current);
                }
                else
                {
                    foreach (var edge in adjacencyList.GetValueOrDefault(current, new List<IEdge>()))
                    {
                        if (!visited.Contains(edge.To))
                        {
                            visited.Add(edge.To);
                            queue.Enqueue(edge.To);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 找到与vertex到相邻关键节点的Edge。如果出现环，允许起点/终点是自己
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public List<IKeyEdge> GetKeyEdgesToAdjacentKeyVertices(Vertex vertex)
        {
            // == 利用BFS搜索附近的关键结点
            HashSet<Vertex> visited = new HashSet<Vertex>();
            Queue<IKeyEdge> queue = new Queue<IKeyEdge>();
            List<IKeyEdge> result = new List<IKeyEdge>();

            visited.Add(vertex);
            foreach (var keyEdge in adjacencyList[vertex].Select(edge => new KeyEdge(edge)))
            {
                queue.Enqueue(keyEdge);
            }

            //
            while (queue.Count > 0)
            {
                var keyEdge = queue.Dequeue();
                foreach (var edge in adjacencyList[keyEdge.To])
                {
                    if (edge.To.isKey)
                    {
                        // visited.Add(edge.To); 不加也不会出bug
                        result.Add(new KeyEdge(keyEdge).AddEdge(edge));
                    }
                    else if (!visited.Contains(edge.To))
                    {
                        visited.Add(edge.To);
                        queue.Enqueue(new KeyEdge(keyEdge).AddEdge(edge));
                    }
                }
            }

            if (result.Count == 1) "立项以来首次出现1".LogError();
            if (result.Count is not 1 or 2) throw new Exception();
            return result;
        }

        #region 调试相关

        public void DisplayTheMap()
        {
            var tilemapGroup = ReferenceModel.Get.TilemapGroup;
            var OneHeight = ResLoader.Allocate().LoadSync<Tile>(Oneheight_asset.OneHeight);
            var TwoHeight = ResLoader.Allocate().LoadSync<Tile>(Twoheight_asset.TwoHeight);
            var Key = ResLoader.Allocate().LoadSync<Tile>(Key_asset.Key);
            foreach (var each in vertices)
            {
                if (each.isKey)
                {
                    tilemapGroup.Test.SetTile(new Vector3Int(each.x, each.y, 0), Key);
                }
                else
                {
                    if (each.AllowedPassHeight is AllowedPassHeight.One)
                    {
                        tilemapGroup.Test.SetTile(new Vector3Int(each.x, each.y, 0), OneHeight);
                    }
                    else
                    {
                        tilemapGroup.Test.SetTile(new Vector3Int(each.x, each.y, 0), TwoHeight);
                    }
                }
            }
        }

        #endregion


        #region 已废弃内容

        // 烘焙

        // /// <summary>
        // /// 返回一个临时的mapMatrix，唯一的修改是：添加了两个keyVertex
        // /// </summary>
        // /// <param name="VertexA"></param>
        // /// <param name="VertexB"></param>
        // /// <returns></returns>
        // private Matrix<Vertex> BakeTempAddTwoKeyVertices(Vertex VertexA, Vertex VertexB)
        // {
        //     var tempMatrix = mapMatrix.Clone as Matrix<Vertex>;
        // }

        #endregion

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}