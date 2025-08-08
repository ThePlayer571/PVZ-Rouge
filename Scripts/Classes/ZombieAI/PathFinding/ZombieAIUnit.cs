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
using UnityEngine.InputSystem;
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


            #region 一大坨调试代码

            // foreach (var vertex in vertices)
            // {
            //     string output = $"({vertex.x}, {vertex.y}, {vertex.VertexType})：";
            //     foreach (var edge in adjacencyList[vertex])
            //     {
            //         output += $" ({edge.To.x}, {edge.To.y}, {edge.moveType}, {edge.passableHeight})";
            //     }
            //
            //     output.LogInfo();
            // }

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

            #region 内部函数

            void RecordAllVertices()
            {
                // 遍历所有基岩内部的格子（不包括基岩）
                for (int x = 1; x < levelMatrix.Rows - 1; x++)
                {
                    for (int y = 1; y < levelMatrix.Columns - 1; y++)
                    {
                        /* cases:
                         *  1 - (0, 0).!Block
                         *  2 - (0,-1).Block
                         *  3 - (0, 0).Climbable
                         *  4 - (0, 0).Water
                         *  5 - (0, 1).!Water
                         *
                         * conditions:
                         *  1 - 1100 - Plat
                         *  2 - 1110 - PlatWithLadder
                         *  3 - 1010 - Ladder
                         *  4 - xxx1 - Water
                         *  0 - else - notVertex
                         */
                        bool case_1 = !levelMatrix[x, y].Is(CellTypeId.Block);
                        bool case_2 = levelMatrix[x, y - 1].Is(CellTypeId.Block);
                        bool case_3 = levelMatrix[x, y].Is(CellTypeId.Climbable);
                        bool case_4 = levelMatrix[x, y].Is(CellTypeId.Water);
                        bool case_5 = !levelMatrix[x, y + 1].Is(CellTypeId.Water);

                        int condition = (case_1 ? 1 << 3 : 0) |
                                        (case_2 ? 1 << 2 : 0) |
                                        (case_3 ? 1 << 1 : 0) |
                                        (case_4 ? 1 << 0 : 0);

                        condition = condition switch
                        {
                            0b1100 => 1,
                            0b1110 => 2,
                            0b1010 => 3,
                            _ when (condition & 0b0001) != 0 && case_5 => 4,
                            _ => 0
                        };

                        // 获取newVertex
                        Vertex newVertex = null;
                        switch (condition)
                        {
                            case 1: // Plat
                            {
                                var minPassableHeight = GetMinPassableHeight(x, y);
                                newVertex = new Vertex(x, y, VertexType.Plat, minPassableHeight);
                                break;
                            }
                            case 2: // PlatWithLadder
                            {
                                var minPassableHeight = GetMinPassableHeight(x, y);
                                newVertex = new Vertex(x, y, VertexType.PlatWithLadder, minPassableHeight);
                                break;
                            }
                            case 3: // Ladder
                            {
                                var minPassableHeight = GetMinPassableHeight(x, y);
                                newVertex = new Vertex(x, y, VertexType.Ladder, minPassableHeight);
                                break;
                            }
                            case 4: // Water
                            {
                                var minPassableHeight = GetMinPassableHeight(x, y);
                                newVertex = new Vertex(x, y, VertexType.Water, minPassableHeight);
                                break;
                            }
                        }

                        // 设置newVertex
                        if (newVertex != null)
                        {
                            mapMatrix[x, y] = newVertex;
                            vertices.Add(newVertex);
                            adjacencyList.Add(newVertex, new List<Edge>());
                        }


                        int GetMinPassableHeight(int x, int y)
                        {
                            int heightOffset = 1;
                            for (; heightOffset < AITendency.PASSABLE_HEIGHT_最大值; heightOffset++)
                            {
                                var currentCell = levelMatrix[x, y + heightOffset];
                                if (currentCell.Is(CellTypeId.Block)) break;
                            }

                            return heightOffset;
                        }
                    }
                }
            }

            void BuildAdjacencyList(Vertex vertex)
            {
                // tip: 可以忽略matrix越界检测，逻辑上不会越界（有基岩包裹）

                // 核心
                var vertexType = vertex.VertexType;

                if (vertexType.IsPlat())
                {
                    // WalkJump TO Plat (this -> other) [直线和跳跃]
                    {
                        var passVertices =
                            mapMatrix.GetNeighbors(vertex.x, vertex.y, false)
                                .Where(v => v != null && v.VertexType.IsPlat() && CanWalkJumpTo(vertex, v));
                        adjacencyList[vertex]
                            .AddRange(passVertices.Select(v =>
                                new Edge(vertex, v, MoveType.WalkJump, GetPassableHeight_WalkJump(vertex, v))));
                    }
                    // Climb_WalkJump TO Ladder (this <-> other) [斜着向下拉梯子]
                    {
                        var passVertices =
                            new[] { (-1, -1), (1, -1) }.Select(t => mapMatrix[vertex.x + t.Item1, vertex.y + t.Item2])
                                .Where(v => v != null && v.VertexType.IsLadder() && CanWalkJumpTo(vertex, v));
                        foreach (var passVertex in passVertices)
                        {
                            var passableHeight = GetPassableHeight_WalkJump(vertex, passVertex);
                            adjacencyList[vertex]
                                .Add(new Edge(vertex, passVertex, MoveType.Climb_WalkJump, passableHeight));
                            adjacencyList[passVertex]
                                .Add(new Edge(passVertex, vertex, MoveType.Climb_WalkJump, passableHeight));
                        }
                    }
                    // WalkJump_Swim TO Water (this <-> other) [下水]
                    {
                        var passVertices =
                            new[] { (-1, -1), (1, -1) }.Select(t => mapMatrix[vertex.x + t.Item1, vertex.y + t.Item2])
                                .Where(v => v != null && v.VertexType.IsWater() && CanWalkJumpTo(vertex, v));
                        foreach (var passVertex in passVertices)
                        {
                            var passableHeight = GetPassableHeight_WalkJump(vertex, passVertex);
                            adjacencyList[vertex]
                                .Add(new Edge(vertex, passVertex, MoveType.Swim_WalkJump, passableHeight));
                            adjacencyList[passVertex]
                                .Add(new Edge(passVertex, vertex, MoveType.Swim_WalkJump, passableHeight));
                        }
                    }
                    // Climb_WalkJump TO Ladder (this <-> other) [往正上方拉梯子]
                    {
                        var upVertex = mapMatrix[vertex.x, vertex.y + 1];
                        if (upVertex != null && vertexType != VertexType.PlatWithLadder)
                            // 此处不考虑PlatWithLadder（它应该是直接Climb的）
                        {
                            var passableHeight = upVertex.PassableHeight;
                            adjacencyList[vertex]
                                .Add(new Edge(vertex, upVertex, MoveType.Climb_WalkJump, passableHeight));
                            adjacencyList[upVertex]
                                .Add(new Edge(upVertex, vertex, MoveType.Climb_WalkJump, passableHeight));
                        }
                    }
                    // Fall&&HumanLadder TO AnyOther (this <-> other) [往两边掉落]
                    {
                        var y = vertex.y;
                        foreach (int x in new[] { vertex.x - 1, vertex.x + 1 })
                        {
                            if (levelMatrix[x, y].Is(CellTypeId.Block)) continue;
                            var downVertex = GetDownVertex(x, y);
                            if (y - downVertex.y <= 1) continue;

                            var passableHeight = GetPassableHeight_Fall(vertex, downVertex);
                            adjacencyList[vertex].Add(new Edge(vertex, downVertex, MoveType.Fall, passableHeight));
                            adjacencyList[downVertex]
                                .Add(new Edge(downVertex, vertex, MoveType.HumanLadder, passableHeight));
                        }
                    }
                }

                if (vertexType.IsLadder())
                {
                    // Climb TO Ladder (this -> other) [向上下爬]
                    {
                        var passedVertices =
                            new[] { (0, 1), (0, -1) }.Select(t => mapMatrix[vertex.x + t.Item1, vertex.y + t.Item2])
                                .Where(v => v != null && v.VertexType.IsLadder());
                        adjacencyList[vertex].AddRange(passedVertices.Select(v =>
                            new Edge(vertex, v, MoveType.ClimbLadder, v.PassableHeight)));
                    }
                    // Fall&&HumanLadder TO AnyOther (this <-> other) [往正下方掉落]
                    {
                        if (vertexType == VertexType.Ladder && mapMatrix[vertex.x, vertex.y - 1] == null)
                        {
                            var downVertex = GetDownVertex(vertex.x, vertex.y - 1);
                            var passableHeight = vertex.PassableHeight;
                            adjacencyList[vertex].Add(new Edge(vertex, downVertex, MoveType.Fall, passableHeight));
                            adjacencyList[downVertex]
                                .Add(new Edge(downVertex, vertex, MoveType.HumanLadder, passableHeight));
                        }
                    }
                }

                if (vertexType.IsWater())
                {
                    // Swim TO Water (this -> other) [往左右游]
                    {
                        var passedVertices =
                            new[] { (-1, 0), (1, 0) }.Select(t => mapMatrix[vertex.x + t.Item1, vertex.y + t.Item2])
                                .Where(v => v != null && v.VertexType.IsWater());
                        adjacencyList[vertex].AddRange(passedVertices.Select(v =>
                            new Edge(vertex, v, MoveType.Swim, GetPassableHeight_WalkJump(vertex, v))));
                    }
                    // Climb_Swim TO Ladder (this <-> other) [往正上方拉梯子]
                    {
                        var upVertex = mapMatrix[vertex.x, vertex.y + 1];
                        if (upVertex != null)
                        {
                            var passableHeight = upVertex.PassableHeight;
                            adjacencyList[vertex]
                                .Add(new Edge(vertex, upVertex, MoveType.Climb_Swim, passableHeight));
                            adjacencyList[upVertex]
                                .Add(new Edge(upVertex, vertex, MoveType.Climb_Swim, passableHeight));
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
                        return !levelMatrix[xToTest, yToTest].Is(CellTypeId.Block);
                    }
                }

                int GetPassableHeight_WalkJump(Vertex from, Vertex to)
                {
                    if (from.y == to.y)
                    {
                        return Math.Min(from.PassableHeight, to.PassableHeight);
                    }
                    else
                    {
                        var lowerVertex = from.y < to.y ? from : to;
                        var higherVertex = to.y > from.y ? to : from;

                        return Math.Min(lowerVertex.PassableHeight - 1, higherVertex.PassableHeight);
                    }
                }

                int GetPassableHeight_Fall(Vertex from, Vertex to)
                {
                    if (from.y <= to.y) throw new ArgumentException();
                    var x = to.x;
                    var y = from.y;

                    int offsetY = 0;
                    for (; offsetY < AITendency.PASSABLE_HEIGHT_最大值; offsetY++)
                    {
                        if (levelMatrix[x, y + offsetY].Is(CellTypeId.Block)) break;
                    }

                    return Math.Min(offsetY + 1, from.PassableHeight);
                }

                Vertex GetDownVertex(int x, int y)
                {
                    try
                    {
                        for (int offsetY = 0;; offsetY--)
                            // 需要已经确认一定存在downVertex
                        {
                            if (mapMatrix[x, y + offsetY] != null)
                                return mapMatrix[x, y + offsetY];
                        }
                    }
                    catch
                    {
                        $"error: {x},{y}".LogInfo();
                        throw new Exception();
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
                if (edges.Count > 1 && edges.Skip(1).Any(e =>
                        e.moveType != edges[0].moveType || e.passableHeight != edges[0].passableHeight)) return true;

                return false;
            }

            void SetClusterCache(Vertex keyVertex, List<KeyEdge> keyEdges)
            {
                // 逻辑：先把keyVertex解决，再把keyEdge中的所有vertex解决
                // 优先选择WalkJump且可到达的相邻keyVertex，如果没有，就与自己组成Cluster

                // [1] KeyVertex
                var idealKeyVertex = keyEdges.FirstOrDefault(k =>
                    k.moveType == MoveType.WalkJump && k.passableHeight >= keyVertex.PassableHeight)?.To;
                idealKeyVertex ??= keyVertex;
                _clusterCache.Add(keyVertex, new Cluster(keyVertex, idealKeyVertex));

                // [2] Vertex
                foreach (var keyEdge in keyEdges)
                {
                    var includeVertices = keyEdge.IncludeVertices();
                    // 中间没有结点 || 中间结点都被设置过了
                    if (includeVertices.Count == 2 || _clusterCache.ContainsKey(includeVertices[1])) continue;

                    var cluster = new Cluster(includeVertices.First(), includeVertices.Last());
                    for (int i = 1; i < includeVertices.Count - 1; i++)
                    {
                        _clusterCache.Add(includeVertices[i], cluster);
                    }
                }
            }

            #endregion
        }

        #endregion

        #region 寻路相关

        private IPathManager _pathManager;

        #region 底层函数（不调用KeyAgjecencyList）

        /// <summary>
        /// 找到与vertex到相邻关键节点的Edge。如果出现环，可能会报错(没出现过)。如果vertex.isKey，不会包括自己
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public List<KeyEdge> FindKeyEdgesToAdjacentKeyVertices(Vertex vertex)
        {
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

        #endregion

        #region 实用函数

        /// <summary>
        /// 传入两个keyVertex，返回它们之间的KeyEdge
        /// </summary>
        /// <param name="startVertex"></param>
        /// <param name="endVertex"></param>
        /// <returns></returns>
        public KeyEdge GetKeyEdge(Vertex startVertex, Vertex endVertex)
        {
            // 验证输入
            if (!startVertex.isKey) throw new ArgumentException();
            if (!endVertex.isKey) throw new ArgumentException();

            //
            return keyAdjacencyList[startVertex]
                       .FirstOrDefault(keyEdge => keyEdge.To == endVertex) ??
                   throw new ArgumentException(
                       $"无法找到keyEdge：{startVertex.Position}->{endVertex.Position}, 在KeyAdjacencyList中");
        }

        /// <summary>
        /// 传入两个vertex，已知它们在同一个Cluster（或如果有keyVertex，一定相邻）。创建或返回它们之间的KeyEdge
        /// </summary>
        /// <param name="startVertex"></param>
        /// <param name="endVertex"></param>
        /// <returns></returns>
        public KeyEdge CreateKeyEdgeInOneCluster(Vertex startVertex, Vertex endVertex)
        {
            if (startVertex == endVertex) throw new ArgumentException();

            var cluster = startVertex.isKey && endVertex.isKey
                ? new Cluster(startVertex, endVertex)
                : startVertex.isKey
                    ? GetCluster(endVertex)
                    : GetCluster(startVertex);

            var keyEdges = new[]
                { GetKeyEdge(cluster.vertexA, cluster.vertexB), GetKeyEdge(cluster.vertexB, cluster.vertexA) };
            foreach (var keyEdge in keyEdges)
            {
                var result = new KeyEdge(keyEdge.moveType);
                bool startRecord = false;
                bool findResult = false;
                foreach (var edge in keyEdge.includeEdges)
                {
                    if (edge.From == startVertex) startRecord = true;
                    if (edge.To == endVertex && !startRecord) break;
                    if (startRecord)
                    {
                        result.AddEdge(edge);
                        if (edge.To == endVertex)
                        {
                            findResult = true;
                            break;
                        }
                    }
                }

                if (findResult) return result;
            }

            throw new Exception($"未找到keyEdge: {startVertex.Position}->{endVertex.Position}");
        }

        #endregion


        #region Cluster缓存

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
            // 仅在Debug模式下使用
            var resLoader = ResLoader.Allocate();
            var tilemap = LevelTilemapNode.Instance.Debug;
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
                    if (each.PassableHeight == 1)
                    {
                        tilemap.SetTile(new Vector3Int(each.x, each.y, 0), OneHeight);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(each.x, each.y, 0), TwoHeight);
                    }
                }
            }

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
    }
}