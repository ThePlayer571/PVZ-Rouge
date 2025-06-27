using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Public
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
            foreach (var keyEdge in path.keyEdges)
            {
                if (keyEdge.To.isKey)
                {
                    moveQueue.Enqueue(new MoveData(keyEdge.moveType, keyEdge.To.Position, MoveStage.FollowVertex));
                }
            }

            moveQueue.Enqueue(new MoveData(path.keyEdges.Last().moveType, Vector2Int.zero, MoveStage.FindDave));
        }
    }
}