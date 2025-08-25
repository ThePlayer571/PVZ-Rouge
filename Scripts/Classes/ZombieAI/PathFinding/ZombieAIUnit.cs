using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.ZombieAI.Class;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.Others.LevelScene;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

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

        public async Task InitializeFromAsync(Matrix<Cell> levelMatrix)
        {
            // 初始化
            _pathManager = new PathManager(this);
            // 异步支持
            _isBaking = true;
            _bakeTcs = new TaskCompletionSource<object>();
            // 开始烘焙
            GameManager.Instance.StartCoroutine(BakeFrom(levelMatrix));
            await _bakeTcs.Task;
        }

        public Task RebakeFromAsync(Matrix<Cell> levelMatrix, out bool isRebakeSuccess)
        {
            if (_isBaking)
            {
                isRebakeSuccess = false;
                return Task.CompletedTask;
            }

            _isBaking = true;
            _bakeTcs = new TaskCompletionSource<object>();
            GameManager.Instance.StartCoroutine(BakeFrom(levelMatrix));
            isRebakeSuccess = true;
            return _bakeTcs.Task;
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

        private Dictionary<Vertex, Cluster> _clusterCache;

        #endregion

        #region 烘焙相关

        /// <summary>
        /// 根据当前的地图烘焙数据结构
        /// </summary>
        private IEnumerator BakeFrom(Matrix<Cell> levelMatrix)
        {
            "start bake".LogInfo();
            // !!== 注释有重要的调试代码，请勿删除 ==!!
            // 初始化数据结构
            var baker = new Baker(levelMatrix);
            // 记录烘焙时间
            LastBakeTime = Time.time;

            // [STEP 1] 记录所有顶点
            baker.STEP_1();
            yield return null;

            // foreach (var vertex in vertices)
            // {
            //     $"{vertex.Position}, {vertex.VertexType}".LogInfo();
            // }

            // [STEP 2] 构建邻接表
            baker.STEP_2();
            yield return null;

            // [STEP 3] 标记关键顶点
            baker.STEP_3();


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

            // foreach (var vertex in keyVertices)
            // {
            //     $"Key: {vertex.Position}, {vertex.VertexType}".LogInfo();
            // }

            #endregion

            // [STEP 4] 连接关键结点
            baker.STEP_4();

            // [STEP 5] 重置数据结构
            this.mapMatrix = baker.mapMatrix;
            this.adjacencyList = baker.adjacencyList;
            this.keyAdjacencyList = baker.keyAdjacencyList;
            this.vertices = baker.vertices;
            this.keyVertices = baker.keyVertices;
            this._clusterCache = baker._clusterCache;
            _pathManager.ClearCache();
            // 异步支持
            _bakeTcs?.SetResult(null);
            _isBaking = false;
            "end bake".LogInfo();
        }

        #endregion

        #region 寻路相关

        private IPathManager _pathManager;

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

        // 标记是否需要重新烘焙
        public bool RebakeDirty { get; set; }
        public float LastBakeTime { get; private set; }

        private bool _isBaking = false;
        private TaskCompletionSource<object> _bakeTcs;
    }
}