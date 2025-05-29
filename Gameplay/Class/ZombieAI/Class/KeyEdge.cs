using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Class
{
    /// <summary>
    /// 表示两个 KeyVertex 之间的连接
    /// </summary>
    public interface IKeyEdge : IEdge
    {
        /// <summary>
        /// 包含的所有 Edge
        /// </summary>
        List<IEdge> includeEdges { get; }

        List<Vertex> IncludeVertices();

        /// <summary>
        /// 添加一个 Edge 到 KeyEdge 中
        /// </summary>
        /// <param name="edge">要添加的 Edge</param>
        /// <returns>更新后的 KeyEdge</returns>
        IKeyEdge AddEdge(IEdge edge);

        /// <summary>
        /// 将 KeyEdge 反转
        /// </summary>
        /// <param name="adjacencyList">邻接表</param>
        /// <returns>反转后的 KeyEdge</returns>
        IKeyEdge Adversed(Dictionary<Vertex, List<IEdge>> adjacencyList);
    }

    /// <summary>
    /// KeyEdge 的实现类
    /// </summary>
    public class KeyEdge : IKeyEdge
    {
        #region 字段与属性

        public List<IEdge> includeEdges { get; private set; } = new();
        public Edge.EdgeType edgeType { get; }
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

        public IKeyEdge Adversed(Dictionary<Vertex, List<IEdge>> adjacencyList)
        {
            var reversedEdges = new List<IEdge>();
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

        public IKeyEdge AddEdge(IEdge edge)
        {
            if (edge.allowedPassHeight is AllowedPassHeight.One &&
                this.allowedPassHeight is AllowedPassHeight.TwoAndMore)
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

        public KeyEdge(IEdge startEdge)
        {
            includeEdges.Add(startEdge);
            this.edgeType = startEdge.edgeType;
            this.allowedPassHeight = startEdge.allowedPassHeight;
        }

        public KeyEdge(IKeyEdge startEdge, List<IEdge> includeEdges = null)
        {
            this.includeEdges = includeEdges ?? startEdge.includeEdges.ToList();
            this.edgeType = startEdge.edgeType;
            this.allowedPassHeight = startEdge.allowedPassHeight;
        }

        #endregion
    }
}
