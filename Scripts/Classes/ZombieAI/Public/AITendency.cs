using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Class;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.ZombieAI.Public
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

        public readonly float seed;

        public readonly AllowedPassHeight minPassHeight;
        public readonly MainAI mainAI;

        #endregion

        #region 属性

        public static AITendency Default => new AITendency(MainAI.Default, AllowedPassHeight.TwoAndMore);

        #endregion

        #region 公有方法

        /// <summary>
        /// 根据当前 AI 倾向过滤路径列表，移除不符合条件的路径
        /// </summary>
        /// <param name="paths">路径列表</param>
        public void ApplyFilter(List<Path> paths)
        {
            // TODO: 实现路径过滤逻辑
        }

        /// <summary>
        /// 根据随机种子和路径权重选择一条路径
        /// </summary>
        /// <param name="paths">路径列表</param>
        /// <param name="factor">算法的影响因子</param>
        /// <returns>选择的路径</returns>
        public Path ChooseOnePath(List<Path> paths, float factor = 0.01f)
        {
            if (paths == null || paths.Count == 0) throw new ArgumentException("路径列表不能为空");
            if (paths.Count == 1) return paths.First();

            var weights = paths.Select((p => p.Weight(this))).ToArray();
            // $"scores是{String.Join(",", weights.Select(s => s.ToString("F3")))}".LogInfo(); // 修改为三位小数输出
            float minWeight = weights.Min();

            // 计算每个路径的score（对差值敏感）
            var scores = weights.Select(w => MathF.Exp(-(w - minWeight) * factor)).ToArray();
            // $"scores是{String.Join(",", scores.Select(s => s.ToString("F3")))}".LogInfo(); // 修改为三位小数输出
            float total = scores.Sum();
            float randomValue = total * seed;
            float cumulative = 0f;
            for (int i = 0; i < scores.Length; i++)
            {
                cumulative += scores[i];
                if (cumulative >= randomValue)
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
        /// <param name="mainAI"></param>
        /// <param name="minPassHeight">允许通过的最小高度</param>
        /// <param name="chooseClosestPath">是否选择最近路径（未使用）</param>
        public AITendency(MainAI mainAI, AllowedPassHeight minPassHeight = AllowedPassHeight.TwoAndMore,
            bool chooseClosestPath = false)
        {
            this.mainAI = mainAI;
            this.minPassHeight = minPassHeight;
            seed = (float)RandomHelper.Default.Value;
            
            $"this seed is {seed}".LogInfo();
        }

        #endregion

        #region override

        public override int GetHashCode()
        {
            // seed不参与HashCode计算
            return HashCode.Combine(minPassHeight, mainAI);
        }

        public override bool Equals(object obj)
        {
            if (obj is AITendency other)
            {
                return this.minPassHeight == other.minPassHeight && this.mainAI == other.mainAI;
            }

            // seed不参与Equals计算
            throw new Exception();
        }

        #endregion
    }
}