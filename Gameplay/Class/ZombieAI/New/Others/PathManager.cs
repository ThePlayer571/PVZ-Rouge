using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Core;
using TPL.PVZR.Core.PriorityQueue;
using TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.New.Others
{
    public interface IPathManager
    {
        IPath GetPathAllowUnreachable(Vertex startVertex, Vertex endVertex, AITendency aiTendency);
    }

/*
 * 缓存路径
 * 路径不一定是最短路径，但一定包括最短的
 */
    public class PathManager : IPathManager
    {
        #region 公有方法

        public IPath GetPathAllowUnreachable(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            var paths = GetMultiPaths(startVertex, endVertex, aiTendency);
            return aiTendency.ChooseOnePath(paths);
        }

        #endregion

        #region 一层具象

        class PathFindingAlgorithm
        {
            private readonly Dictionary<Vertex, List<IKeyEdge>> _keyAdjacencyList;

            public PathFindingAlgorithm(Dictionary<Vertex, List<IKeyEdge>> keyAdjacencyList)
            {
                this._keyAdjacencyList = keyAdjacencyList;
            }

            public List<Path> AStarFindSiegePath(Cluster startCluster, Cluster endCluster, AITendency aiTendency)
            {
                // 预制数据
                var keyAdjacencyList = this._keyAdjacencyList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
                keyAdjacencyList[endCluster.vertexA].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexB));
                keyAdjacencyList[endCluster.vertexB].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexA));
                //
                var a = new AStarPathfinder(keyAdjacencyList);
                var pathToA = a.FindPath(startCluster,endCluster.vertexA);
                var pathToB = AStarFindPath(endCluster.vertexB);
                return new List<Path> { pathToA, pathToB };
                
                
            }

            class AStarNode:FastPriorityQueueNode
            {
                public Vertex vertex;
                public AStarNode Parent;
                public IKeyEdge fromEdge;
                public float GCost;
                public float HCost;
                public float Fcost => GCost + HCost;

                public AStarNode(Vertex vertex, AStarNode parent, IKeyEdge fromEdge, float fCost, float gCost)
                {
                    this.vertex = vertex;
                    this.parent = parent;
                    this.fromEdge = fromEdge;
                    this.fCost = fCost;
                    this.gCost = gCost;
                }
            }

            class AStarCostComparer : IComparer<AStarNode>
            {
                public int Compare(AStarNode x, AStarNode y)
                {
                    return x.cost.CompareTo(y.cost);
                }
            }
        }


        private List<Path> GetMultiPaths(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            List<Path> result = new List<Path>();
            // 获取首尾的KeyEdge
            List<IKeyEdge> startKeyEdges, endKeyEdges;
            startKeyEdges = startVertex.isKey
                ? new List<IKeyEdge>()
                : _zombieAIUnit.GetKeyEdgesToAdjacentKeyVertices(startVertex);
            endKeyEdges = startVertex.isKey
                ? new List<IKeyEdge>()
                : _zombieAIUnit.GetKeyEdgesToAdjacentKeyVertices(endVertex)
                    .Select(keyEdge => keyEdge.Adversed(_zombieAIUnit.adjacencyList)).ToList();
            // 预制数据
            var startCluster = new Cluster(startKeyEdges.First().To, startKeyEdges.Last().To);
            var endCluster = new Cluster(endKeyEdges.First().To, endKeyEdges.Last().To);
            var fromToKey = new FromToKey<Cluster>(startCluster, endCluster);
            var pathKey = new PathKey(fromToKey, aiTendency);
            // 不存在这条路，利用寻路算法补充
            if (!_pathCache.ContainsPath(pathKey))
            {
                var pfa = new PathFindingAlgorithm();
                var siegePaths = pfa.AStarFindSiegePath(startCluster, endCluster, aiTendency);
                foreach (var siegePath in siegePaths)
                {
                    _pathCache.AddPath(pathKey, siegePath);
                }
            }


            // 如果已经有这条路了，直接返回
            if (_pathCache.TryGetValue(pathKey, out var paths))
            {
                foreach (var path in paths)
                {
                    result.Add(new Path(
                        startKeyEdges.First(keyEdge => keyEdge.To == path.keyEdges.First().From), path,
                        endKeyEdges.First(keyEdge => keyEdge.From == path.keyEdges.Last().To)));
                }

                return result;
            }
            else
            {
                throw new Exception("与预期不符：使用寻路算法后仍然不存在路");
            }
            // 拼接路径
        }

        #endregion

        #region 数据结构

        private ZombieAIUnit _zombieAIUnit;

        private IPathCache _pathCache;

        #endregion

        #region 构造函数

        public PathManager(ZombieAIUnit zombieAIUnit)
        {
            this._zombieAIUnit = zombieAIUnit;
        }

        #endregion
    }
}