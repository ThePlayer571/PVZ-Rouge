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
        Path GetOnePath(Vertex startVertex, Vertex endVertex, AITendency aiTendency);
    }

    /*
     * 缓存路径
     * 路径不一定是最短路径（启发函数导致，无需在意），但一定包括最短的
     */
    public class PathManager : IPathManager
    {
        #region 公有方法

        public Path GetOnePath(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            var paths = GetMultiPaths(startVertex, endVertex, aiTendency);
            return aiTendency.ChooseOnePath(paths);
        }

        #endregion

        #region 一层具象

        private List<Path> GetMultiPaths(Vertex startVertex, Vertex endVertex, AITendency aiTendency)
        {
            // 预制数据
            var startCluster = _zombieAIUnit.GetCluster(startVertex);
            var endCluster = _zombieAIUnit.GetCluster(endVertex);


            // $"调用 GetMultiPaths({startVertex.Position}->{endVertex.Position}) | IsIdentical: {startCluster.IsIdentical(endCluster)}".LogInfo();

            // 简单情况处理，这时不需要调用寻路算法
            if (startCluster.IsIdentical(endCluster))
            {
                var _ = _zombieAIUnit.FindKeyEdgeInOneKeyEdge(startVertex, endVertex);
                return new List<Path> { new Path(_) };
            }

            if (startCluster.GetIntersection(endCluster, out var intersection))
            {
                var path = new Path();

                var firstEdge = _zombieAIUnit.FindKeyEdgeInOneKeyEdge(startVertex, intersection);
                path.Add(firstEdge);

                var secondEdge = _zombieAIUnit.FindKeyEdgeInOneKeyEdge(intersection, endVertex);
                path.Add(secondEdge);

                return new List<Path> { path };
            }

            // 调用寻路算法

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
                    KeyEdge start, end;

                    var keyVertexStart = path.keyEdges.First().From;
                    var keyVertexEnd = path.keyEdges.Last().To;

                    if (startVertex == keyVertexStart) start = null;
                    else if (startVertex.isKey)
                        start = _zombieAIUnit.keyAdjacencyList[startVertex]
                            .First(keyEdge => keyEdge.To == keyVertexStart);
                    else
                        start = _zombieAIUnit.FindKeyEdgesToAdjacentKeyVertices(startVertex)
                            .First(keyEdge => keyEdge.To == keyVertexStart);

                    if (endVertex == keyVertexEnd) end = null;
                    else if (endVertex.isKey)
                        end = _zombieAIUnit.keyAdjacencyList[endVertex]
                            .First(keyEdge => keyEdge.From == keyVertexEnd);
                    else
                    {
                        if (endVertex == endCluster.vertexA)
                        {
                            var AToB = _zombieAIUnit.keyAdjacencyList[endCluster.vertexA]
                                .First(e => e.To == endCluster.vertexB);
                            end = _zombieAIUnit.GetKeyEdgeInKeyEdge(endCluster.vertexA, endVertex, AToB);
                        }
                        else
                        {
                            var BToA = _zombieAIUnit.keyAdjacencyList[endCluster.vertexB]
                                .First(e => e.To == endCluster.vertexA);
                            end = _zombieAIUnit.GetKeyEdgeInKeyEdge(endCluster.vertexB, endVertex, BToA);
                        }
                    }

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