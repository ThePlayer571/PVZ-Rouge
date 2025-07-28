using System;
using TPL.PVZR.Helpers.New.Methods;
using UnityEngine;

namespace TPL.PVZR.Classes.ZombieAI.Public
{
    [Serializable]
    public class MoveData
    {
        public MoveType moveType;
        public Vector2Int target;
        public Vector2Int from;
        public MoveStage moveStage;

        
        private Vector2? _cachedTargetWorldPos;
        private Vector2? _cachedFromWorldPos;
        

        public Vector2 targetWorldPos
        {
            get
            {
                _cachedTargetWorldPos ??= LevelGridHelper.CellToWorld(target);

                return _cachedTargetWorldPos.Value;
            }
        }
        public Vector2 fromWorldPos
        {
            get
            {
                _cachedFromWorldPos ??= LevelGridHelper.CellToWorld(from);

                return _cachedFromWorldPos.Value;
            }
        }

        public MoveData(MoveType moveType, Vector2Int target, Vector2Int from, MoveStage moveStage)
        {
            this.moveType = moveType;
            this.target = target;
            this.from = from;
            this.moveStage = moveStage;
        }
    }
}