using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Class;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.ZombieAI.Public
{
    /// <summary>
    /// 表示 AI 的行为倾向
    /// </summary>
    public class AITendency
    {
        public const int PASSABLE_HEIGHT_最大值 = 3; // 请至少设为“游戏内最大僵尸高度+1”，不然存在“高度差”的地形都无法正确处理

        public readonly MainAI mainAI;
        public readonly int minPassableHeight;
        public readonly float seed;
        public readonly bool chooseClosestPath;


        #region 公有方法

        /// <summary>
        /// 判断该边是否被禁止（该ai绝对不能通过）
        /// </summary>
        /// <param name="keyEdge"></param>
        /// <returns></returns>
        public bool IsBannedKeyEdge(KeyEdge keyEdge, bool withDebug = false)
        {
            if (keyEdge.passableHeight < minPassableHeight)
            {
                if (withDebug)
                    $"find ban: {keyEdge}".LogInfo();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 根据随机种子和路径权重选择一条路径
        /// </summary>
        /// <param name="paths">路径列表</param>
        /// <param name="factor">算法的影响因子</param>
        /// <returns>选择的路径</returns>
        public Path ChooseOnePath(List<Path> paths, float factor = 0.01f)
        {
            // 异常处理
            if (paths == null || paths.Count == 0) throw new ArgumentException("路径列表不能为空");

            // 提前返回：当只有一条路径
            if (paths.Count == 1) return paths.First();
            // 提前返回：当启用chooseClosestPath
            if (chooseClosestPath)
            {
                return paths.MinBy(path => path.Weight(this));
            }

            // == 核心逻辑
            var weights = paths.Select((p => p.Weight(this))).ToArray();
            float minWeight = weights.Min();

            // 计算每个路径的score（对差值敏感）
            var scores = weights.Select(w => MathF.Exp(-(w - minWeight) * factor)).ToArray();
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
        public AITendency(MainAI mainAI, int minPassableHeight, bool chooseClosestPath = false)
        {
            this.mainAI = mainAI;
            this.minPassableHeight = minPassableHeight;
            seed = (float)RandomHelper.Default.Value;
            this.chooseClosestPath = chooseClosestPath;
        }

        #endregion

        #region SHIT

        public override int GetHashCode()
        {
            // seed不参与HashCode计算
            return HashCode.Combine(mainAI, minPassableHeight, chooseClosestPath);
        }

        public override bool Equals(object obj)
        {
            if (obj is AITendency other)
            {
                return this.mainAI == other.mainAI &&
                       this.minPassableHeight == other.minPassableHeight &&
                       this.chooseClosestPath == other.chooseClosestPath;
            }

            // seed不参与Equals计算
            throw new Exception();
        }

        public enum MainAI
        {
            Default,
            CanSwim,
            CanPutLadder
        }

        public static AITendency Default => new AITendency(MainAI.Default, 2);
        public static AITendency CanSwim => new AITendency(MainAI.CanSwim, 2);

        public static AITendency Imp => new AITendency(MainAI.Default, 1);

        #endregion
    }
}