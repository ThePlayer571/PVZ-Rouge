using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Class;
using TPL.PVZR.Classes.ZombieAI.Public;

namespace TPL.PVZR.Classes.ZombieAI.PathFinding
{
    public interface IPathManager
    {
        Path GetOnePath(Vertex startVertex, Vertex endVertex, AITendency aiTendency);
        void ClearCache();
    }

    public class PathManager : IPathManager
    {
        #region 公有方法

        public Path GetOnePath(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            var paths = GetMultiPaths(startVertex, endVertex, aiTendency);
            return aiTendency.ChooseOnePath(paths);
        }

        public void ClearCache()
        {
            _pathCache.Clear();
        }

        #endregion

        #region 一层具象

        private List<Path> GetMultiPaths(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            // 预制数据
            var startCluster = _zombieAIUnit.GetCluster(startVertex);
            var endCluster = _zombieAIUnit.GetCluster(endVertex);

            // 简单情况处理，这时不需要调用寻路算法
            if (startVertex == endVertex)
            {
                var moveType = startVertex.VertexType.ToMoveType();
                return new List<Path> { new Path(moveType) };
            }

            if (startCluster.IsIdentical(endCluster))
            {
                var keyEdge = _zombieAIUnit.CreateKeyEdgeInOneCluster(startVertex, endVertex);
                // 已经被逻辑地确定，不需要这行代码。但以防万一，请不要删除
                // if (aiTendency.IsBannedKeyEdge(keyEdge))
                // {
                //     goto flagCallAlgorithm;
                // }

                return new List<Path> { new Path(keyEdge.moveType) };
            }

            if (startCluster.GetIntersection(endCluster, out var intersection))
            {
                var path = new Path();

                if (startVertex != intersection)
                {
                    var firstEdge = _zombieAIUnit.CreateKeyEdgeInOneCluster(startVertex, intersection);
                    if (aiTendency.IsBannedKeyEdge(firstEdge))
                    {
                        goto flagCallAlgorithm;
                    }

                    path.Add(firstEdge);
                }

                if (endVertex != intersection)
                {
                    var secondEdge = _zombieAIUnit.CreateKeyEdgeInOneCluster(intersection, endVertex);
                    if (aiTendency.IsBannedKeyEdge(secondEdge, true))
                    {
                        goto flagCallAlgorithm;
                    }

                    path.Add(secondEdge);
                }

                return new List<Path> { path };
            }

            // 调用寻路算法
            flagCallAlgorithm:

            var pathKey = new PathKey(new FromToKey<Cluster>(startCluster, endCluster), aiTendency);

            // 不存在这条路，利用寻路算法补充
            if (!_pathCache.ContainsPath(pathKey))
            {
                var pfa = new PathFindingAlgorithm(_zombieAIUnit.keyAdjacencyList);
                List<Path> siegePaths = pfa.AStarFindSiegePath(startCluster, endCluster, aiTendency);

                _pathCache.AddPathRange(pathKey, siegePaths);
                // "调用了寻路算法".LogInfo();
            }

            // 此时肯定有路径了，返回所有路径
            List<Path> result = new List<Path>();
            if (_pathCache.TryGetValue(pathKey, out var paths))
            {
                foreach (var path in paths)
                {
                    // 拼接路径
                    KeyEdge start = null, end = null;

                    var keyVertexStart = path.keyEdges.First().From;
                    var keyVertexEnd = path.keyEdges.Last().To;

                    if (startVertex != keyVertexStart)
                        start = _zombieAIUnit.CreateKeyEdgeInOneCluster(startVertex, keyVertexStart);
                    if (endVertex != keyVertexEnd)
                        end = _zombieAIUnit.CreateKeyEdgeInOneCluster(keyVertexEnd, endVertex);

                    var newPath = new Path();
                    if (start != null) newPath.Add(start);
                    newPath.keyEdges.AddRange(path.keyEdges);
                    if (end != null) newPath.Add(end);

                    result.Add(newPath);
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