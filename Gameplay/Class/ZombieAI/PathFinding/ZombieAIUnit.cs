using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Core;
using TPL.PVZR.Core.PriorityQueue;
using TPL.PVZR.Gameplay.Class.ZombieAI.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.PathFinding
{
    /* 必读
     * 普通vertex的附近结点有且仅有两个
     * key则有任意个
     */
    /// <summary>
    /// 用于僵尸AI的计算单元
    /// </summary>
    public class ZombieAIUnit : ICanGetModel, IZombieAIUnit
    {
        #region IZombieAIUnit 接口实现

        public void InitializeFromMap()
        {
            BakeFromMap();
        }

        // 获取路径
        public IPath FindPath(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            return _pathManager.GetOnePath(startVertex, endVertex, aiTendency);
        }

        public Vertex GetVertex(int x, int y)
        {
            return mapMatrix[x, y] ?? throw new ArgumentException("该位置不存在结点");
        }

        public ZombieAIUnit()
        {
            _pathManager = new PathManager(this);
        }

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
        private void BakeFromMap()
        {
            // !!== 注释有重要的调试代码，请勿删除 ==!!
            var _LevelModel = this.GetModel<ILevelModel>();
            var CellGrid = _LevelModel.CellGrid;

            // 初始化数据结构
            mapMatrix = new Matrix<Vertex>(_LevelModel.MapConfig.size.x, _LevelModel.MapConfig.size.y);
            vertices = new List<Vertex>();
            keyVertices = new List<Vertex>();
            adjacencyList = new Dictionary<Vertex, List<IEdge>>();
            keyAdjacencyList = new Dictionary<Vertex, List<IKeyEdge>>();

            // [STEP 1] 记录所有顶点
            RecordAllVertices();

            // [STEP 2] 构建邻接表
            foreach (var vertex in vertices)
            {
                BuildAdjacencyList(vertex);
            }

            // [STEP 3] 标记关键顶点
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
            // foreach (var doubledKey in keyVertices.GroupBy(keyVertex => keyVertex).Where(group => group.Count() > 1))
            // {
            //     $"发现重复的关键结点({doubledKey.First().x},{doubledKey.First().y}), 数量是{doubledKey.Count()}, 已成功去重".LogError();
            // }

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
                var keyEdges = GetKeyEdgesToAdjacentKeyVertices(keyVertex);
                keyAdjacencyList[keyVertex] = keyEdges;
                SetClusterCache(keyVertex, keyEdges);
            }

            void SetClusterCache(Vertex keyVertex, List<IKeyEdge> keyEdges)
            {
                // keyVertex的Cluster
                var closest = keyEdges.OrderBy(keyEdge => keyEdge.Weight(AITendency.Default)).First().To;
                _clusterCache[keyVertex] = new Cluster(keyVertex, closest);

                // 每个keyEdge的vertex的Cluster
                foreach (var keyEdge in keyEdges)
                {
                    var includeVertices = keyEdge.IncludeVertices();
                    // 中间没有结点 || 中间结点都被设置过了
                    if (includeVertices.Count == 2 || _clusterCache.ContainsKey(includeVertices[1])) continue;

                    var cluster = new Cluster(includeVertices.First(), includeVertices.Last());
                    for (int i = 1; i < includeVertices.Count - 1; i++)
                    {
                        _clusterCache[includeVertices[i]] = cluster;
                    }
                }
            }

            #region 内部函数

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
                if (!adjacencyList.ContainsKey(vertex))
                {
                    adjacencyList[vertex] = new List<IEdge>(); // 每个结点都会有个表（即便是空的）
                }

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

                // [TYPE 2] Fall && HumanLadder
                // 这两个是相对的，代码按Fall的视角写（看起来像：写Fall的代码，顺便把HumanLadder实现了）
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
                        if (!adjacencyList.ContainsKey(toVertex)) adjacencyList[toVertex] = new();
                        adjacencyList[toVertex].Add(new Edge(toVertex, vertex, Edge.EdgeType.HumanLadder,
                            allowedPassHeight));
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

            #endregion
        }

        #endregion

        #region 寻路相关

        private IPathManager _pathManager;

        #region 底层函数（不调用KeyAgjecencyList）

        /// <summary>
        /// 找到与vertex相邻的所有关键
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
        /// 找到与vertex到相邻关键节点的Edge。如果出现环，可能会报错(没出现过)。如果vertex.isKey，不会包括自己
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public List<IKeyEdge> GetKeyEdgesToAdjacentKeyVertices(Vertex vertex)
        {
            // 验证输入
            if (!adjacencyList.ContainsKey(vertex)) throw new ArgumentException("Start/End not in graph");

            // 初始化数据结构
            Stack<IKeyEdge> frontier = new Stack<IKeyEdge>();
            List<Vertex> visited = new List<Vertex>();
            List<IKeyEdge> result = new List<IKeyEdge>();

            visited.Add(vertex);
            foreach (var each in adjacencyList[vertex])
            {
                frontier.Push(new KeyEdge(each));
                visited.Add(each.To);
            }

            // 
            while (frontier.Count > 0)
            {
                var current = frontier.Peek();
                // 探索最新的结点
                var v = current.To;
                if (v.isKey)
                {
                    result.Add(current);
                    frontier.Pop();
                }

                // 将临近的结点加入列表
                else
                {
                    var adj = adjacencyList[current.To].Where(edge => !visited.Contains(edge.To)).ToList();
                    {
                        // 异常处理
                        if (adj.Count >= 2)
                        {
                            string exp = "GetKeyEdgesToAdjacentKeyVertices的路径中出现岔路，这是不应该的。";
                            exp += $"源头是({current.From.x},{current.From.y})。";
                            exp += $"正在探索：" + String.Join(",",
                                adj.Select(each => $"({each.From.x},{each.From.y})->({each.To.x},{each.To.y})"));
                            Debug.LogWarning(exp);
                        }
                    }

                    var next = adj.First();
                    visited.Add(next.To);
                    current.AddEdge(next);
                }
            }

            if (!vertex.isKey && result.Count != 2)
            {
                var exp =
                    $"普通结点找到了非2个的keyEdge，我是:({vertex.x},{vertex.y}), To: {String.Join(",", result.Select(each => $"({each.To.x}, {each.To.y})"))}";
                throw new Exception(exp);
            }

            return result;
        }

        #endregion

        #region 函数

        private Dictionary<Vertex, Cluster> _clusterCache = new Dictionary<Vertex, Cluster>();

        public Cluster GetCluster(Vertex vertex)
        {
            if (_clusterCache.TryGetValue(vertex, out var cluster))
            {
                return cluster;
            }

            throw new NotImplementedException($"_clusterCache未包含所有Vertex导致出错，这个Vertex是({vertex.x},{vertex.y})");
        }

        #endregion

        #endregion

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

        public void LogAllKeyAdjacencyList()
        {
            foreach (var keyVertex in keyVertices)
            {
                string output = $"KeyVertex: ({keyVertex.x},{keyVertex.y})->";
                foreach (var keyEdge in keyAdjacencyList[keyVertex])
                {
                    output += $"({keyEdge.To.x},{keyEdge.To.y}), ";
                }

                output.LogInfo();
            }
        }

        public void LogThePath(IPath path)
        {
            string output = "";
            foreach (var edge in path.keyEdges)
            {
                output += $"({edge.From.x},{edge.From.y}) -> ({edge.To.x},{edge.To.y}) && ";
            }

            output.LogInfo();
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