using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Gameplay.Class.ZombieAI.New.Others;
using TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.New.PathFinding
{
    public class PathFindingAlgorithm
    {
        
        private readonly Dictionary<Vertex, List<IKeyEdge>> _keyAdjacencyList;

        public PathFindingAlgorithm(Dictionary<Vertex, List<IKeyEdge>> keyAdjacencyList)
        {
            this._keyAdjacencyList = keyAdjacencyList;
        }

        public List<IPath> AStarFindSiegePath(Cluster startCluster, Cluster endCluster, AITendency aiTendency)
        {
            // 预制数据
            var keyAdjacencyList = this._keyAdjacencyList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            keyAdjacencyList[endCluster.vertexA].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexB));
            keyAdjacencyList[endCluster.vertexB].RemoveAll(keyEdge => keyEdge.To.Equals(endCluster.vertexA));
            //
            var a = new AStarPathfinder(keyAdjacencyList);
            var pathToA = a.FindPath(startCluster, endCluster.vertexA, aiTendency);
            var pathToB = a.FindPath(startCluster, endCluster.vertexB, aiTendency);
            return new List<IPath> { pathToA, pathToB };
        }
    }
}