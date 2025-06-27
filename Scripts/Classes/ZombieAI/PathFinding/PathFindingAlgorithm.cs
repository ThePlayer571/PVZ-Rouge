using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Gameplay.Class.ZombieAI.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.PathFinding
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
            keyAdjacencyList[endCluster.vertexA].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexB));
            keyAdjacencyList[endCluster.vertexB].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexA));
            //
            var a = new AStarPathfinder(keyAdjacencyList);
            var pathToA = a.FindPath(startCluster, endCluster.vertexA, aiTendency);
            var pathToB = a.FindPath(startCluster, endCluster.vertexB, aiTendency);
            var result = new List<Path> { pathToA, pathToB }.Where(p => p != null).ToList();
            if (!result.Any())
            {
                throw new Exception("AStarFindSiegePath首次未发现任何路");
            }
            return result ;
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