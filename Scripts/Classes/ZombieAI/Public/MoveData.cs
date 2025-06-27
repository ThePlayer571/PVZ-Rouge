using TPL.PVZR.Core;
using TPL.PVZR.Helpers.Methods;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Public
{
    public class MoveData
    {
        public readonly MoveType moveType;
        public readonly Vector2Int target;
        public readonly MoveStage moveStage;

        private Vector2? _cachedWorldPos;

        public Vector2 targetWorldPos
        {
            get
            {
                if (_cachedWorldPos == null)
                {
                    // 假设有一个静态工具类用于转换
                    _cachedWorldPos = LevelGridHelper.CellToWorld(target);
                }
                return _cachedWorldPos.Value;
            }
        }

        public MoveData(MoveType moveType, Vector2Int target, MoveStage moveStage)
        {
            this.moveType = moveType;
            this.target = target;
            this.moveStage = moveStage;
        }
    }
}