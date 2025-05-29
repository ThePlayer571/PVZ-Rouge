using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Gameplay.Class.ZombieAI.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.PathFinding
{
    public interface IPathManager
    {
        IPath GetOnePath(Vertex startVertex, Vertex endVertex, AITendency aiTendency);
    }

    /*
     * 缓存路径
     * 路径不一定是最短路径（启发函数导致，无需在意），但一定包括最短的
     */
    public class PathManager : IPathManager
    {
        #region 公有方法

        public IPath GetOnePath(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            var paths = GetMultiPaths(startVertex, endVertex, aiTendency);
            return aiTendency.ChooseOnePath(paths);
        }

        #endregion

        #region 一层具象

        private List<IPath> GetMultiPaths(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            // 预制数据
            var startCluster = _zombieAIUnit.GetCluster(startVertex);
            var endCluster = _zombieAIUnit.GetCluster(endVertex);
            // TODO: 
            
            
            
            List<IKeyEdge> startKeyEdges, endKeyEdges;
            PathKey pathKey;

            startKeyEdges = startVertex.isKey
                ? new List<IKeyEdge>()
                : _zombieAIUnit.GetKeyEdgesToAdjacentKeyVertices(startVertex);
            endKeyEdges = endVertex.isKey
                ? new List<IKeyEdge>()
                : _zombieAIUnit.GetKeyEdgesToAdjacentKeyVertices(endVertex)
                    .Select(keyEdge => keyEdge.Adversed(_zombieAIUnit.adjacencyList)).ToList();
            pathKey = new PathKey(new FromToKey<Cluster>(startCluster, endCluster), aiTendency);
            
            // 不存在这条路，利用寻路算法补充
            if (!_pathCache.ContainsPath(pathKey))
            {
                var pfa = new PathFindingAlgorithm(_zombieAIUnit.keyAdjacencyList);
                List<IPath> siegePaths = pfa.AStarFindSiegePath(startCluster, endCluster, aiTendency);

                _pathCache.AddPathRange(pathKey, siegePaths);
                "调用了寻路算法".LogInfo();
            }

            // 如果已经有这条路了，直接返回
            List<IPath> result = new List<IPath>();
            if (_pathCache.TryGetValue(pathKey, out var paths))
            {
                foreach (var path in paths)
                {
                    // 拼接路径
                    var start = startKeyEdges.Any()
                        ? startKeyEdges.First(keyEdge => keyEdge.To == path.keyEdges.First().From)
                        : null;
                    var end = endKeyEdges.Any()
                        ? endKeyEdges.First(keyEdge => keyEdge.From == path.keyEdges.Last().To)
                        : null;
                    result.Add(new Path(start, path, end));
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
            this._pathCache = new PathCache();
        }

        #endregion
    }
}