using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Core;
using TPL.PVZR.Core.PriorityQueue;
using TPL.PVZR.Gameplay.Class.ZombieAI.New.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI.New.PathFinding;
using TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.New.Others
{
    public interface IPathManager
    {
        IPath GetPathAllowUnreachable(Vertex startVertex, Vertex endVertex, AITendency aiTendency);
    }

    /*
     * 缓存路径
     * 路径不一定是最短路径（启发函数导致，无需在意），但一定包括最短的
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

        private List<IPath> GetMultiPaths(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            List<IPath> result = new List<IPath>();
            // 获取首尾的KeyEdge
            List<IKeyEdge> startKeyEdges, endKeyEdges;
            startKeyEdges = startVertex.isKey
                ? new List<IKeyEdge>()
                : _zombieAIUnit.GetKeyEdgesToAdjacentKeyVertices(startVertex);
            endKeyEdges = endVertex.isKey
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
                var pfa = new PathFindingAlgorithm(_zombieAIUnit.keyAdjacencyList);
                var siegePaths = pfa.AStarFindSiegePath(startCluster, endCluster, aiTendency);
                
                _pathCache.AddPathRange(pathKey, siegePaths);
            }
            
            // 如果已经有这条路了，直接返回
            if (_pathCache.TryGetValue(pathKey, out var paths))
            {
                foreach (var path in paths)
                {
                    // 拼接路径
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