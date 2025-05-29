using System.Collections.Generic;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Class
{
    // Copilot
    public class EdgeWeightCalculator
    {
        private readonly Dictionary<(Edge.EdgeType, AITendency.MainAI), float> _weightRules;

        public EdgeWeightCalculator()
        {
            _weightRules = new Dictionary<(Edge.EdgeType, AITendency.MainAI), float>
            {
                // 根据文档表格填充权重规则
                {(Edge.EdgeType.WalkJump, AITendency.MainAI.Default), 10f},
                {(Edge.EdgeType.WalkJump, AITendency.MainAI.CanPutLadder), 10f},
                {(Edge.EdgeType.WalkJump, AITendency.MainAI.CanSwim), 10f},

                {(Edge.EdgeType.Water, AITendency.MainAI.Default), 20f},
                {(Edge.EdgeType.Water, AITendency.MainAI.CanPutLadder), 20f},
                {(Edge.EdgeType.Water, AITendency.MainAI.CanSwim), 3f},

                {(Edge.EdgeType.Fall, AITendency.MainAI.Default), 1f},
                {(Edge.EdgeType.Fall, AITendency.MainAI.CanPutLadder), 1f},
                {(Edge.EdgeType.Fall, AITendency.MainAI.CanSwim), 1f},

                {(Edge.EdgeType.HumanLadder, AITendency.MainAI.Default), 100f},
                {(Edge.EdgeType.HumanLadder, AITendency.MainAI.CanPutLadder), 1f},
                {(Edge.EdgeType.HumanLadder, AITendency.MainAI.CanSwim), 100f},

                {(Edge.EdgeType.ClimbLadder, AITendency.MainAI.Default), 10f},
                {(Edge.EdgeType.ClimbLadder, AITendency.MainAI.CanPutLadder), 10f},
                {(Edge.EdgeType.ClimbLadder, AITendency.MainAI.CanSwim), 10f},
            };
        }

        public float GetWeight(Edge.EdgeType edgeType, AITendency.MainAI aiTendency)
        {
            if (_weightRules.TryGetValue((edgeType, aiTendency), out var weight))
            {
                return weight;
            }

            // 默认权重
            return 1.0f;
        }
    }
}
