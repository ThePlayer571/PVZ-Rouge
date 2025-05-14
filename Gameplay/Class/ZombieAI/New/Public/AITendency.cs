using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Core.Random;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit
{
    /// <summary>
    /// 表示 AI 的行为倾向
    /// </summary>
    public class AITendency
    {
        #region 枚举

        /// <summary>
        /// 主行为倾向
        /// </summary>
        public enum MainAI
        {
            Default,
            CanSwim,
            CanPutLadder
        }

        #endregion

        #region 字段与属性

        /// <summary>
        /// 随机种子，用于路径选择
        /// </summary>
        public readonly float seed;

        /// <summary>
        /// 允许通过的最小高度
        /// </summary>
        public AllowedPassHeight minPassHeight;

        /// <summary>
        /// 主行为倾向
        /// </summary>
        public MainAI mainAI;

        #endregion

        #region 公有方法

        /// <summary>
        /// 根据当前 AI 倾向过滤路径列表，移除不符合条件的路径
        /// </summary>
        /// <param name="paths">路径列表</param>
        public void ApplyFilter(List<IPath> paths)
        {
            // TODO: 实现路径过滤逻辑
        }

        /// <summary>
        /// 根据随机种子和路径权重选择一条路径
        /// </summary>
        /// <param name="paths">路径列表</param>
        /// <returns>选择的路径</returns>
        public IPath ChooseOnePath(List<IPath> paths)
        {
            // 调整权重，使用平方值增加差异性
            var adjustedWeights = paths.Select(p => (float)Math.Pow(p.Weight(this), 2)).ToArray();
            float total = adjustedWeights.Sum();
            float randomValue = seed * total;

            float cumulative = 0;
            for (int i = 0; i < adjustedWeights.Length; i++)
            {
                cumulative += adjustedWeights[i];
                if (randomValue <= cumulative)
                {
                    return paths[i];
                }
            }

            return paths.Last();
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造一个 AI 倾向实例
        /// </summary>
        /// <param name="minPassHeight">允许通过的最小高度</param>
        /// <param name="chooseClosestPath">是否选择最近路径（未使用）</param>
        public AITendency(AllowedPassHeight minPassHeight = AllowedPassHeight.TwoAndMore,
            bool chooseClosestPath = false)
        {
            this.minPassHeight = minPassHeight;
            seed = (float)RandomHelper.Default.Value;
        }

        #endregion
    }
}
