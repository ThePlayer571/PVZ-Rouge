using System;
using TPL.PVZR.Classes.ZombieAI.Public;
using UnityEngine;

namespace TPL.PVZR.Classes.ZombieAI.Class
{
    public enum VertexType
    {
        Plat,
        Ladder,
        PlatWithLadder,
        Water
    }

    public static class VertexTypeExtensions
    {
        public static bool IsPlat(this VertexType vertexType)
        {
            return vertexType is VertexType.Plat or VertexType.PlatWithLadder;
        }

        public static bool IsLadder(this VertexType vertexType)
        {
            return vertexType is VertexType.Ladder or VertexType.PlatWithLadder;
        }

        public static bool IsWater(this VertexType vertexType)
        {
            return vertexType == VertexType.Water;
        }

        public static MoveType ToMoveType(this VertexType vertexType)
        {
            return vertexType switch
            {
                VertexType.Plat => MoveType.WalkJump,
                VertexType.Ladder => MoveType.ClimbLadder,
                VertexType.PlatWithLadder => MoveType.WalkJump,
                VertexType.Water => MoveType.Swim,
                _ => throw new ArgumentOutOfRangeException(nameof(vertexType), vertexType, null)
            };
        }
    }

    public class Vertex
    {
        #region 构造函数

        public Vertex(int x, int y, VertexType vertexType, int passableHeight, bool isKey = false)
        {
            this.x = x;
            this.y = y;
            this.VertexType = vertexType;
            this.PassableHeight = Math.Min(passableHeight, AITendency.PASSABLE_HEIGHT_最大值);
            this.isKey = isKey;
        }

        #endregion

        #region 数据存储

        public readonly int x;
        public readonly int y;

        /// <summary>
        /// Vertex之上有的空间（能允许几格高的僵尸通过）
        /// </summary>
        public int PassableHeight;

        public VertexType VertexType;

        /// <summary>
        /// 是否是关键结点
        /// </summary>
        /// <remarks>定义：两个邻接关键结点之间的路，可以用同一种移动方式通过</remarks>>
        public bool isKey;

        #endregion

        public Vector2Int Position => new Vector2Int(x, y);

        #region override

        public override bool Equals(object obj)
        {
            if (obj is Vertex vertex)
            {
                return this.x == vertex.x && this.y == vertex.y;
            }

            throw new ArgumentException("与不允许的类型相比");
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.x, this.y);
        }

        public static bool operator ==(Vertex a, Vertex b) => a?.x == b?.x && a?.y == b?.y;
        public static bool operator !=(Vertex a, Vertex b) => !(a == b);

        #endregion
    }
}