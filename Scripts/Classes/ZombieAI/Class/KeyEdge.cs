using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.ZombieAI.Public;

namespace TPL.PVZR.Classes.ZombieAI.Class
{
    /// <summary>
    /// 表示两个 KeyVertex 之间的连接
    /// </summary>
    public class KeyEdge
    {
        #region 字段与属性

        public List<Edge> includeEdges { get; private set; } = new();

        public MoveType moveType { get; private set; }


        public AllowedPassHeight allowedPassHeight { get; private set; }
        public Vertex From => includeEdges.First().From;
        public Vertex To => includeEdges.Last().To;

        /// <summary>
        /// 缓存每种 AITendency.MainAI 对应的权重
        /// </summary>
        private readonly Dictionary<AITendency.MainAI, float> _weightCache = new();

        #endregion

        #region 公有方法

        public List<Vertex> IncludeVertices()
        {
            var vertices = new List<Vertex>();
            vertices.Add(includeEdges.First().From);
            vertices.AddRange(includeEdges.Select(e => e.To));
            return vertices;
        }

        public float Weight(AITendency aiTendency)
        {
            if (_weightCache.TryGetValue(aiTendency.mainAI, out var cachedWeight))
            {
                return cachedWeight;
            }

            float totalWeight = includeEdges.Sum(edge => edge.Weight(aiTendency));
            _weightCache[aiTendency.mainAI] = totalWeight;
            return totalWeight;
        }

        public KeyEdge Adversed(Dictionary<Vertex, List<Edge>> adjacencyList)
        {
            var reversedEdges = new List<Edge>();
            for (int i = includeEdges.Count - 1; i >= 0; i--)
            {
                var oldEdge = includeEdges[i];
                var newEdge = adjacencyList[oldEdge.To].FirstOrDefault(edge => Equals(edge.To, oldEdge.From));
                if (newEdge is null) throw new Exception("该边无法反转");
                reversedEdges.Add(newEdge);
            }

            this.includeEdges = reversedEdges;
            return new KeyEdge(this, reversedEdges);
        }

        public KeyEdge AddEdge(Edge edge)
        {
            // moveType
            if (this.moveType != MoveType.NotSet && this.moveType != edge.moveType)
                throw new Exception(
                    $"无法添加不同类型的边. current: ({From.Position})->({To.Position}), {this.moveType}; new: ({edge.From.Position})->({edge.To.Position}), {edge.moveType}");
            if (this.moveType == MoveType.NotSet) this.moveType = edge.moveType;

            // allowedPassHeight
            if (edge.allowedPassHeight == AllowedPassHeight.One &&
                this.allowedPassHeight == AllowedPassHeight.TwoAndMore)
            {
                this.allowedPassHeight = AllowedPassHeight.One;
            }

            includeEdges.Add(edge);

            // 清除缓存，确保后续调用 Weight 时重新计算
            _weightCache.Clear();

            return this;
        }

        #endregion

        #region 构造函数

        public KeyEdge(MoveType moveType = MoveType.NotSet)
        {
            includeEdges = new List<Edge>();
            this.moveType = moveType;
            this.allowedPassHeight = AllowedPassHeight.TwoAndMore;
        }

        public KeyEdge(Edge startEdge)
        {
            includeEdges.Add(startEdge);
            this.moveType = startEdge.moveType;
            this.allowedPassHeight = startEdge.allowedPassHeight;
        }

        public KeyEdge(KeyEdge startEdge, List<Edge> includeEdges = null)
        {
            this.includeEdges = includeEdges ?? startEdge.includeEdges.ToList();
            this.moveType = startEdge.moveType;
            this.allowedPassHeight = startEdge.allowedPassHeight;
        }

        #endregion
    }
}