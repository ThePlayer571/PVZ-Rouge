using System.Collections.Generic;
using TPL.PVZR.Classes.ZombieAI.Public;

namespace TPL.PVZR.Classes.ZombieAI.Class
{
    // Copilot
    public class EdgeWeightCalculator
    {
        private static readonly Dictionary<(MoveType, AITendency.MainAI), float> _weightRules = new()
        {
            // 根据文档表格填充权重规则
            { (MoveType.WalkJump, AITendency.MainAI.Default), 10f },
            { (MoveType.WalkJump, AITendency.MainAI.CanPutLadder), 10f },
            { (MoveType.WalkJump, AITendency.MainAI.CanSwim), 10f },

            { (MoveType.Swim, AITendency.MainAI.Default), 20f },
            { (MoveType.Swim, AITendency.MainAI.CanPutLadder), 20f },
            { (MoveType.Swim, AITendency.MainAI.CanSwim), 3f },

            { (MoveType.Fall, AITendency.MainAI.Default), 1f },
            { (MoveType.Fall, AITendency.MainAI.CanPutLadder), 1f },
            { (MoveType.Fall, AITendency.MainAI.CanSwim), 1f },

            { (MoveType.HumanLadder, AITendency.MainAI.Default), 10000f },
            { (MoveType.HumanLadder, AITendency.MainAI.CanPutLadder), 1f },
            { (MoveType.HumanLadder, AITendency.MainAI.CanSwim), 10000f },

            { (MoveType.ClimbLadder, AITendency.MainAI.Default), 10f },
            { (MoveType.ClimbLadder, AITendency.MainAI.CanPutLadder), 10f },
            { (MoveType.ClimbLadder, AITendency.MainAI.CanSwim), 10f },

            { (MoveType.Climb_WalkJump, AITendency.MainAI.Default), 10f },
            { (MoveType.Climb_WalkJump, AITendency.MainAI.CanPutLadder), 10f },
            { (MoveType.Climb_WalkJump, AITendency.MainAI.CanSwim), 10f },
        };

        public float GetWeight(MoveType edgeType, AITendency.MainAI aiTendency)
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