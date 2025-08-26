using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Public;

namespace TPL.PVZR.Classes.ZombieAI.Class
{
    public class EdgeWeightCalculator
    {
        public static float GetWeight(MoveType edgeType, AITendency.MainAI mainAI, int heightDifference)
        {
            // 简化处理复合类型
            edgeType = edgeType switch
            {
                MoveType.Swim_WalkJump => MoveType.WalkJump,
                MoveType.Climb_Swim => MoveType.ClimbLadder,
                MoveType.Climb_WalkJump => MoveType.ClimbLadder,
                _ => edgeType
            };

            switch (mainAI)
            {
                case AITendency.MainAI.Default:
                    return edgeType switch
                    {
                        MoveType.WalkJump => 10f,
                        MoveType.Swim => 20f,
                        MoveType.Fall => 1f,
                        MoveType.HumanLadder => 40f * heightDifference,
                        MoveType.ClimbLadder => 10f,
                        _ => 10f
                    };
                case AITendency.MainAI.CanPutLadder:
                    return edgeType switch
                    {
                        MoveType.WalkJump => 10f,
                        MoveType.Swim => 20f,
                        MoveType.Fall => 1f,
                        MoveType.HumanLadder => 1f,
                        MoveType.ClimbLadder => 10f,
                        _ => 10f
                    };
                case AITendency.MainAI.CanSwim:
                    return edgeType switch
                    {
                        MoveType.WalkJump => 10f,
                        MoveType.Swim => 3f,
                        MoveType.Fall => 1f,
                        MoveType.HumanLadder => 40f * heightDifference,
                        MoveType.ClimbLadder => 10f,
                        _ => 10f
                    };
                default:
                    $"出现未考虑的AI倾向，使用默认权重: {edgeType}, {mainAI}".LogWarning();
                    return 10f;
            }
        }
    }
}