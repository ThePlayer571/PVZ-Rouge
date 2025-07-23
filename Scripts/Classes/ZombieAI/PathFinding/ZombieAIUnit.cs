using System;
using System.Collections.Generic;
using System.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.ZombieAI.Class;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others.LevelScene;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Classes.ZombieAI.PathFinding
{
    /* 必读
     * 普通vertex的附近结点有且仅有两个
     * key则有任意个
     */
    /// <summary>
    /// 用于僵尸AI的计算单元
    /// </summary>
    public class ZombieAIUnit : IZombieAIUnit
    {
        #region IZombieAIUnit 接口实现

        public void InitializeFrom(Matrix<Cell> levelMatrix)
        {
            BakeFrom(levelMatrix);
        }

        // 获取路径
        public IZombiePath FindPath(Vector2Int start, Vector2Int end, AITendency aiTendency)
        {
            var startVertex = this.GetVertex(start.x, start.y);
            var endVertex = this.GetVertex(end.x, end.y);

            var path = _pathManager.GetOnePath(startVertex, endVertex, aiTendency);
            // LogThePath(path);
            return new ZombiePath(path);
        }

        public Cluster GetClusterSafely(Vector2Int pos)
        {
            Vertex vertex = mapMatrix[pos.x, pos.y];
            if (vertex == null) return null;
            else return GetCluster(vertex);
        }

        public Vertex GetVertexSafely(Vector2Int pos)
        {
            Vertex vertex = mapMatrix[pos.x, pos.y];
            return vertex;
        }

        public Vertex GetVertex(int x, int y)
        {
            return mapMatrix[x, y] ?? throw new ArgumentException($"该位置不存在结点: ({x},{y})");
        }

        public ZombieAIUnit()
        {
            _pathManager = new PathManager(this);
        }

        #endregion

        #region 数据结构

        // 必要数据
        private Matrix<Vertex> mapMatrix;

        public Dictionary<Vertex, List<Edge>> adjacencyList;
        public Dictionary<Vertex, List<KeyEdge>> keyAdjacencyList;

        // 为了性能而记录的数据
        private List<Vertex> vertices;
        private List<Vertex> keyVertices;

        #endregion

        #region 烘焙相关

        /// <summary>
        /// 根据当前的地图烘焙数据结构
        /// </summary>
        private void BakeFrom(Matrix<Cell> levelMatrix)
        {
            // !!== 注释有重要的调试代码，请勿删除 ==!!

            // 初始化数据结构
            mapMatrix = new Matrix<Vertex>(levelMatrix.Rows, levelMatrix.Columns);
            vertices = new List<Vertex>();
            keyVertices = new List<Vertex>();
            adjacencyList = new Dictionary<Vertex, List<Edge>>();
            keyAdjacencyList = new Dictionary<Vertex, List<KeyEdge>>();

            // [STEP 1] 记录所有顶点
            RecordAllVertices();

            // foreach (var vertex in vertices)
            // {
            //     $"{vertex.Position}, {vertex.VertexType}".LogInfo();
            // }

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

            // foreach (var vertex in vertices)
            // {
            //     string output = $"({vertex.x}, {vertex.y})：";
            //     foreach (var edge in adjacencyList[vertex])
            //     {
            //         output += $" ({edge.To.x}, {edge.To.y}, {edge.moveType})";
            //     }
            //
            //     output.LogInfo();
            // }

            #region 一大坨调试代码

            // foreach (var doubledVertex in vertices.Where(v => adjacencyList[v].Count > 1))
            // {
            //     string output = $"({doubledVertex.x}, {doubledVertex.y})连接了多个结点：";
            //     foreach (var edge in adjacencyList[doubledVertex])
            //     {
            //         output += $" ({edge.To.x}, {edge.To.y})";
            //     }
            //
            //     output.LogInfo();
            // }

            // foreach (var doubledKey in keyVertices.GroupBy(keyVertex => keyVertex).Where(group => group.Count() > 1))
            // {
            //     $"发现重复的关键结点({doubledKey.First().x},{doubledKey.First().y}), 数量是{doubledKey.Count()}, 已成功去重".LogError();
            // }
// 以防万一的去重
            keyVertices = keyVertices.Distinct().ToList();
            // foreach (var vertex in keyVertices)
            // {
            //     $"Key: {vertex.Position}, {vertex.VertexType}".LogInfo();
            // }

            #endregion

            // [STEP 4] 连接关键结点
            foreach (var keyVertex in keyVertices)
            {
                var keyEdges = FindKeyEdgesToAdjacentKeyVertices(keyVertex);
                keyAdjacencyList[keyVertex] = keyEdges;
                SetClusterCache(keyVertex, keyEdges);
            }

            void SetClusterCache(Vertex keyVertex, List<KeyEdge> keyEdges)
            {
                // keyVertex的Cluster
                // 两种逻辑：[1] 附近结点中有WalkJump能达到的 -> 和最近的组合成一个Cluster [2] 没有WalkJump能达到的 -> 和自己组合成一个Cluster
                var temp = keyEdges.Where(ke => ke.moveType == MoveType.WalkJump)
                    .OrderBy(keyEdge => keyEdge.Weight(AITendency.Default)).ToList();
                var closest = temp.Count > 0 ? temp.First().To : keyVertex;
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
                for (int x = 1; x < levelMatrix.Rows - 1; x++)
                {
                    // y = 0 一定是基岩，没必要考虑 (其实还有其他无需考虑的值，但只有这个会报错所以。。)
                    for (int y = 1; y < levelMatrix.Columns - 1; y++)
                    {
                        /* case : (x,y).IsEmpty, (x,y-1).IsPlat, (x,y).IsClimbable
                         * 110 -> Plat
                         * 001 -> Ladder
                         * 011 -> PlatWithLadder
                         */
                        bool _1 = levelMatrix[x, y].IsEmpty;
                        bool _2 = levelMatrix[x, y - 1].IsBlock;
                        bool _3 = levelMatrix[x, y].IsClimbable;

                        // ↓ 一些稍微复杂的逻辑
                        if (_1 && _2 && !_3) //Plat
                        {
                            if (_3) $"find _3 at ({x},{y}), {_3}".LogWarning();
                            var allowPassHeight = levelMatrix[x, y + 1].IsEmpty
                                ? AllowedPassHeight.TwoAndMore
                                : AllowedPassHeight.One;
                            var newVertex = new Vertex(x, y, VertexType.Plat, allowPassHeight);
                            mapMatrix[x, y] = newVertex;
                            vertices.Add(newVertex);
                        }
                        else if (!_1 && !_2 && _3) // Ladder
                        {
                            var newVertex = new Vertex(x, y, VertexType.Ladder, AllowedPassHeight.TwoAndMore);
                            mapMatrix[x, y] = newVertex;
                            vertices.Add(newVertex);
                        }
                        else if (!_1 && _2 && _3) // PlatWithLadder
                        {
                            var newVertex = new Vertex(x, y, VertexType.PlatWithLadder, AllowedPassHeight.TwoAndMore);
                            mapMatrix[x, y] = newVertex;
                            vertices.Add(newVertex);
                        }
                    }
                }
            }

            void BuildAdjacencyList(Vertex vertex)
            {
                // #!#!# 不需要检测越界，因为不可能越界
                // 确保每个结点都有邻接表
                if (!adjacencyList.ContainsKey(vertex)) adjacencyList[vertex] = new List<Edge>();
                var adjacentEdges = adjacencyList[vertex];

                // [TYPE 1] WalkJump (this -> other)
                if (vertex.VertexType is VertexType.Plat or VertexType.PlatWithLadder)
                {
                    // 走到平台上
                    {
                        var pass = mapMatrix.GetNeighbors(vertex.x, vertex.y, false).Where(v =>
                            v != null && v.VertexType is VertexType.Plat or VertexType.PlatWithLadder &&
                            CanWalkJumpTo(vertex, v));
                        adjacentEdges.AddRange(pass.Select(v => new Edge(vertex, v, MoveType.WalkJump)));
                    }
                    // 到楼梯上
                    {
                        var pass = new[] { (-1, -1), (1, -1) }
                            .Select(t => mapMatrix[vertex.x + t.Item1, vertex.y + t.Item2])
                            .Where(v => v != null && v.VertexType is VertexType.Ladder && CanWalkJumpTo(vertex, v));
                        adjacentEdges.AddRange(pass.Select(v => new Edge(vertex, v, MoveType.ClimbWalkJump)));
                    }
                }


                // [Type 1] ClimbWalkJump (this -> other)
                if (vertex.VertexType is VertexType.Ladder or VertexType.PlatWithLadder)
                {
                    // 跳到平台上
                    {
                        var pass = new[] { (-1, 1), (1, 1) }.Select(t =>
                            mapMatrix[vertex.x + t.Item1, vertex.y + t.Item2]).Where(v =>
                            v != null && v.VertexType is VertexType.Plat or VertexType.PlatWithLadder &&
                            CanWalkJumpTo(vertex, v));
                        adjacentEdges.AddRange(pass.Select(v => new Edge(vertex, v, MoveType.ClimbWalkJump)));
                    }
                }

                if (vertex.VertexType is VertexType.Ladder)
                {
                    {
                        var downVertex = mapMatrix[vertex.x, vertex.y - 1];
                        bool pass = downVertex != null && downVertex.VertexType == VertexType.Plat;
                        if (pass) adjacentEdges.Add(new Edge(vertex, downVertex, MoveType.ClimbWalkJump));
                    }
                }

                // [Type 2] Climb (this -> other)
                // 在梯子上往附近爬
                if (vertex.VertexType is VertexType.Ladder or VertexType.PlatWithLadder)
                {
                    {
                        var pass = mapMatrix.GetNeighbors(vertex.x, vertex.y, false).Where(v =>
                            v != null && v.VertexType is VertexType.Ladder or VertexType.PlatWithLadder &&
                            CanWalkJumpTo(vertex, v));
                        adjacentEdges.AddRange(pass.Select(v => new Edge(vertex, v, MoveType.ClimbLadder)));
                    }
                }

                // 在平台上，梯子就在正上方，直接上去
                if (vertex.VertexType is VertexType.Plat)
                {
                    {
                        var upVertex = mapMatrix[vertex.x, vertex.y + 1];
                        var pass = upVertex != null && upVertex.VertexType == VertexType.Ladder;
                    }
                }

                // [TYPE 3] Fall && HumanLadder (this <-> other)
                // 这两个是相对的，代码按Fall的视角写
                // [1] 平台往两边下落
                if (vertex.VertexType is VertexType.Plat or VertexType.PlatWithLadder)
                {
                    foreach (int fallX in new int[] { vertex.x - 1, vertex.x + 1 }) // 检测左右两格
                    {
                        // [STEP 0] 排除不可能Fall的情况
                        if (levelMatrix[fallX, vertex.y].IsBlock) continue; // 唯一通路被墙堵住
                        if (mapMatrix[fallX, vertex.y] != null) continue; // 应该是WalkJump
                        if (mapMatrix[fallX, vertex.y - 1] != null) continue; // 应该是WalkJump

                        // 此时已经确认绝对是Fall
                        // [STEP 1] 获取通路高度
                        AllowedPassHeight allowedPassHeight;
                        if (levelMatrix[fallX, vertex.y].IsEmpty && levelMatrix[fallX, vertex.y + 1].IsBlock)
                        {
                            allowedPassHeight = AllowedPassHeight.One;
                        }
                        else
                        {
                            allowedPassHeight = AllowedPassHeight.TwoAndMore;
                        }

                        // [STEP 2] 向下遍历
                        for (int y = vertex.y - 2; y > 0; y--)
                        {
                            var currentVertex = mapMatrix[fallX, y];

                            if (currentVertex == null) continue; // 尚未找到落点

                            var toVertex = currentVertex;
                            // 设置边 (Fall)
                            adjacentEdges.Add(new Edge(vertex, toVertex, MoveType.Fall, allowedPassHeight));
                            // 设置边 (HumanLadder)
                            if (!adjacencyList.ContainsKey(toVertex)) adjacencyList[toVertex] = new List<Edge>();
                            adjacencyList[toVertex]
                                .Add(new Edge(toVertex, vertex, MoveType.HumanLadder, allowedPassHeight));
                            break;
                        }
                    }
                }

                // [2] 断楼梯往正下方下落
                if (vertex.VertexType is VertexType.Ladder)
                {
                    var downVertex = mapMatrix[vertex.x, vertex.y - 1];
                    bool pass = downVertex == null;
                    if (pass)
                    {
                        for (int y = vertex.y - 2; y > 0; y--)
                        {
                            var currentVertex = mapMatrix[vertex.x, vertex.y];
                            if (currentVertex == null) continue; // 尚未找到落点

                            var toVertex = currentVertex;
                            // 设置边 (Fall)
                            adjacentEdges.Add(new Edge(vertex, toVertex, MoveType.Fall,
                                AllowedPassHeight.TwoAndMore));
                            // 设置边 (HumanLadder)
                            if (!adjacencyList.ContainsKey(toVertex)) adjacencyList[toVertex] = new List<Edge>();
                            adjacencyList[toVertex].Add(new Edge(toVertex, vertex, MoveType.HumanLadder,
                                AllowedPassHeight.TwoAndMore));
                        }
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
                        return levelMatrix[xToTest, yToTest].IsEmpty || levelMatrix[xToTest, yToTest].IsClimbable;
                    }
                }
            }

            bool ShouldBeKeyVertex(Vertex vertex)
            {
                var edges = adjacencyList[vertex];
                // [特殊情况处理]
                // 1 - 无任何相邻结点
                if (edges.Count == 0) return false;
                // [核心代码]
                // 1 - 只连接一条keyEdge
                if (edges.Count == 1) return true;
                // 2 - 连接多条keyEdge，且移动方式不尽相同
                if (edges.Count > 1 && edges.Any(edge => edge.moveType != edges[1].moveType)) return true;

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
        public List<Vertex> FindAdjacentKeyVertices(in Vertex vertex, bool includeSelf = false)
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
                    foreach (var edge in adjacencyList.GetValueOrDefault(current, new List<Edge>()))
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
        public List<KeyEdge> FindKeyEdgesToAdjacentKeyVertices(in Vertex vertex)
        {
            // 验证输入
            if (!adjacencyList.ContainsKey(vertex)) throw new ArgumentException("Start/End not in graph");

            // 初始化数据结构
            Stack<KeyEdge> frontier = new Stack<KeyEdge>();
            List<Vertex> visited = new List<Vertex>();
            List<KeyEdge> result = new List<KeyEdge>();

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

        /// <summary>
        /// 给两个Vertex，已知它们在同一KeyEdge里，start到end的keyEdge
        /// </summary>
        /// <returns></returns>
        public KeyEdge FindKeyEdgeInOneKeyEdge(in Vertex startVertex, in Vertex endVertex)
        {
            // 验证输入
            if (!(adjacencyList.ContainsKey(startVertex) && adjacencyList.ContainsKey(endVertex)))
                throw new ArgumentException("Start/End not in graph");
            // [Warning] 可能出bug：如果moveType不是WalkJump
            if (startVertex == endVertex)
                return new KeyEdge(new Edge(startVertex, endVertex, MoveType.WalkJump, AllowedPassHeight.TwoAndMore));
            if (startVertex.isKey)
            {
                if (endVertex.isKey)
                {
                    var otherKeyVertex = endVertex; // 避免in
                    var keyEdge = keyAdjacencyList[startVertex].FirstOrDefault(ke => ke.To == otherKeyVertex);
                    return GetKeyEdgeInKeyEdge(startVertex, endVertex, keyEdge);
                }
                else
                {
                    var endCluster = GetCluster(endVertex);
                    var otherKeyVertex = endCluster.vertexA == startVertex ? endCluster.vertexB : endCluster.vertexA;
                    var keyEdge = keyAdjacencyList[startVertex].FirstOrDefault(ke => ke.To == otherKeyVertex);
                    return GetKeyEdgeInKeyEdge(startVertex, endVertex, keyEdge);
                }
            }

            // 初始化数据结构
            Stack<KeyEdge> frontier = new Stack<KeyEdge>();
            List<Vertex> visited = new List<Vertex>();

            visited.Add(startVertex);
            foreach (var each in adjacencyList[startVertex])
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
                // [Case 1] 找到了终点
                if (v == endVertex) return current;

                // [Case 2] 遇到了keyVertex（已规避终点是keyVertex的情况）
                if (v.isKey)
                {
                    frontier.Pop();
                    continue;
                }

                // [Case 3] 常规寻路

                // 将临近的结点加入列表
                var adj = adjacencyList[current.To].Where(edge => !visited.Contains(edge.To)).ToList();

                // 异常处理
                if (adj.Count >= 2)
                {
                    string exp = "GetKeyEdgesToAdjacentKeyVertices的路径中出现岔路，这是不应该的。";
                    exp += $"源头是({current.From.x},{current.From.y})。";
                    exp += $"正在探索：" + String.Join(",",
                        adj.Select(each => $"({each.From.x},{each.From.y})->({each.To.x},{each.To.y})"));
                    Debug.LogWarning(exp);
                }


                var next = adj.First();
                visited.Add(next.To);
                current.AddEdge(next);
            }


            throw new ArgumentException($"GetKeyEdgeInOneKeyEdge未找到路径: {startVertex.Position}->{endVertex.Position}");
        }

        public KeyEdge GetKeyEdgeInKeyEdge(in Vertex startVertex, in Vertex endVertex, in KeyEdge keyEdge)
        {
            // 验证输入
            if (startVertex == endVertex) throw new ArgumentException();

            //
            var result = new KeyEdge(keyEdge.moveType);
            bool startRecord = false;
            foreach (var edge in keyEdge.includeEdges)
            {
                if (edge.From == startVertex) startRecord = true;
                if (startRecord)
                {
                    result.AddEdge(edge);
                    if (edge.To == endVertex) return result;
                }
            }

            throw new ArgumentException(
                $"无法找到keyEdge：{startVertex.Position}->{endVertex.Position}, 在KeyEdge({keyEdge.From.x},{keyEdge.From.y}) -> ({keyEdge.To.x},{keyEdge.To.y})中");
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

            throw new ArgumentException($"_clusterCache未包含所有Vertex导致出错，这个Vertex是({vertex.x},{vertex.y})");
        }

        #endregion

        #endregion

        #region 调试相关

        public void DebugDisplayMatrix()
        {
            var resLoader = ResLoader.Allocate();
            var tilemap = LevelTilemapController.Instance.Debug;
            var OneHeight = resLoader.LoadSync<Tile>(Leveldebug.BundleName, Leveldebug.DebugOneHeight);
            var TwoHeight = resLoader.LoadSync<Tile>(Leveldebug.BundleName, Leveldebug.DebugTwoHeight);
            var Key = resLoader.LoadSync<Tile>(Leveldebug.BundleName, Leveldebug.DebugKey);
            foreach (var each in vertices)
            {
                if (each.isKey)
                {
                    tilemap.SetTile(new Vector3Int(each.x, each.y, 0), Key);
                }
                else
                {
                    if (each.AllowedPassHeight is AllowedPassHeight.One)
                    {
                        tilemap.SetTile(new Vector3Int(each.x, each.y, 0), OneHeight);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(each.x, each.y, 0), TwoHeight);
                    }
                }
            }

            resLoader.Recycle2Cache();
        }

        public void DebugLogCluster(Vector2Int pos)
        {
            var vertex = GetVertex(pos.x, pos.y);
            var cluster = GetCluster(vertex);
            $"cluster of ({vertex.x},{vertex.y}) is ({cluster.vertexA.x},{cluster.vertexA.y}) and ({cluster.vertexB.x},{cluster.vertexB.y})"
                .LogInfo();
        }

        public void LogAllKeyAdjacencyList()
        {
            foreach (var keyVertex in keyVertices)
            {
                string output = $"[LogAllKeyAdjacencyList] KeyVertex: ({keyVertex.x},{keyVertex.y})->";
                foreach (var keyEdge in keyAdjacencyList[keyVertex])
                {
                    output += $"({keyEdge.To.x},{keyEdge.To.y}), ";
                }

                output.LogInfo();
            }
        }

        public void LogThePath(Path path)
        {
            string output = "";
            foreach (var edge in path.keyEdges)
            {
                output += $"({edge.From.x},{edge.From.y}) -> ({edge.To.x},{edge.To.y}) && ";
            }

            output.LogInfo();
        }

        public void LogThePath(IZombiePath path)
        {
            string output = "[LogThePath] ";
            while (path.Count > 0)
            {
                var next = path.NextTarget();
                output += $"{next.target}({next.moveType})->";
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