using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Gameplay.Class.ZombieAI.Class;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Public
{
    /// <summary>
    /// 表示一条路径，由多个 KeyEdge 组成
    /// </summary>
    public class Path
    {
        #region 字段与属性

        public List<KeyEdge> keyEdges { get; }

        /// <summary>
        /// 缓存每种 AITendency.MainAI 对应的路径权重
        /// </summary>
        private readonly Dictionary<AITendency.MainAI, float> _weightCache;

        #endregion

        #region 公有方法

        /// <summary>
        /// 根据 AITendency 计算路径的总权重
        /// </summary>
        /// <param name="aiTendency">AI 倾向</param>
        /// <returns>路径的总权重</returns>
        public float Weight(AITendency aiTendency)
        {
            // 优先从缓存中获取权重
            if (_weightCache.TryGetValue(aiTendency.mainAI, out var cachedWeight))
                return cachedWeight;

            // 如果缓存中没有，则重新计算并更新缓存
            float totalWeight = keyEdges.Sum(keyEdge => keyEdge.Weight(aiTendency));
            _weightCache[aiTendency.mainAI] = totalWeight;
            return totalWeight;
        }

        /// <summary>
        /// 添加一个 KeyEdge 到路径中
        /// </summary>
        /// <param name="keyEdge">要添加的 KeyEdge</param>
        public void Add(KeyEdge keyEdge)
        {
            if (keyEdges.Count > 0 && keyEdges.Last().To != keyEdge.From)
            {
                throw new System.Exception("KeyEdge 的起点必须与当前路径的终点相同");
            }

            keyEdges.Add(keyEdge);
            // 清除缓存，确保后续调用 Weight 时重新计算
            _weightCache.Clear();
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造一个路径，由起始 KeyEdge、已有路径和结束 KeyEdge 组成
        /// </summary>
        /// <param name="startKeyEdge">起始 KeyEdge</param>
        /// <param name="path">已有路径</param>
        /// <param name="endKeyEdge">结束 KeyEdge</param>
        public Path(KeyEdge startKeyEdge, Path path, KeyEdge endKeyEdge)
        {
            keyEdges = new List<KeyEdge>();
            _weightCache = new Dictionary<AITendency.MainAI, float>();

            if (startKeyEdge != null) this.keyEdges.Add(startKeyEdge);
            this.keyEdges.AddRange(path.keyEdges);
            if (endKeyEdge != null) this.keyEdges.Add(endKeyEdge);
        }

        public Path(KeyEdge keyEdge)
        {
            keyEdges = new List<KeyEdge> { keyEdge };
            _weightCache = new Dictionary<AITendency.MainAI, float>();
        }

        /// <summary>
        /// 构造一个空路径
        /// </summary>
        public Path()
        {
            keyEdges = new List<KeyEdge>();
            _weightCache = new Dictionary<AITendency.MainAI, float>();
        }

        #endregion
    }
}