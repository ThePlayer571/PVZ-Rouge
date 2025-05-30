using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Public
{
    public class MoveData
    {
        public readonly MoveType moveType;
        public readonly Vector2Int target;
        public readonly MoveStage moveStage;

        public MoveData(MoveType moveType, Vector2Int target, MoveStage moveStage)
        {
            this.moveType = moveType;
            this.target = target;
            this.moveStage = moveStage;
        }
    }
}