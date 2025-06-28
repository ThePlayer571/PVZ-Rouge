using System;
using TPL.PVZR.Helpers.Methods;
using UnityEngine;

namespace TPL.PVZR.Classes.ZombieAI.Public
{
    [Serializable]
    public class MoveData
    {
        public MoveType moveType;
        public Vector2Int target;
        public MoveStage moveStage;

        [NonSerialized]
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