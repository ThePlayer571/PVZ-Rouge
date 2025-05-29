using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Public
{
    public interface IZombiePath
    {
        Vector2Int TargetPosition { get; }
        MoveType MoveType { get; }
    }
    
    
    public enum MoveType
    {
        WalkJump,
        Fall,
        Water,
        HumanLadder,
        ClimbLadder
    }
}