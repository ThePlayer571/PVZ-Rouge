using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Class;
using TPL.PVZR.Classes.ZombieAI.Public;

namespace TPL.PVZR.Classes.ZombieAI.PathFinding
{
    public class PathFindingAlgorithm
    {
        private readonly Dictionary<Vertex, List<KeyEdge>> _keyAdjacencyList;

        public PathFindingAlgorithm(Dictionary<Vertex, List<KeyEdge>> keyAdjacencyList)
        {
            this._keyAdjacencyList = keyAdjacencyList;
        }

        // 提示：终点是单个结点时无法围攻
        public List<Path> AStarFindSiegePath(Cluster startCluster, Cluster endCluster, AITendency aiTendency)
        {
            // 预制数据
            var keyAdjacencyList = this._keyAdjacencyList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            // 清除 endCluster 的连接（以便生成围攻路径）
            keyAdjacencyList[endCluster.vertexA].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexB));
            keyAdjacencyList[endCluster.vertexB].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexA));
            // 清除AITendency不允许的边
            foreach (var kvp in keyAdjacencyList)
            {
                kvp.Value.RemoveAll(keyEdge => aiTendency.IsBannedKeyEdge(keyEdge));
            }

            //
            var a = new AStarPathfinder(keyAdjacencyList);
            var pathToA = a.FindPath(startCluster, endCluster.vertexA, aiTendency);
            var pathToB = a.FindPath(startCluster, endCluster.vertexB, aiTendency);
            var result = new List<Path> { pathToA, pathToB }.Where(p => p != null).ToList();
            if (!result.Any())
            {
                throw new Exception("AStarFindSiegePath首次未发现任何路");
            }

            return result;
        }

        public List<Path> AStarFindSiegePath(Vertex startVertex, Cluster endCluster, AITendency aiTendency)
        {
            // 预制数据
            var keyAdjacencyList = this._keyAdjacencyList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            keyAdjacencyList[endCluster.vertexA].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexB));
            keyAdjacencyList[endCluster.vertexB].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexA));
            //
            var a = new AStarPathfinder(keyAdjacencyList);
            var pathToA = a.FindPath(startVertex, endCluster.vertexA, aiTendency);
            var pathToB = a.FindPath(startVertex, endCluster.vertexB, aiTendency);
            var result = new List<Path> { pathToA, pathToB }.Where(p => p != null).ToList();
            if (!result.Any())
            {
                throw new Exception("AStarFindSiegePath首次未发现任何路");
            }

            return result;
        }
    }
}