using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using TPL.PVZR.Services;
using UnityEngine;

namespace TPL.PVZR.Models
{
    public interface IPhaseModel : IModel
    {
        GamePhase GamePhase { get; set; }
        bool IsInRoughPhase(RoughPhase roughPhase);
    }

    public class PhaseModel : AbstractModel, IPhaseModel
    {
        public GamePhase GamePhase { get; set; }

        private readonly Dictionary<GamePhase, HashSet<RoughPhase>> _phaseToRoughPhaseCache = new();

        protected override void OnInit()
        {
            InitializePhaseCache();
            GamePhase = GamePhase.BeforeStart;
        }

        public bool IsInRoughPhase(RoughPhase roughPhase)
        {
            if (_phaseToRoughPhaseCache.TryGetValue(GamePhase, out var roughPhases))
            {
                return roughPhases.Contains(roughPhase);
            }

            // 如果没有找到对应的RoughPhase配置，返回false
            return false;
        }

        private void InitializePhaseCache()
        {
            var gamePhaseType = typeof(GamePhase);
            var gamePhaseValues = Enum.GetValues(gamePhaseType).Cast<GamePhase>();

            foreach (var gamePhase in gamePhaseValues)
            {
                var fieldInfo = gamePhaseType.GetField(gamePhase.ToString());
                if (fieldInfo == null) continue;

                var roughPhaseAttribute = fieldInfo.GetCustomAttribute<RoughPhaseAttribute>();
                if (roughPhaseAttribute?.RoughPhases != null)
                {
                    _phaseToRoughPhaseCache[gamePhase] = new HashSet<RoughPhase>(roughPhaseAttribute.RoughPhases);
                }
                else
                {
                    // 没有属性标记的GamePhase使用空集合
                    _phaseToRoughPhaseCache[gamePhase] = new HashSet<RoughPhase>();
                }
            }
        }
    }


    public enum RoughPhase
    {
        /* 
         * 【过程】需要玩家操作退出
         * 【短过程】进入后隔一段时间自动退出
         * 【瞬时】进入后立刻退出
         */
        Game,
        Level,
        Process,
        ShortProcess,
        Instant,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RoughPhaseAttribute : Attribute
    {
        public RoughPhase[] RoughPhases { get; }

        public RoughPhaseAttribute(params RoughPhase[] roughPhases)
        {
            RoughPhases = roughPhases;
        }
    }
}