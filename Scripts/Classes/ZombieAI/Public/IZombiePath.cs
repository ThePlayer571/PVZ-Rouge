using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.ZombieAI.Class;
using UnityEngine;

namespace TPL.PVZR.Classes.ZombieAI.Public
{
    public interface IZombiePath
    {
        MoveData NextTarget();
        int Count { get; }
    }

    public class ZombiePath : IZombiePath
    {
        #region 接口实现: IZombiePath

        public MoveData NextTarget()
        {
            if (moveQueue.Count <= 0) throw new Exception("move queue is empty");
            return moveQueue.Dequeue();
        }

        public int Count => moveQueue.Count;

        #endregion

        private Queue<MoveData> moveQueue = new Queue<MoveData>();

        public ZombiePath(Path path)
        {
            foreach (var keyEdge in path.keyEdges.Where(keyEdge => keyEdge.To.isKey))
            {
                moveQueue.Enqueue(new MoveData(keyEdge.moveType, keyEdge.To.Position, MoveStage.FollowVertex));
            }


            var referenceMoveType = path.finalMoveType ?? path.keyEdges.Last().moveType;

            var moveType = referenceMoveType switch
            {
                MoveType.ClimbLadder => MoveType.ClimbLadder,
                MoveType.Swim => MoveType.Swim,
                MoveType.Climb_Swim => MoveType.ClimbLadder,
                _ => MoveType.WalkJump
            };


            moveQueue.Enqueue(new MoveData(moveType, Vector2Int.zero, MoveStage.FindDave));
        }
    }
}