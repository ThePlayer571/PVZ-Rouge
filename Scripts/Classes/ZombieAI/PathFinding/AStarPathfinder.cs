using System.Collections.Generic;
using TPL.PVZR.Core.PriorityQueue;
using TPL.PVZR.Gameplay.Class.ZombieAI.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.PathFinding
{
    // use deepseek && ChatGPT
    public class AStarPathfinder
    {
        // 邻接表结构：Dictionary<起点, List<带权边>>
        private readonly Dictionary<Vertex, List<KeyEdge>> _keyAdjacencyList;

        public AStarPathfinder(Dictionary<Vertex, List<KeyEdge>> keyAdjacencyList)
        {
            _keyAdjacencyList = keyAdjacencyList;
        }

        #region AStarNode

        private class AStarNode : FastPriorityQueueNode
        {
            public float gCost;
            public float hCost;
            public float fCost => gCost + hCost;

            public KeyEdge fromEdge;
            public Vertex vertex;
            public Vector2Int position => new Vector2Int(vertex.x, vertex.y);
            public AStarNode parent;


            public AStarNode(KeyEdge fromEdge, Vertex vertex, AStarNode parent, float gCost, float hCost)
            {
                this.fromEdge = fromEdge;
                this.vertex = vertex;
                this.parent = parent;
                this.gCost = gCost;
                this.hCost = hCost;
            }
        }

        #endregion

        // 对外接口 - 单起点
        public Path FindPath(Vertex start, Vertex end, AITendency aiTendency)
        {
            return FindPathInternal(start, end, aiTendency);
        }

        // 对外接口 - 双起点（Cluster）
        public Path FindPath(Cluster clusterStart, Vertex end, AITendency aiTendency)
        {
            return FindPathInternal(clusterStart, end, aiTendency);
        }

        #region 私有

        private float Heuristic(Vertex start, Vertex end)
        {
            // 每格的Weight为10
            return 10 * (Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y));
        }

        private Path ReconstructPath(AStarNode current)
        {
            Path result = new Path();
            Stack<KeyEdge> keyEdges = new(); // 使用栈反转路径方向
            while (current.parent != null)
            {
                if (current.fromEdge != null)
                    keyEdges.Push(current.fromEdge);
                current = current.parent;
            }

            while (keyEdges.Count > 0)
            {
                result.Add(keyEdges.Pop());
            }

            return result;
        }

        private Path FindPathInternal(Cluster startCluster, Vertex end, AITendency aiTendency)
        {
            if (startCluster.Include(end)) return new Path();
            var vertexA = startCluster.vertexA;
            var vertexB = startCluster.vertexB;

            if (!_keyAdjacencyList.ContainsKey(vertexA) || !_keyAdjacencyList.ContainsKey(end))
                throw new System.ArgumentException("Start/End not in graph");

            var frontier = new FastPriorityQueue<AStarNode>(1000); // TODO: 动态调整容量
            var vertexToNode = new Dictionary<Vertex, AStarNode>();

            void AddStartNode(Vertex v)
            {
                if (v == null) return; // 防止空引用
                var startNode = new AStarNode(null, v, null, 0, Heuristic(v, end));
                frontier.Enqueue(startNode, startNode.fCost);
                vertexToNode[v] = startNode;
            }

            AddStartNode(vertexA);
            if (!_keyAdjacencyList.ContainsKey(vertexB))
                Debug.LogWarning("vertexB not in graph, treating as single-point start");
            else if (!vertexB.Equals(vertexA)) // 避免重复
                AddStartNode(vertexB);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.vertex == end)
                    return ReconstructPath(current);

                if (!_keyAdjacencyList.TryGetValue(current.vertex, out var edges)) continue;

                foreach (KeyEdge edge in edges)
                {
                    var neighbor = edge.To;
                    float tentativeGCost = current.gCost + edge.Weight(aiTendency);

                    if (vertexToNode.TryGetValue(neighbor, out var neighborNode))
                    {
                        if (neighborNode.gCost > tentativeGCost)
                        {
                            neighborNode.fromEdge = edge;
                            neighborNode.parent = current;
                            neighborNode.gCost = tentativeGCost;
                            frontier.UpdatePriority(neighborNode, neighborNode.fCost);
                        }
                    }
                    else
                    {
                        neighborNode = new AStarNode(edge, edge.To, current, tentativeGCost, Heuristic(neighbor, end));
                        vertexToNode[neighbor] = neighborNode;
                        frontier.Enqueue(neighborNode, neighborNode.fCost);
                    }
                }
            }

            // Debug.LogWarning(
            //     $"Path not found, from:({startCluster.vertexA.x},{startCluster.vertexA.y})({startCluster.vertexB.x},{startCluster.vertexB.y}), to: ({end.x},{end.y})");
            return null; // 或抛出异常
        }

        private Path FindPathInternal(Vertex start, Vertex end, AITendency aiTendency)
        {
            // 验证输入
            if (!_keyAdjacencyList.ContainsKey(start) || !_keyAdjacencyList.ContainsKey(end))
                throw new System.ArgumentException("Start/End not in graph");

            // 初始化数据结构
            var frontier = new FastPriorityQueue<AStarNode>(maxNodes: 1000);
            // vertexToNode 用于从 Vertex 映射到其对应的 A* 节点信息（如 gCost、fCost 等）
            // 注意：Vertex 为外部图结构，不适合存储 A* 状态
            var vertexToNode = new Dictionary<Vertex, AStarNode>();

            // 初始化起点
            var startNode = new AStarNode(null, start, null, 0, Heuristic(start, end));
            frontier.Enqueue(startNode, startNode.fCost);

            vertexToNode.Add(start, startNode);

            while (frontier.Count > 0)
            {
                AStarNode current = frontier.Dequeue();

                if (current.vertex == end)
                {
                    return ReconstructPath(current);
                }

                // 遍历邻接边
                foreach (KeyEdge edge in _keyAdjacencyList[current.vertex])
                {
                    Vertex neighbor = edge.To;
                    float tentativeGCost = current.gCost + edge.Weight(aiTendency);

                    // 发现更优路径
                    if (!vertexToNode.ContainsKey(neighbor))
                    {
                        var neighborNode = new AStarNode(edge, edge.To, current, tentativeGCost,
                            Heuristic(neighbor, end));
                        vertexToNode[neighbor] = neighborNode;

                        frontier.Enqueue(neighborNode, neighborNode.fCost);
                    }
                    else if (vertexToNode[neighbor].gCost > tentativeGCost)
                    {
                        var neighborNode = vertexToNode[neighbor];
                        neighborNode.fromEdge = edge;
                        neighborNode.gCost = tentativeGCost;

                        frontier.UpdatePriority(neighborNode, neighborNode.fCost);
                    }
                }
            }

            // 未找到任何路径
            return null;
        }

        #endregion
    }
}